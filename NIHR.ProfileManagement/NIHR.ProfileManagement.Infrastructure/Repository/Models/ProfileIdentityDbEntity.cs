namespace NIHR.ProfileManagement.Infrastructure.Repository.Models
{
    public class ProfileIdentityDbEntity : DbEntity
    {
        public int Id { get; set; }

        public string Sub { get; set; }

        public int ProfileInfoId { get; set; }

        public virtual ProfileInfoDbEntity ProfileInfo { get; set; } = null!;

        public ProfileIdentityDbEntity()
        {
            Sub = "";
        }
    }
}
