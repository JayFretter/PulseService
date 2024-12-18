﻿using PulseService.Domain.Adapters;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Handlers;

public class UserHandler : IUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        user.Password = _passwordHasher.Hash(user.Password);
        await _userRepository.AddUserAsync(user, cancellationToken);
    }

    public async Task<BasicUserCredentials?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserByUsernameAsync(username, cancellationToken);
    }

    public async Task<BasicUserCredentials?> GetUserByCredentialsAsync(UserCredentials credentials,
        CancellationToken cancellationToken)
    {
        credentials.Password = _passwordHasher.Hash(credentials.Password);
        return await _userRepository.GetUserByCredentialsAsync(credentials, cancellationToken);
    }

    public async Task<bool> UsernameIsTakenAsync(string username, CancellationToken cancellationToken)
    {
        return await GetUserByUsernameAsync(username, cancellationToken) is not null;
    }
}