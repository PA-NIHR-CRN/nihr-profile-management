
namespace NIHR.ProfileManagement.Infrastructure.Repository.Models
{
    public abstract class DbEntity
    {
        public DateTime Created { get; set; } = DateTime.Now;
    }

    public class ProfileInfoDbEntity : DbEntity
    {
        public int Id { get; set; }

        public virtual ICollection<ProfileIdentity> Identities { get; set; }

        public virtual ICollection<PersonNameDbEntity> Names { get; set; }

        public ProfileInfoDbEntity()
        {
            Identities = new HashSet<ProfileIdentity>();
            Names = new HashSet<PersonNameDbEntity>();
        }
    }

    public partial class PersonNameDbEntity : DbEntity
    {
        public int Id { get; set; }

        public int ProfileInfoId { get; set; }

        public string Family { get; set; } = null!;

        public string Given { get; set; } = null!;

        public virtual ProfileInfoDbEntity ProfileInfo { get; set; } = null!;
    }

    public class ProfileIdentity : DbEntity
    {
        public int Id { get; set; }

        public string Sub { get; set; }

        public int ProfileInfoId { get; set; }

        public virtual ProfileInfoDbEntity ProfileInfo { get; set; } = null!;

        public ProfileIdentity()
        {
            Sub = "";
        }
    }
}
