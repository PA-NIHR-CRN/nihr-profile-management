using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Domain.Abstractions
{
    public interface IProfileManagementRepository
    {
        Task<CreatePersonResponse> CreatePersonAsync(CreatePersonRequest request);
    }

    public interface IProfileManagementService
    {
        Task<CreatePersonResponse> CreatePersonAsync(CreatePersonRequest request);
    }
}
