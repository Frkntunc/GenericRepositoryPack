using Domain.Entities;
using Domain.Entities.Common;
using Shared.Enums;

namespace Domain.Entities;

public class User : Entity<User, long>, IAuditableEntity
{
    private User(string email, string firstName, string lastName, string passwordHash)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        Status = StatusType.Available;
        LoginTryCount = 0;
        IsBlocked = false;
        LastPasswordChangeDate = null;
    }

    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public StatusType Status { get; private set; }
    public string PasswordHash { get; private set; } = null!;
    public DateTime? LastPasswordChangeDate { get; private set; }
    public int LoginTryCount { get; private set; }
    public bool IsBlocked { get; private set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public static User Create(string email, string firstName, string lastName, string passwordHash)
    {
        return new User(email, firstName, lastName, passwordHash);
    }

    public void ChangeStatus(StatusType newStatus)
    {
        Status = newStatus;
    }

    public void BlockUser()
    {
        IsBlocked = true;
    }

    public void SetEmail(string email)
    {
        Email = email;
    }

    public void SetFirstName(string firstName)
    {
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        LastName = lastName;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        LastPasswordChangeDate = DateTime.UtcNow;
    }
}
