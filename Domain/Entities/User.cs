using Domain.Entities;
using Domain.Entities.Common;
using Shared.Enums;

namespace Domain.Entities;

public class User : Entity<User, long>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public StatusType Status { get; set; }
    public string PasswordHash { get; set; } = null!;
    public DateTime? LastPasswordChangeDate { get; set; }
    public int LoginTryCount { get; set; }
    public bool IsBlocked { get; set; }
    public ICollection<UserRole> Roles { get; set; }
}
