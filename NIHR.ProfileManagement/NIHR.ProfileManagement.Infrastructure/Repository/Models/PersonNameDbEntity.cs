namespace NIHR.ProfileManagement.Infrastructure.Repository.Models
{
    public partial class PersonNameDbEntity : DbEntity
    {
        public int Id { get; set; }

        public int ProfileInfoId { get; set; }

        public string Family { get; set; } = null!;

        public string Given { get; set; } = null!;

        public virtual ProfileInfoDbEntity ProfileInfo { get; set; } = null!;
    }
}
