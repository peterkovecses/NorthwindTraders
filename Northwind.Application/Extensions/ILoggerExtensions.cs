using Microsoft.Extensions.Logging;

namespace Northwind.Application.Extensions
{
    public static class ILoggerExtensions
    {
        public static void LogError<T>(this ILogger<T> logger, Exception exception)
        {
            logger.LogError($"{exception.GetType()} occurred: {exception.Message}");
        }
    }
}
