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

			// "sub" is the only mandatory field we can expect. Everything else is optional
            if(!string.IsNullOrEmpty(request.Firstname)
                && !string.IsNullOrEmpty(request.Lastname))
            {
                var personNameDbEntity = new PersonNameDbEntity
                {
                    Family = request.Lastname,
                    Given = request.Firstname,
                    ProfileInfo = profileInfoDbEntity
                };

                profileInfoDbEntity.Names.Add(personNameDbEntity);
            }

            profileInfoDbEntity.Identities.Add(profileIdentifier);

            await _context.Profiles.AddAsync(profileInfoDbEntity);

            //await _context.SaveChangesAsync();

            return new CreateProfileResponse() {
                Profile = new ProfileInfo(profileInfoDbEntity.Created,
                profileInfoDbEntity.Names.FirstOrDefault()?.Given ?? "",
                profileInfoDbEntity.Names.FirstOrDefault()?.Family ?? "",
                profileInfoDbEntity.Identities.First().Sub)
            };
        }
    }
}
