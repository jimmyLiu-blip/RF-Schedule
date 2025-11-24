using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Enums;
using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities.Scheduling
{
    public class Regulation : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int RegulationId { get; set; }

        public int ProjectId { get; set; }

        [MaxLength(100)]
        public string RegulationName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public RegulationStatus Status { get; set; } = RegulationStatus.NotStarted;

        public bool ManualStatusOverride { get; set; } = false;

        [MaxLength(500)]
        public string? Note {  get; set; }

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // ISoftDeletable
        public bool IsDeleted { get; set; }

        public int? DeletedByUserId { get; set; }

        public DateTime? DeletedDate { get; set; }

        // Concurrency Token
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation Properties
        public Project Project { get; set; } = null!;

        public User CreatedBy { get; set; } = null!;

        public User? ModifiedBy { get; set; } 

        public ICollection<TestItem> TestItems { get; set; } = new List<TestItem>();
    }
}
