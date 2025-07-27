using Domain.Entities;
using Domain.Entities.Common;
using Shared.Enums;

namespace Domain.Entities;

public class User : Entity<User, long>
{
    public string Email { get; protected set; }
    public string FirstName { get; protected set; }
    public string LastName { get; protected set; }
    public StatusType Status { get; protected set; }
    public string PasswordHash { get; protected set; } = null!;
    public DateTime? LastPasswordChangeDate { get; protected set; }
    public int LoginTryCount { get; protected set; }
    public bool IsBlocked { get; protected set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
