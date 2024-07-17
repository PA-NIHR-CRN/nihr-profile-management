using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Infrastructure.Repository
{
    public class ProfileManagementRepository : IProfileManagementRepository
    {
        public Task<CreatePersonResponse> CreatePersonAsync(CreatePersonRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
