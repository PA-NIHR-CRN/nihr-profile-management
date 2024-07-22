using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.EnumsAndConstants;
using NIHR.ProfileManagement.Domain.Models;
using System.Threading;

namespace NIHR.ProfileManagement.Domain.Services
{
    public class ProfileManagementService : IProfileManagementService
    {
        private readonly IProfileManagementRepository _profileManagementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _profileManagementRepository = _unitOfWork.ProfileManagementRepository;
        }

        public async Task<CreateProfileResponse> CreatePersonAsync(CreateProfileRequest request,
            CancellationToken cancellationToken)
        {
            var createProfileResponse = await _profileManagementRepository.CreateProfileAsync(request);

            await _unitOfWork.ProfileOutboxRepository.AddToOutboxAsync(new AddToOuxboxRequest
            {
                Payload = System.Text.Json.JsonSerializer.Serialize(request),
                EventType = NsipEventTypes.ProfileCreated,
                SourceSystem = request.ApiSystemName ?? "test"
            },
            cancellationToken);

            await _unitOfWork.CommitAsync();

            return createProfileResponse;
        }
    }
}
