﻿using PulseService.Domain.Adapters;
using PulseService.Domain.Models;
using PulseService.Domain.Models.Dtos;

namespace PulseService.Domain.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IUserRepository _userRepository;

        public UserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<UserDto?> GetUserByCredentialsAsync(UserCredentials credentials)
        {
            return await _userRepository.GetUserByCredentialsAsync(credentials);
        }

        public async Task<bool> UsernameIsTakenAsync(string username)
        {
            if (await GetUserByUsernameAsync(username) is not null)
            {
                return true;
            }

            return false;
        }
    }
}