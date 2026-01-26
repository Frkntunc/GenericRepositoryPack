using ApplicationService.SharedKernel.Auth.Common;
using Domain.Entities;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace Persistence.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IUserContext _currentUserService;

        public AuditInterceptor(IUserContext currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;

            if (dbContext == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            // Değişiklikleri yakala
            var auditEntries = new List<AuditLog>();
            var entries = dbContext.ChangeTracker.Entries<IAuditableEntity>(); // Sadece IAuditableEntity olanlar

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = AuditLog.Create
                (
                    entry.Entity.GetType().Name,
                    _currentUserService.UserId ?? "System", // Kullanıcı yoksa System
                    DateTime.UtcNow,
                    entry.State.ToString()
                );

                var oldValues = new Dictionary<string, object>();
                var newValues = new Dictionary<string, object>();
                var affectedColumns = new List<string>();

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.IsTemporary) continue; // ID oluşmadıysa geç

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            newValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            oldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                oldValues[propertyName] = property.OriginalValue;
                                newValues[propertyName] = property.CurrentValue;
                                affectedColumns.Add(propertyName);
                            }
                            break;
                    }
                }

                auditEntry.SetOldValues(oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues));
                auditEntry.SetNewValues(newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues));
                auditEntry.SetAffectedColumns(affectedColumns.Count == 0 ? null : JsonSerializer.Serialize(affectedColumns));

                // Primary Key yakalama (Insert işlemlerinde Id SaveChanges'dan sonra oluşur, 
                // tam profesyonel çözüm için 2 aşamalı kayıt gerekir ama basitlik adına burada geçiyoruz)
                // Update/Delete için:
                if (entry.State != EntityState.Added)
                {
                    // Entity'nin Primary Key değerini bulma mantığı buraya eklenebilir.
                    // Basit haliyle:
                    // auditEntry.PrimaryKey = entry.Property("Id").CurrentValue.ToString();
                }

                auditEntries.Add(auditEntry);
            }

            // Audit logları veritabanına ekle
            // Not: Bu kısım normal transaction'a dahil olur. Asıl işlem hata alırsa log da atılmaz (ki doğrusu budur).
            if (auditEntries.Count > 0)
            {
                await dbContext.Set<AuditLog>().AddRangeAsync(auditEntries);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
