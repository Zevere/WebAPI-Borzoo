using System.Net;
using Borzoo.Models;
using Borzoo.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Borzoo.Controllers
{
    [Route("/zv/[controller]")]
    public class UsersController : Controller
    {
        [HttpGet("{userId}")]
        public IActionResult Get(string userId)
        {
            return NoContent();
        }

        [HttpPost]
        [Consumes(Constants.ZVeerContentTypes.User.Creation)]
        [ProducesResponseType(typeof(UserFullDto), (int) HttpStatusCode.Created)]
//        [ProducesResponseType(typeof(UserPrettyDto), (int) HttpStatusCode.Created)] // ToDo
        [ProducesResponseType(typeof(EmptyContentDto), (int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Error), (int) HttpStatusCode.Conflict)]
        public IActionResult Post([FromBody] UserCreationRequest model)
        {
            if (model is null || !TryValidateModel(model))
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }

            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpPatch("{userId}")]
        public IActionResult Patch(string userId)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpDelete("{userId}")]
        public IActionResult Delete(string userId)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }
    }
}