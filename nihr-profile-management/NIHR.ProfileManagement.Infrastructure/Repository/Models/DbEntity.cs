
namespace NIHR.ProfileManagement.Infrastructure.Repository.Models
{
    public abstract class DbEntity
    {
        public DateTime Created { get; set; } = DateTime.Now;
    }

    public class PersonEntity : DbEntity
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public PersonEntity()
        {
            Firstname = "";
            Lastname = "";
        }
    }

    public class PersonIdentityIdentifierEntity : DbEntity
    {
        public int Id { get; set; }

        public string Sub { get; set; }

        public PersonIdentityIdentifierEntity()
        {
            Sub = "";
        }
    }
}
