using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Web.Models;
using Borzoo.Web.Models.User;
using Microsoft.AspNetCore.Authorization;
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

        [HttpHead("{userName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Head(string userName)
        {
            IActionResult result;
            if (string.IsNullOrWhiteSpace(userName))
            {
                result = BadRequest(); // ToDo use an error response generator helper class 
            }
            else
            {
                try
                {
                    await _userRepo.GetByNameAsync(userName);
                    result = NoContent();
                }
                catch (EntityNotFoundException)
                {
                    result = NotFound(); // ToDo use error class
                }
            }

            return result;
        }

        [Produces(
            Constants.ZevereContentTypes.User.Full,
            Constants.ZevereContentTypes.User.Pretty // ToDo
        )]
        [HttpGet("{userName}")]
        [Authorize]
        public async Task<IActionResult> Get(string userName)
        {
            // ToDo check accept headers
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest(); // ToDo use an error response generator helper class
            if (!User.Identity.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            IActionResult result = default;
            UserEntity user;
            try
            {
                user = await _userRepo.GetByNameAsync(userName);
            }
            catch (EntityNotFoundException)
            {
                user = default;
                result = NotFound(); // ToDo use error class
            }

            if (user != null)
            {
                UserDtoBase dto;
                string acceptType = Request.Headers["accept"].SingleOrDefault();
                switch (acceptType)
                {
                    case Constants.ZevereContentTypes.User.Full:
                        dto = (UserFullDto) user;
                        break;
                    case Constants.ZevereContentTypes.User.Pretty:
                        dto = (UserPrettyDto) user;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Accept type");
                }

                result = StatusCode((int) HttpStatusCode.OK, dto);
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