

using Microsoft.Extensions.Logging;
using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.EnumsAndConstants;

namespace NIHR.ProfileManagement.Domain.Services
{
    public class ProfileOutboxProcessor : IOutboxProcessor
    {
        private readonly IProfileOutboxRepository _outboxRepository;
        private readonly IProfileEventMessagePublisher _profileEventMessagePublisher;
        private readonly ILogger<ProfileOutboxProcessor> _logger;

        public ProfileOutboxProcessor(
            IProfileOutboxRepository studyRecordOutboxRepository,
            IProfileEventMessagePublisher studyEventMessagePublisher,
            ILogger<ProfileOutboxProcessor> logger)
        {
            this._outboxRepository = studyRecordOutboxRepository;
            this._profileEventMessagePublisher = studyEventMessagePublisher;
            this._logger = logger;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OutboxProcessor starting ProcessAsync");

            // Read Outbox
            var numberOfOutboxItemsProcessed = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var nextOutboxItemToProcess = _outboxRepository.GetNextUnprocessedOutboxItem();

                if (nextOutboxItemToProcess == null)
                {
                    // Log
                    _logger.LogInformation("No entries found for processing in outbox table.");
                    return;
                }

                _logger.LogInformation($"Processing outbox record with id {nextOutboxItemToProcess.Id}");

                numberOfOutboxItemsProcessed++;

                _logger.LogDebug($"Processing outbox record with id {nextOutboxItemToProcess.Id} - mark as processing");

                // Mark item as "Processing"
                await _outboxRepository.UpdateOutboxItemStatusAsync(
                    nextOutboxItemToProcess.Id,
                    OutboxStatus.Processing,
                    DateTime.Now);

                await _outboxRepository.CommitAsync();

                _logger.LogDebug($"Processing outbox record with id {nextOutboxItemToProcess.Id} - sending to kafka");

                // Publish to Kafka
                await _profileEventMessagePublisher.Publish(nextOutboxItemToProcess.Payload,
                    cancellationToken);

                _logger.LogDebug($"Processing outbox record with id {nextOutboxItemToProcess.Id} - mark as completed");

                // Mark item as completed
                await _outboxRepository.UpdateOutboxItemStatusAsync(
                    nextOutboxItemToProcess.Id,
                    OutboxStatus.CompletedSuccessfully,
                    null,
                    DateTime.Now);

                await _outboxRepository.CommitAsync();
            }

            _logger.LogInformation($"ProcessAsync completed processing {numberOfOutboxItemsProcessed} records.");
        }
    }
}
