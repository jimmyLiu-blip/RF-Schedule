using RFScheduling.Domain.Interfaces;
using RFScheduling.Domain.Enums;

namespace RFScheduling.Domain.Entities
{
    public class TestItemRevision : ISoftDeletable, ICreatable, IModifiable
    {
        public int RevisionId { get; set; }


    }
}
