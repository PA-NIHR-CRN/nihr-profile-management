using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Domain.Services
{
    public class ProfileManagementService : IProfileManagementService
    {
        private readonly IProfileManagementRepository _profileManagementRepository;

        public ProfileManagementService(IProfileManagementRepository profileManagementRepository)
        {
            _profileManagementRepository = profileManagementRepository;
        }

        public async Task<CreateProfileResponse> CreatePersonAsync(CreateProfileRequest request)
        {
            return await _profileManagementRepository.CreateProfileAsync(request);
        }
    }
}
