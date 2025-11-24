using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Enums;
using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities.Scheduling
{
    public class WorkLog : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int WorkLogId { get; set; }

        public int TestItemId { get; set; }

        public int? RevisionId { get; set; }

        public int EngineerUserId { get; set; }

        public DateTime WorkDate { get; set; }

        public decimal ActualHours { get; set; }

        public WorkLogStatus Status { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public int? DelayReasonId { get; set; } 

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [MaxLength(500)]
        public string? ModificationReason { get; set; }

        // ISoftDeletable
        public bool IsDeleted { get; set; } = false;

        public int? DeletedByUserId { get; set; }

        public DateTime? DeletedDate { get; set; }

        // Concurrency Token
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation Properties

        public TestItem TestItem { get; set; } = null!;

        public TestItemRevision? TestItemRevision { get; set; }

        public User CreatedBy { get; set; } = null!;

        public User? ModifiedBy { get; set; }

        public User Engineer { get; set; } = null!;

        public DelayReason? DelayReason { get; set; }
    }
}
