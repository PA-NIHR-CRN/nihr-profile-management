namespace NIHR.ProfileManagement.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        public IProfileManagementRepository ProfileManagementRepository { get; }

        public IProfileOutboxRepository ProfileOutboxRepository { get; }

        Task CommitAsync();

        void Dispose();
    }
}
