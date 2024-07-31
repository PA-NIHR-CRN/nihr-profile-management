using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.EnumsAndConstants;
using NIHR.ProfileManagement.Domain.Models;
using NIHR.ProfileManagement.Infrastructure.Repository;
using NIHR.ProfileManagement.Infrastructure.Repository.Models;
using System.Text.Json;

namespace NIHR.ProfileManagement.Infrastructure.Repository
{
    public class ProfileOutboxRepository : IProfileOutboxRepository
    {
        private readonly ProfileManagementDbContext _context;
        private readonly INsipMessageHelper _nsipGrisMessageHelper;

        public ProfileOutboxRepository(ProfileManagementDbContext context,
            INsipMessageHelper nsipGrisMessageHelper)
        {
            _context = context;
            _nsipGrisMessageHelper = nsipGrisMessageHelper;
        }

        public async Task AddToOutboxAsync(AddToOuxboxRequest request, CancellationToken cancellationToken)
        {
            // Wrap the payload json in an NSIP message
            var nsipMessage = _nsipGrisMessageHelper.Prepare(request.EventType, request.SourceSystem, request.Payload);

            // Serialize the NSIP message using standard serialization options.
            var nsipMessageAsPayload = JsonSerializer.Serialize(nsipMessage);

            await _context.OutboxRecords.AddAsync(new OutboxRecordDbEntity
            {
                EventType = request.EventType,
                SourceSystem = request.SourceSystem,
                Payload = nsipMessageAsPayload
            });
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public OutboxItem? GetNextUnprocessedOutboxItem()
        {
            var nextOutboxItem = _context.OutboxRecords
                 .FirstOrDefault(outbox => outbox.Status == OutboxStatus.Created);

            if(nextOutboxItem == null)
            {
                return null;
            }

            return new OutboxItem(nextOutboxItem.Id,
                nextOutboxItem.Payload,
                nextOutboxItem.SourceSystem,
                nextOutboxItem.EventType,
                nextOutboxItem.ProcessingStartDate,
                nextOutboxItem.ProcessingCompletedDate,
                nextOutboxItem.Status);
        }

        public async Task UpdateOutboxItemStatusAsync(int id, OutboxStatus status, DateTime? processingStartedTime = null, DateTime? processingCompletedTime = null)
        {
            var recordToUpdate = await _context
                .OutboxRecords
                .FindAsync(id);

            if(recordToUpdate == null)
            {
                throw new KeyNotFoundException($"Could not find outbox entry for id {id}");
            }

            recordToUpdate.Status = status;

            recordToUpdate.ProcessingStartDate = processingStartedTime.HasValue
                ? processingStartedTime.Value
                : recordToUpdate.ProcessingStartDate;

            recordToUpdate.ProcessingCompletedDate = processingCompletedTime.HasValue
                ? processingCompletedTime.Value
                : recordToUpdate.ProcessingCompletedDate;
        }
    }
}
