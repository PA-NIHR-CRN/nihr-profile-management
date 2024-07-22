using Microsoft.AspNetCore.Mvc;
using NIHR.ProfileManagement.Api.EnumsAndConstants;

namespace NIHR.ProfileManagement.Api.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {

            return Ok();
        }

        protected string ApiConsumerSystemName
        {
            get
            {
                foreach (var userClaim in this.User.Claims)
                {
                    if (userClaim.Type == TokenClaimNames.ApiSystemName)
                    {
                        return userClaim.Value;
                    }
                }

                return "";
            }
        }
    }
}