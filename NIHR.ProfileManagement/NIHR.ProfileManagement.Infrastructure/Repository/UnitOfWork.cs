using NIHR.ProfileManagement.Domain.Abstractions;
namespace NIHR.ProfileManagement.Infrastructure.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly ProfileManagementDbContext _context;

        public IProfileManagementRepository ProfileManagementRepository { get; private set; }

        public IProfileOutboxRepository ProfileOutboxRepository { get; private set; }

        public UnitOfWork(ProfileManagementDbContext dbContext, INsipMessageHelper nsipGrisMessageHelper)
        {
            _context = dbContext;
            ProfileManagementRepository = new ProfileManagementRepository(dbContext);
            ProfileOutboxRepository = new ProfileOutboxRepository(dbContext, nsipGrisMessageHelper);
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
