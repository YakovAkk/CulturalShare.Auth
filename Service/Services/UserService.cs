using AuthenticationProto;
using DomainEntity.Entities;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Repositories;
using Service.Services.Base;

namespace Service.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordService _passwordService;
    private readonly IUserRepository _userRepository;
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly IFollowerEntityRepository _followerEntityRepository;
    private readonly IRestrictedUserEntityRepository _restrictedUserEntityRepository;

    public UserService(
        ILogger<UserService> logger,
        IPasswordService passwordService,
        IUserRepository userRepository,
        IUserSettingsRepository userSettingsRepository,
        IFollowerEntityRepository followerEntityRepository,
        IRestrictedUserEntityRepository restrictedUserEntityRepository)
    {
        _logger = logger;
        _passwordService = passwordService;
        _userRepository = userRepository;
        _userSettingsRepository = userSettingsRepository;
        _followerEntityRepository = followerEntityRepository;
        _restrictedUserEntityRepository = restrictedUserEntityRepository;
    }

    public async Task<ErrorOr<int>> CreateUserAsync(CreateUserRequest request)
    {
        _logger.LogDebug($"{nameof(CreateUserAsync)} request. User = {request.FirstName} {request.LastName} registered");

         (byte[] passwordHash, byte[] passwordSalt) = _passwordService.CreatePasswordHash(request.Password);

        var user = new UserEntity(request.FirstName, request.LastName, request.Email, passwordHash, passwordSalt);

        var result = _userRepository.Add(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogDebug($"Customer {user.Id} was created.");

        return user.Id;
    }

    public async Task<ErrorOr<Empty>> FollowUserAsync(FollowUserRequest request, int userId)
    {
        if (request.FolloweeId == userId)
        {
            return Error.Conflict("You cannot follow yourself.");
        }

        var userExists = _userRepository.GetAll().Any(u => u.Id == userId);
        var restrictedUserExists = _userRepository.GetAll().Any(u => u.Id == request.FolloweeId);

        if (!userExists && !restrictedUserExists)
        {
            return Error.Conflict("User doesn't exist.");
        }

        var existingFollow = await _followerEntityRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.FollowerId == userId && x.FolloweeId == request.FolloweeId);

        if (existingFollow is not null)
        {
            if (existingFollow.IsFollow)
            {
                return Error.Conflict("You are already following this user.");
            }

            existingFollow.Follow();
            existingFollow.UpdatedAt = DateTime.UtcNow;
            _followerEntityRepository.Update(existingFollow);
        }
        else
        {
            var newFollow = new FollowerEntity(userId, request.FolloweeId);
            _followerEntityRepository.Add(newFollow);
        }

        await _followerEntityRepository.SaveChangesAsync();
        return new Empty();
    }

    public async Task<ErrorOr<Empty>> UnfollowUserAsync(UnfollowUserRequest request, int userId)
    {
        var followRecord = await _followerEntityRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.FollowerId == userId && x.FolloweeId == request.FolloweeId);

        if (followRecord is null || !followRecord.IsFollow)
        {
            return Error.Conflict("You are not following this user.");
        }

        followRecord.Unfollow();
        _followerEntityRepository.Update(followRecord);
        await _followerEntityRepository.SaveChangesAsync();

        return new Empty();
    }

    public async Task<ErrorOr<Empty>> AllowUserAsync(AllowUserRequest request, int userId)
    {
        var restriction = await _restrictedUserEntityRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RestrictedUserId == request.UserIdToAllowObserving);

        if (restriction is null || !restriction.IsRestricted)
        {
            return Error.Conflict("You are not restricting this user.");
        }

        restriction.Unrestrict();
        _restrictedUserEntityRepository.Update(restriction);
        await _restrictedUserEntityRepository.SaveChangesAsync();

        return new Empty();
    }

    public async Task<ErrorOr<Empty>> RestrictUserAsync(RestrictUserRequest request, int userId)
    {
        if (request.UserIdToRestrict == userId)
        {
            return Error.Conflict("You cannot restrict yourself.");
        }

        var userExists = _userRepository.GetAll().Any(u => u.Id == userId);
        var restrictedUserExists = _userRepository.GetAll().Any(u => u.Id == request.UserIdToRestrict);

        if (!userExists && !restrictedUserExists)
        {
            return Error.Conflict("User doesn't exist.");
        }

        var restriction = await _restrictedUserEntityRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RestrictedUserId == request.UserIdToRestrict);

        if (restriction is not null)
        {
            if (restriction.IsRestricted)
            {
                return Error.Conflict("You are already restricting this user.");
            }

            restriction.Restrict();
            restriction.UpdatedAt = DateTime.UtcNow;
            _restrictedUserEntityRepository.Update(restriction);
        }
        else
        {
            var newRestriction = new RestrictedUserEntity(userId, request.UserIdToRestrict);
            _restrictedUserEntityRepository.Add(newRestriction);
        }

        await _restrictedUserEntityRepository.SaveChangesAsync();
        return new Empty();
    }

    public async Task<ErrorOr<SearchUserResponse>> SearchUserByNameAsync(SearchUserRequest request)
    {
        var users = await _userRepository.GetAll()
            .Where(x => x.FirstName.Contains(request.Name) || x.LastName.Contains(request.Name))
            .Select(x => new UserInfo
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            })
            .ToListAsync();

        var result = new SearchUserResponse
        {
            Users = { users }
        };

        return result;
    }

    public async Task<ErrorOr<Empty>> ToggleNotificationsAsync(ToggleNotificationsRequest request, int userId)
    {
        _logger.LogInformation($"{nameof(ToggleNotificationsAsync)} request received");

        var userExists = _userRepository.GetAll().Any(u => u.Id == userId);

        if (!userExists)
        {
            return Error.Conflict("User doesn't exist.");
        }

        var userSettings = await _userSettingsRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (userSettings == null)
        {
            userSettings = new UserSettingsEntity(request.NotificationsEnabled, userId);
            _userSettingsRepository.Add(userSettings);
        }
        else
        {
            userSettings.NotificationsEnabled = request.NotificationsEnabled;
            _userSettingsRepository.Update(userSettings);
        }

        await _userSettingsRepository.SaveChangesAsync();

        return new Empty();
    }
}
