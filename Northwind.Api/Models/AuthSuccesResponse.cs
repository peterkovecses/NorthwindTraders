﻿namespace Northwind.Application.Models
{
    public class AuthSuccesResponse
    {
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
