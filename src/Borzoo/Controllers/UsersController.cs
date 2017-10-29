using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Borzoo.Controllers
{
    [Route("/zv/[controller]/{userId}")]
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Get(string userId)
        {
            return NoContent();
        }

        [HttpPost]
        public IActionResult Post()
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        [HttpPatch]
        public IActionResult Patch(string userId)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        [HttpDelete]
        public IActionResult Delete(string userId)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }
    }
}
