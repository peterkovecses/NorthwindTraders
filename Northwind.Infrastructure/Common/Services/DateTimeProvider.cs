using Northwind.Domain.Common.Interfaces;

namespace Northwind.Infrastructure.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetDateTime() => DateTime.UtcNow;
    }
}
