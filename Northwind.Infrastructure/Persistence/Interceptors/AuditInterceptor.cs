using Microsoft.EntityFrameworkCore.Diagnostics;
using Northwind.Domain.Common;
using Northwind.Domain.Common.Interfaces;
using static Microsoft.EntityFrameworkCore.EntityState;

namespace Northwind.Infrastructure.Persistence.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuditInterceptor(IDateTimeProvider dateTimeProvider) 
        {
            _dateTimeProvider = dateTimeProvider;
        }

        private void SetCreateUpdateDates(DbContextEventData eventData)
        {
            var now = _dateTimeProvider.GetDateTime();
            foreach (var entityEntry in eventData.Context.ChangeTracker.Entries<EntityBase>())
            {
                if (entityEntry.State == Added)
                {
                    entityEntry.Entity.Created = now;
                }

                if (entityEntry.State is Added or Modified)
                {
                    entityEntry.Entity.LastModified = now;
                }
            }
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetCreateUpdateDates(eventData);
            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            SetCreateUpdateDates(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
