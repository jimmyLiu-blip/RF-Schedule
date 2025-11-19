using RFScheduling.Domain.Enums;
using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;


namespace RFScheduling.Domain.Entities
{
    public class DelayReason : ICreatableRequired, IModifiable
    {
        public int DelayReasonId { get; set; }

        [MaxLength(200)]
        public string ReasonText { get; set; } = string.Empty;

        public DelayReasonType ReasonType { get; set; }

        public bool IsActive { get; set; } = true;

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties

        public User CreatedBy { get; set; } = null!;

        public ICollection<WorkLogDelayReason> WorkLogDelayReasons { get; set; } = new List<WorkLogDelayReason>();
    }
}
