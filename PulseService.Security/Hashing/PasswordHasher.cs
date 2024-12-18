﻿using System.Security.Cryptography;
using System.Text;
using PulseService.Domain.Adapters;

namespace PulseService.Security.Hashing;

internal class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var hashedData = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(hashedData);
    }
}