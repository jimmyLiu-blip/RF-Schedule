using RFScheduling.Domain.Interfaces;
using RFScheduling.Domain.Enums;


namespace RFScheduling.Domain.Entities
{
    public class DelayReason : ICreatableRequired, IModifiable
    {
        public int DelayReasonId { get; set; }

        public string ReasonText { get; set; } = string.Empty;

        public DelayReasonType DelayReasonType { get; set; }

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
