using RFScheduling.Domain.Interfaces;
using RFScheduling.Domain.Enums;

namespace RFScheduling.Domain.Entities
{
    public class WorkLogDelayReason
    {
        public int WorkLogDelayReasonId { get; set; }

        public int WorkLogId { get; set; }

        public int DelayReasonId { get; set; }

        // Navigation Properties
        public WorkLog WorkLog { get; set; } = null!;

        public DelayReason DelayReason { get; set; } = null!;
    }
}
