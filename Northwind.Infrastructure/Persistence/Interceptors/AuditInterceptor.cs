using Microsoft.EntityFrameworkCore.Diagnostics;
using Northwind.Application.Interfaces;
using Northwind.Domain.Common;
using static Microsoft.EntityFrameworkCore.EntityState;

namespace Northwind.Infrastructure.Persistence.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;

        public AuditInterceptor(IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService) 
        {
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        private void SetCreateUpdateDates(DbContextEventData eventData)
        {
            var now = _dateTimeProvider.GetDateTime();
            foreach (var entityEntry in eventData.Context.ChangeTracker.Entries<EntityBase>())
            {
                if (entityEntry.State == Added)
                {
                    entityEntry.Entity.Created = now;
                    entityEntry.Entity.CreatedBy = _currentUserService.UserId;
                }

                if (entityEntry.State is Added or Modified)
                {
                    entityEntry.Entity.LastModified = now;
                    entityEntry.Entity.LastModifiedBy = _currentUserService.UserId;
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
