namespace Northwind.Infrastructure
{
    public static class ConfigKeys
    {
        public const string TokenSecret = "TokenValidationParameters:Secret";
        public const string TokenLifeTime = "TokenValidationParameters:TokenLifeTime";
        public const string IdentityPasswordRequireDigit = "IdentityServer:PasswordOptions:RequireDigit";
        public const string IdentityPasswordRequireLowercase = "IdentityServer:PasswordOptions:RequireLowercase";
        public const string IdentityPasswordRequireUppercase = "IdentityServer:PasswordOptions:RequireUppercase";
        public const string IdentityPasswordNonAlphanumeric = "IdentityServer:PasswordOptions:NonAlphanumeric";
        public const string IdentityUserRequireUniqueEmail = "IdentityServer:UserOptions:RequireUniqueEmail";
    }
}
