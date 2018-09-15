//using System;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using Borzoo.Data.Abstractions;
//using Borzoo.Web.Models;
//using Borzoo.Web.Models.User;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Primitives;
//using Newtonsoft.Json.Linq;
//using UserEntity = Borzoo.Data.Abstractions.Entities.User;
//
//namespace Borzoo.Web.Controllers
//{
//    [Route("/zv/[controller]")]
//    public class UsersController : Controller
//    {
//        private readonly IUserRepository _userRepo;
//
//        public UsersController(IUserRepository userRepo)
//        {
//            _userRepo = userRepo;
//        }
//
//        [HttpHead("{userName}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> Head(string userName)
//        {
//            IActionResult result;
//            if (string.IsNullOrWhiteSpace(userName))
//            {
//                result = BadRequest(); // ToDo use an error response generator helper class 
//            }
//            else
//            {
//                try
//                {
//                    await _userRepo.GetByNameAsync(userName);
//                    result = NoContent();
//                }
//                catch (EntityNotFoundException)
//                {
//                    result = NotFound(); // ToDo use error class
//                }
//            }
//
//            return result;
//        }
//
//        [Produces(
//            Constants.ZevereContentTypes.User.Full,
//            Constants.ZevereContentTypes.User.Pretty
//        )]
//        [HttpGet("{userName}")]
//        [Authorize]
//        public async Task<IActionResult> Get(string userName)
//        {
//            if (string.IsNullOrWhiteSpace(userName))
//                return BadRequest(); // ToDo use an error response generator helper class
//            if (!User.Identity.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
//                return Forbid();
//
//            IActionResult result = default;
//            UserEntity user;
//            try
//            {
//                user = await _userRepo.GetByNameAsync(userName);
//            }
//            catch (EntityNotFoundException)
//            {
//                user = default;
//                result = NotFound(); // ToDo use error class
//            }
//
//            if (user != null)
//            {
//                UserDtoBase dto;
//                string acceptType = Request.Headers["accept"].SingleOrDefault();
//                switch (acceptType)
//                {
//                    case Constants.ZevereContentTypes.User.Full:
//                        dto = (UserFullDto) user;
//                        break;
//                    case Constants.ZevereContentTypes.User.Pretty:
//                        dto = (UserPrettyDto) user;
//                        break;
//                    default:
//                        throw new InvalidOperationException("Invalid Accept type");
//                }
//
//                result = StatusCode((int) HttpStatusCode.OK, dto);
//            }
//
//            return result;
//        }
//
//        [HttpPost]
//        [Consumes(Constants.ZevereContentTypes.User.Creation)]
//        [ProducesResponseType(typeof(UserFullDto), StatusCodes.Status201Created)]
//        [ProducesResponseType(typeof(UserPrettyDto), StatusCodes.Status201Created)]
//        [ProducesResponseType(typeof(EmptyContentDto), StatusCodes.Status204NoContent)]
//        public async Task<IActionResult> Post([FromBody] UserCreationDto model)
//        {
//            if (model is null || !TryValidateModel(model))
//            {
//                return StatusCode((int) HttpStatusCode.BadRequest);
//            }
//
//            var user = (UserEntity) model;
//
//            await _userRepo.AddAsync(user);
//
//            string contentType = HttpContext.Request.Headers["Accept"].SingleOrDefault()?.ToLowerInvariant();
//            switch (contentType)
//            {
//                case Constants.ZevereContentTypes.Empty:
//                    return NoContent();
//                case Constants.ZevereContentTypes.User.Pretty:
//                    return Created($"{user.DisplayId}", (UserPrettyDto) user);
//                case Constants.ZevereContentTypes.User.Full:
//                    return Created($"{user.DisplayId}", (UserFullDto) user);
//                default:
//                    return BadRequest();
//            }
//        }
//
//        [HttpPatch("{userName}")]
//        [Authorize]
//        [Consumes("application/json")]
//        [Produces(
//            Constants.ZevereContentTypes.Empty,
//            Constants.ZevereContentTypes.User.Pretty,
//            Constants.ZevereContentTypes.User.Full
//        )]
//        public async Task<IActionResult> Patch([FromRoute] string userName, [FromBody] JObject patch)
//        {
//            if (string.IsNullOrWhiteSpace(userName))
//                return BadRequest(); // ToDo use an error response generator helper class
//            if (!User.Identity.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
//                return Forbid();
//            if (patch is default)
//                return BadRequest();
//
//            string[] properties = {"first_name", "last_name"};
//            if (properties.All(p => patch[p]?.Value<string>() is default))
//                return BadRequest();
//            string fName = patch["first_name"]?.Value<string>();
//            string lName = patch["last_name"]?.Value<string>();
//
//            if (new[] {fName, lName}.All(string.IsNullOrWhiteSpace))
//                return BadRequest();
//
//            var user = await _userRepo.GetByNameAsync(userName);
//            user.FirstName = string.IsNullOrWhiteSpace(fName) ? user.FirstName : fName;
//            user.LastName = string.IsNullOrWhiteSpace(lName) ? user.LastName : lName;
//            await _userRepo.UpdateAsync(user);
//
//            string contentType = HttpContext.Request.Headers["Accept"].SingleOrDefault()?.ToLowerInvariant();
//            switch (contentType)
//            {
//                case Constants.ZevereContentTypes.Empty:
//                    return NoContent();
//                case Constants.ZevereContentTypes.User.Pretty:
//                    return Accepted((UserPrettyDto) user);
//                case Constants.ZevereContentTypes.User.Full:
//                    return Accepted((UserFullDto) user);
//                default:
//                    return BadRequest();
//            }
//        }
//
//        [HttpDelete("{userName}")]
//        [Authorize]
//        [ProducesResponseType((int) HttpStatusCode.NoContent)]
//        public async Task<IActionResult> Delete(string userName)
//        {
//            if (string.IsNullOrWhiteSpace(userName))
//                return BadRequest(); // ToDo use an error response generator helper class
//            if (!User.Identity.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
//                return Forbid();
//
//            var user = await _userRepo.GetByNameAsync(userName);
//            await _userRepo.DeleteAsync(user.Id);
//            return NoContent();
//        }
//    }
//}