using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities
{
    public class SystemSetting
    {
        public int SettingId { get; set; }

        [MaxLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? SettingValue { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
