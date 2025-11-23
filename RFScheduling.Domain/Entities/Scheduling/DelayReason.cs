using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Enums;
using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;


namespace RFScheduling.Domain.Entities.Scheduling
{
    public class DelayReason : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int DelayReasonId { get; set; }

        [MaxLength(200)]
        public string ReasonText { get; set; } = string.Empty;

        public DelayReasonType ReasonType { get; set; }

        public bool IsActive { get; set; } = true;

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // ISoftDeletable
        public bool IsDeleted { get; set; }

        public int? DeletedByUserId { get; set; }

        public DateTime? DeletedDate { get; set; }

        // Navigation Properties

        public User CreatedBy { get; set; } = null!;

        public User? ModifiedBy { get; set; }

        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
