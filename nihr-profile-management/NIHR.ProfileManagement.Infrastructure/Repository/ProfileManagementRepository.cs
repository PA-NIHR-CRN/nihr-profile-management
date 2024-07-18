using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Models;
using NIHR.ProfileManagement.Infrastructure.Repository.Models;

namespace NIHR.ProfileManagement.Infrastructure.Repository
{
    public class ProfileManagementRepository : IProfileManagementRepository
    {
        private readonly ProfileManagementDbContext _context;

        public ProfileManagementRepository(ProfileManagementDbContext context)
        {
            _context = context;
        }

        public async Task<CreateProfileResponse> CreateProfileAsync(CreateProfileRequest request)
        {
            var profileInfoDbEntity = new ProfileInfoDbEntity();

            var profileIdentifier = new ProfileIdentity
            {
                Sub = request.sub
            };

            profileInfoDbEntity.Identities.Add(profileIdentifier);

            var personNameDbEntity = new PersonNameDbEntity
            {
                Family = request.lastname,
                Given = request.firstname,
                ProfileInfo = profileInfoDbEntity
            };

            profileInfoDbEntity.Names.Add(personNameDbEntity);
            profileInfoDbEntity.Identities.Add(profileIdentifier);

            _context.Profiles.Add(profileInfoDbEntity);

            await _context.SaveChangesAsync();

            return new CreateProfileResponse() {
                Profile = new ProfileInfo(profileInfoDbEntity.Created,
                profileInfoDbEntity.Names.First().Given,
                profileInfoDbEntity.Names.First().Family,
                profileInfoDbEntity.Identities.First().Sub)
            };
        }
    }
}
