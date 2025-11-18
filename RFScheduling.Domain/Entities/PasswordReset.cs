namespace RFScheduling.Domain.Entities
{
    public class PasswordReset
    {
        public int PasswordResetId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpireAt { get; set; }

        public DateTime? UseAt { get; set; }

        public DateTime CreatedDate { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
    }
}
