
namespace NIHR.ProfileManagement.Domain.Abstractions
{
    public interface IProfileEventMessagePublisher
    {
        Task PrepareAndPublishAsync(string eventType,
            string sourceSystemName,
            string payload,
            CancellationToken cancellationToken);

        Task Publish(string payload, CancellationToken cancellationToken);
    }
}
