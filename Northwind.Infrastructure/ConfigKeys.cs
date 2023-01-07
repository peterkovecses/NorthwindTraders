namespace Northwind.Infrastructure
{
    public static class ConfigKeys
    {
        public const string TokenValidateIssuer = "TokenValidationParameters:ValidateIssuer";
        public const string TokenValidateAudience = "TokenValidationParameters:ValidateAudience";
        public const string TokenValidateLifetime = "TokenValidationParameters:ValidateLifetime";
        public const string TokenValidateIssuerSigningKey = "TokenValidationParameters:ValidateIssuerSigningKey";
        public const string TokenValidIssuer = "TokenValidationParameters:ValidIssuer";
        public const string TokenValidAudience = "TokenValidationParameters:ValidAudience";
        public const string TokenSecret = "TokenValidationParameters:Secret";
        public const string TokenLifeTime = "TokenValidationParameters:TokenLifeTime";
        public const string IdentityPasswordRequireDigit = "IdentityServer:PasswordOptions:RequireDigit";
        public const string IdentityPasswordRequireLowercase = "IdentityServer:PasswordOptions:RequireLowercase";
        public const string IdentityPasswordRequireUppercase = "IdentityServer:PasswordOptions:RequireUppercase";
        public const string IdentityPasswordNonAlphanumeric = "IdentityServer:PasswordOptions:NonAlphanumeric";
        public const string IdentityUserRequireUniqueEmail = "IdentityServer:UserOptions:RequireUniqueEmail";
    }
}
