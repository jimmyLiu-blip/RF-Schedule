namespace RFScheduling.Domain.Interfaces
{
    public interface ICreatableNullable
    {
        int? CreatedByUserId { get; set; }

        DateTime CreatedDate { get; set; }
    }
}
