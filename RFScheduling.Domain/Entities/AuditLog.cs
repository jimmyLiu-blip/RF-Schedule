namespace RFScheduling.Domain.Entities
{
    public class AuditLog
    {
        public long AuditLogId { get; set; }

        public string TableName { get; set; } = string.Empty;

        public int RecordId { get; set; }

        public string Action {  get; set; } = string.Empty ;

        public string? OldValue { get; set; }

        public string? NewValue { get; set;}

        public int UserId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string? Reason { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
    }
}
