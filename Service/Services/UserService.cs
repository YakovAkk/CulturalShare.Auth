using AuthenticationProto;
using DomainEntity.Entities;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Repositories;
using Service.Services.Base;
using System.Security.Claims;

namespace Service.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordService _passwordService;
    private readonly IUserRepository _userRepository;
    private readonly IUserSettingsRepository _userSettingsRepository;

    public UserService(
        ILogger<UserService> logger,
        IPasswordService passwordService,
        IUserRepository userRepository,
        IUserSettingsRepository userSettingsRepository)
    {
        _logger = logger;
        _passwordService = passwordService;
        _userRepository = userRepository;
        _userSettingsRepository = userSettingsRepository;
    }

    public Task<ErrorOr<Empty>> AllowUserAsync(AllowUserRequest request)
    {
        throw new NotImplementedException();
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

    public Task<ErrorOr<Empty>> FollowUserAsync(FollowUserRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Empty>> RestrictUserAsync(RestrictUserRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<SearchUserResponse>> SearchUserByNameAsync(SearchUserRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<Empty>> ToggleNotificationsAsync(ToggleNotificationsRequest request, HttpContext httpContext)
    {
        _logger.LogInformation("SignOut request received");

        var userIdResult = ExtractUserIdFromContext(httpContext);

        if (userIdResult.IsError)
        {
            _logger.LogWarning("SignOut failed: {Error}", userIdResult.FirstError.Description);
            return userIdResult.Errors;
        }

        var userId = userIdResult.Value;

        var userSettings = await _userSettingsRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (userSettings == null)
        {
            userSettings = new UserSettingsEntity(request.Enable, userId);
            _userSettingsRepository.Add(userSettings);
        }
        else
        {
            userSettings.NotificationsEnabled = request.Enable;
            _userSettingsRepository.Update(userSettings);
        }

        await _userSettingsRepository.SaveChangesAsync();

        return new Empty();
    }

    public Task<ErrorOr<Empty>> UnfollowUserAsync(UnfollowUserRequest request)
    {
        throw new NotImplementedException();
    }

    #region Private

    private ErrorOr<int> ExtractUserIdFromContext(HttpContext? httpContext)
    {
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return Error.Unauthorized("Unauthorized access");
        }

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Error.Unauthorized("Invalid user identifier");
        }

        return userId;
    }

    #endregion
}
