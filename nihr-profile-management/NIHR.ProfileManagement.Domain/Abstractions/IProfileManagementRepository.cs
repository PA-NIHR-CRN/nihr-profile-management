using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Domain.Abstractions
{
    public interface IProfileManagementRepository
    {
        Task<CreateProfileResponse> CreateProfileAsync(CreateProfileRequest request);
    }

    public interface IProfileManagementService
    {
        Task<CreateProfileResponse> CreatePersonAsync(CreateProfileRequest request);
    }
}
