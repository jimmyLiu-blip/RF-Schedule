using RFScheduling.Domain.Entities.Shared;

namespace RFScheduling.Domain.Entities.System
{
    public class PasswordReset
    {
        public int PasswordResetId { get; set; }

        public int UserId { get; set; }

        public Guid Token { get; set; }

        public DateTime ExpireAt { get; set; } 

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedDate { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
    }
}
