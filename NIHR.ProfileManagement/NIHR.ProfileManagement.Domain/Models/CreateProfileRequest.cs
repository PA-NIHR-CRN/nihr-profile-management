
namespace NIHR.ProfileManagement.Domain.Models
{
    public class CreateProfileRequest
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string sub { get; set; }

        public string ApiSystemName { get; set; }
    }

    public class CreateProfileResponse
    {
        public ProfileInfo? Profile { get; set; }
    }

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
