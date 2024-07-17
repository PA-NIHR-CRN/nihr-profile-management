using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Domain.Services
{
    public class ProfileManagementService : IProfileManagementService
    {
        public Task<CreatePersonResponse> CreatePersonAsync(CreatePersonRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
