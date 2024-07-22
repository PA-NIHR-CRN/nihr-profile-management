using NIHR.ProfileManagement.Domain.EnumsAndConstants;

namespace NIHR.ProfileManagement.Domain.Models
{
    public class OutboxItem
    {
        public int Id { get; private set; }

        public string Payload { get; private set; }

        public string SourceSystem { get; private set; }

        public string EventType { get; private set; }

        public DateTime? ProcessingStartDate { get; private set; }

        public DateTime? ProcessingCompletedDate { get; private set; }

        public OutboxStatus Status { get; private set; }

        public OutboxItem(int id,
            string payload,
            string sourceSystem,
            string eventType,
            DateTime? processingStartDate,
            DateTime? processingCompletedDate,
            OutboxStatus status)
        {
            Id = id;
            Payload = payload;
            SourceSystem = sourceSystem;
            EventType = eventType;
            ProcessingStartDate = processingStartDate;
            ProcessingCompletedDate = processingCompletedDate;
            Status = status;
        }
    }
}
