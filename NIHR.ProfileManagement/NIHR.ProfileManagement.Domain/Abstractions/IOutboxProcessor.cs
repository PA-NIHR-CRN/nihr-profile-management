
namespace NIHR.ProfileManagement.Domain.Abstractions
{
    public interface IOutboxProcessor
    {
        Task ProcessAsync(CancellationToken cancellationToken);
    }
}
