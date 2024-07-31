using Microsoft.AspNetCore.Mvc;
using NIHR.ProfileManagement.Api.Documentation;
using NIHR.ProfileManagement.Api.Models.Dto;
using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Models;
using Swashbuckle.AspNetCore.Filters;

namespace NIHR.ProfileManagement.Api.Controllers
{
    [Route("api/profile")]
    public class ProfileController : ApiControllerBase
    {
        private readonly IProfileManagementService _profileManagementService;

        public ProfileController(IProfileManagementService profileManagementService)
        {
            _profileManagementService = profileManagementService;
        }

        [SwaggerRequestExample(typeof(CreateUserRequestDto), typeof(CreateNewProfileRequestExample))]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateUserRequestDto createUserRequestDto, CancellationToken cancellationToken)
        {
            var createPersonRequest = new CreateProfileRequest{
                Firstname = createUserRequestDto.firstname,
                Lastname = createUserRequestDto.lastname,
                sub = createUserRequestDto.sub
            };

            var result = await _profileManagementService.CreatePersonAsync(createPersonRequest, cancellationToken);

            return Ok(result);
        }
    }
}