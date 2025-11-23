using System.ComponentModel.DataAnnotations;
using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Interfaces;

namespace RFScheduling.Domain.Entities.System
{
    public class SystemSetting : IModifiable
    {
        public int SettingId { get; set; }

        [MaxLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [MaxLength(500)]
        public string SettingValue { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        public User? ModifiedBy { get; set; }
    }
}
