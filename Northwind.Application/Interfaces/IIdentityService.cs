﻿using Northwind.Application.Models;

namespace Northwind.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);

    }
}
