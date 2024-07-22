using NIHR.ProfileManagement.Domain.EnumsAndConstants;
using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Domain.Abstractions
{
    public interface IProfileOutboxRepository
    {
        Task AddToOutboxAsync(AddToOuxboxRequest request, CancellationToken cancellationToken);

        OutboxItem? GetNextUnprocessedOutboxItem();

        Task UpdateOutboxItemStatusAsync(int id,
            OutboxStatus status,
            DateTime? processingStartedTime = null,
            DateTime? processingCompletedTime = null);

        Task CommitAsync();
    }
}
