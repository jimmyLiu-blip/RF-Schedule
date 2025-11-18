namespace RFScheduling.Domain.Entities
{
    public class SystemSetting
    {
        public int SettingId { get; set; }

        public string SettingKey { get; set; } = string.Empty;

        public string? SettingValue { get; set; }

        public string? Description { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
