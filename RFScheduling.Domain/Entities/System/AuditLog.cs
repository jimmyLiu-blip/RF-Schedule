using RFScheduling.Domain.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RFScheduling.Domain.Entities.System
{
    public class AuditLog
    {
        public long AuditLogId { get; set; }

        [MaxLength(50)]

        public string TableName { get; set; } = string.Empty;

        public int RecordId { get; set; }

        [MaxLength(20)]
        public string Action {  get; set; } = string.Empty ;

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? OldValue { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? NewValue { get; set;}

        public int UserId { get; set; }

        public DateTime ModifiedDate { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
    }
}
