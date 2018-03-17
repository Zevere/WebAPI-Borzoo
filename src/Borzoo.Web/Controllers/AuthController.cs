//using System;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using Borzoo.Data.Abstractions;
//using Borzoo.Web.Models.Login;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Formatters;
//using Microsoft.Net.Http.Headers;
//using UserEntity = Borzoo.Data.Abstractions.Entities.User;
//
//namespace Borzoo.Web.Controllers
//{
//    public class AuthController : Controller
//    {
//        private readonly IUserRepository _userRepo;
//
//        public AuthController(IUserRepository userRepo)
//        {
//            _userRepo = userRepo;
//        }
//
//        [Consumes(Constants.ZevereContentTypes.Login.Creation)]
//        [Produces(Constants.ZevereContentTypes.Login.Token)]
//        [ProducesResponseType(typeof(LoginDto), StatusCodes.Status201Created)]
//        [HttpPost(Constants.ZevereRoutes.Login)]
//        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
//        {
//            if (model is null || !TryValidateModel(model))
//            {
//                return StatusCode(StatusCodes.Status400BadRequest);
//            }
//
//            var user = await _userRepo.GetByPassphraseLoginAsync(model.UserName, model.Passphrase);
//
//            bool alreadyHasToken = !string.IsNullOrEmpty(user.Token);
//            string token = alreadyHasToken ? user.Token : GenerateAlphaNumericString(100);
//            await _userRepo.SetTokenForUserAsync(user.Id, token);
//
//            string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
//            var loginDto = new LoginDto(encodedToken);
//
//            return new ObjectResult(loginDto)
//            {
//                StatusCode = alreadyHasToken ? StatusCodes.Status200OK : StatusCodes.Status201Created,
//                ContentTypes = new MediaTypeCollection
//                {
//                    MediaTypeHeaderValue.Parse(Constants.ZevereContentTypes.Login.Token)
//                }
//            };
//        }
//
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [HttpPost(Constants.ZevereRoutes.Logout)]
//        [Authorize]
//        public async Task<IActionResult> Logout()
//        {
//            string token = User.FindFirstValue("token");
//
//            bool isRevoked = await _userRepo.RevokeTokenAsync(token);
//            return isRevoked
//                ? NoContent()
//                : Unauthorized() as IActionResult;
//        }
//
//        private string GenerateAlphaNumericString(int charCount)
//        {
//            var rnd = new Random(DateTime.UtcNow.Millisecond);
//            var chars = Enumerable.Range(0, charCount)
//                .Select(_ =>
//                {
//                    char c = default;
//                    int charType = rnd.Next() % 3;
//                    switch (charType)
//                    {
//                        case 0: // Number
//                            c = (char) rnd.Next(48, 50);
//                            break;
//                        case 1: // Upper-Case Letter
//                            c = (char) rnd.Next(65, 91);
//                            break;
//                        case 2: // Lower-Case Letter
//                            c = (char) rnd.Next(97, 123);
//                            break;
//                    }
//
//                    return c;
//                });
//            return string.Join(string.Empty, chars);
//        }
//    }
//}