namespace NIHR.ProfileManagement.Infrastructure.Repository.Models
{
    public class ProfileInfoDbEntity : DbEntity
    {
        public int Id { get; set; }

        public virtual ICollection<ProfileIdentityDbEntity> Identities { get; set; }

        public virtual ICollection<PersonNameDbEntity> Names { get; set; }

        public ProfileInfoDbEntity()
        {
            Identities = new HashSet<ProfileIdentityDbEntity>();
            Names = new HashSet<PersonNameDbEntity>();
        }
    }
}
