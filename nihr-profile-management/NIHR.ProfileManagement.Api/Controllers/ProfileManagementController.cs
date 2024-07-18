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

        [ProducesResponseType(typeof(CreateUserResponseDto), StatusCodes.Status201Created)]
        [SwaggerRequestExample(typeof(CreateUserRequestDto), typeof(CreateNewProfileRequestExample))]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateUserRequestDto createUserRequestDto)
        {
            var createPersonRequest = new CreateProfileRequest{
                firstname = createUserRequestDto.firstname,
                lastname = createUserRequestDto.lastname,
                sub = createUserRequestDto.sub
            };

            var result = await _profileManagementService.CreatePersonAsync(createPersonRequest);

            return Ok(result);
        }
    }
}