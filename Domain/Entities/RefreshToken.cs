using Domain.Entities.Common;

namespace Domain.Entities
{
    public class RefreshToken : Entity<RefreshToken, Guid>
    {
        private RefreshToken(string userId)
        {
            Token = Guid.NewGuid().ToString("N");
            UserId = userId;
            ExpiryDate = DateTime.UtcNow.AddDays(7);
            IsRevoked = false;
        }

        public string Token { get; private set; } = null!;
        public string UserId { get; private set; } = null!;
        public DateTime ExpiryDate { get; private set; }
        public bool IsRevoked { get; private set; } = false;
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;

        public static RefreshToken Create(string userId)
        {
            return new RefreshToken(userId);
        }

        public void ChangeRevokeStatus(bool status)
        {
            IsRevoked = status;
        }
    }
}
