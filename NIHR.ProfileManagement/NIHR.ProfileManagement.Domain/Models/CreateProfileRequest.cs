
namespace NIHR.ProfileManagement.Domain.Models
{
    public class CreateProfileRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string sub { get; set; }

        public string ApiSystemName { get; set; }
    }
}
