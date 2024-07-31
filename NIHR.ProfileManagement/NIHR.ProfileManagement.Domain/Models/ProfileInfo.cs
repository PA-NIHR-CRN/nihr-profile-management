
namespace NIHR.ProfileManagement.Domain.Models
{
    public class ProfileInfo
    {
        public DateTime Created { get; private set; }

        public string Firstname { get; private set; }

        public string Lastname { get; private set; }

        public string PrimarySub { get; private set; }

        public ProfileInfo(DateTime created,
            string firstname,
            string lastname,
            string primarySub)
        {
            Created = created;
            Firstname = firstname;
            Lastname = lastname;
            PrimarySub = primarySub;
        }
    }
}
