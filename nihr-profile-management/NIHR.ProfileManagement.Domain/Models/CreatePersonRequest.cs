
namespace NIHR.ProfileManagement.Domain.Models
{
    public class CreatePersonRequest
    {
    }

    public class CreatePersonResponse
    {

    }

    public class PersonProfileInfo
    {
        public DateTime Created { get; private set; }

        public PersonProfileInfo(DateTime created)
        {
            Created = created;
        }
    }
}
