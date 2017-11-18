using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Web.Models;
using Borzoo.Web.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Controllers
{
    [Route("/zv/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [Consumes(
            Constants.ZevereContentTypes.User.Full
//            Constants.ZVeerContentTypes.User.Pretty // ToDo
        )]
        [HttpGet("{userName}")]
        public async Task<IActionResult> Get(string userName)
        {
            // ToDo check accept headers
            IActionResult result = StatusCode((int) HttpStatusCode.NotImplemented);
            if (string.IsNullOrWhiteSpace(userName))
            {
                result = BadRequest(); // ToDo use an error response generator helper class 
            }
            else
            {
                UserEntity user = null;
                try
                {
                    user = await _userRepo.GetByNameAsync(userName);
                }
                catch (EntityNotFoundException)
                {
                    result = NotFound(); // ToDo use error class
                }

                if (user != null)
                {
                    var dto = (UserFullDto) user;
                    result = StatusCode((int) HttpStatusCode.OK, dto);
                }
            }
            return result;
        }

        [HttpPost]
        [Consumes(Constants.ZevereContentTypes.User.Creation)]
        [ProducesResponseType(typeof(UserFullDto), StatusCodes.Status201Created)]
//        [ProducesResponseType(typeof(UserPrettyDto), (int) HttpStatusCode.Created)] // ToDo
        [ProducesResponseType(typeof(EmptyContentDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] UserCreationRequest model)
        {
            if (model is null || !TryValidateModel(model))
            {
                return StatusCode((int) HttpStatusCode.BadRequest);
            }

            var entity = (UserEntity) model;

            await _userRepo.AddAsync(entity);

            bool hasAcceptHeader = HttpContext.Request.Headers.TryGetValue("ACCEPT", out StringValues vals);
            if (hasAcceptHeader && vals.First().Equals(Constants.ZevereContentTypes.User.Full))
            {
                return StatusCode((int) HttpStatusCode.Created, (UserFullDto) entity);
            }
            else
            {
                return StatusCode((int) HttpStatusCode.NotImplemented);
            }
        }

        [HttpPatch("{userName}")]
        public IActionResult Patch(string userName)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpDelete("{userName}")]
        public IActionResult Delete(string userName)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }
    }
}