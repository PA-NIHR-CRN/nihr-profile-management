
using NIHR.ProfileManagement.Domain.EnumsAndConstants;

namespace NIHR.ProfileManagement.Infrastructure.Repository.Models
{
    public partial class OutboxRecordDbEntity : DbEntity
    {
        public int Id { get; set; }

        public string Payload { get; set; }

        public string SourceSystem { get; set; }

        public string EventType { get; set; }

        public DateTime? ProcessingStartDate { get; set; }

        public DateTime? ProcessingCompletedDate { get; set; }

        public OutboxStatus Status { get; set; }

        public OutboxRecordDbEntity()
        {
            Payload = "";
            SourceSystem = "";
            EventType = "";
            Status = OutboxStatus.Created;
        }
    }
}
