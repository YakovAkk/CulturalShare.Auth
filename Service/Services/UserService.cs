﻿using AuthenticationProto;
using DomainEntity.Entities;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Repository.Repositories;
using Service.Services.Base;

namespace Service.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordService _passwordService;
    private readonly IUserRepository _userRepository;

    public UserService(
        ILogger<UserService> logger,
        IPasswordService passwordService,
        IUserRepository userRepository)
    {
        _logger = logger;
        _passwordService = passwordService;
        _userRepository = userRepository;
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
}
