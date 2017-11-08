using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Models;
using Borzoo.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Controllers
{
    [Route("/zv/[controller]")]
    public class UsersController : Controller
    {
        private readonly IEntityRepository<UserEntity> _userRepo;

        public UsersController(IEntityRepository<UserEntity> userRepo)
        {
            _userRepo = userRepo;
        }

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
        public async Task<IActionResult> Post([FromBody] UserCreationRequest model)
        {
            if (model is null || !TryValidateModel(model))
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }

            var entity = (UserEntity) model;

            await _userRepo.AddAsync(entity);

            bool hasAcceptHeader = HttpContext.Request.Headers.TryGetValue("ACCEPT", out StringValues vals);
            if (hasAcceptHeader && vals.First().Equals(Constants.ZVeerContentTypes.User.Full))
            {
                return StatusCode((int) HttpStatusCode.Created, (UserFullDto) entity);
            }
            else
            {
                return StatusCode((int) HttpStatusCode.NotImplemented);
            }
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