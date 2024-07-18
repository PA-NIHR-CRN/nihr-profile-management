using NIHR.ProfileManagement.Api.Models.Dto;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Text;

namespace NIHR.ProfileManagement.Api.Documentation
{
    public class CreateNewProfileRequestExample : IExamplesProvider<CreateUserRequestDto>
    {
        public CreateUserRequestDto GetExamples()
        {
            return new CreateUserRequestDto
            {
                sub = Guid.NewGuid().ToString(),
                firstname = "James",
                lastname = "007"
            };
        }
    }
}
