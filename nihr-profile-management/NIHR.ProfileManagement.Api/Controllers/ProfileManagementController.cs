using Microsoft.AspNetCore.Mvc;
using NIHR.ProfileManagement.Api.Models.Dto;
using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Api.Controllers
{
    public class ProfileController : ApiControllerBase
    {
        private readonly IProfileManagementService _profileManagementService;

        public ProfileController(IProfileManagementService profileManagementService)
        {
            _profileManagementService = profileManagementService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateUserRequestDto createUserRequestDto)
        {
            var createPersonRequest = new CreatePersonRequest();

            var result = await _profileManagementService.CreatePersonAsync(createPersonRequest);

            return Ok();
        }
    }
}