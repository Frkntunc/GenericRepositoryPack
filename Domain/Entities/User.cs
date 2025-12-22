using Domain.Entities;
using Domain.Entities.Common;
using Shared.Enums;

namespace Domain.Entities;

public class User : Entity<User, long> , IAuditableEntity
{
    public string Email { get; internal set; }
    public string FirstName { get; internal set; }
    public string LastName { get; internal set; }
    public StatusType Status { get; internal set; }
    public string PasswordHash { get; internal set; } = null!;
    public DateTime? LastPasswordChangeDate { get; internal set; }
    public int LoginTryCount { get; internal set; }
    public bool IsBlocked { get; internal set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
