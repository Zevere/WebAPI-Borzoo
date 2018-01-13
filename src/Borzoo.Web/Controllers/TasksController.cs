using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Web.Models;
using Borzoo.Web.Models.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Borzoo.Web.Controllers
{
    [Route("/zv/users/{userName}/[controller]")]
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskRepository _taskRepo;

        public TasksController(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        [HttpPost]
        [Consumes(Constants.ZevereContentTypes.Task.Creation)]
        [ProducesResponseType(typeof(EmptyContentDto), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Post([FromRoute] string userName, [FromBody] TaskCreationDto dto)
        {
            var result = EnsureSameAuthorizedUser(userName);
            if (result != null)
                return result;
            if (dto is default || !TryValidateModel(dto))
                return StatusCode((int) HttpStatusCode.BadRequest);
            if (dto.DueBy != default)
            {
                dto.DueBy = dto.DueBy.ToUniversalTime();
                result = ValidateDueDate(dto.DueBy);
                if (result != null)
                    return result;
            }

            var task = (UserTask) dto;

            _taskRepo.UserName = userName;
            await _taskRepo.AddAsync(task);

            string contentType = HttpContext.Request.Headers["Accept"].SingleOrDefault()?.ToLowerInvariant();
            switch (contentType)
            {
                case Constants.ZevereContentTypes.Empty:
                    return NoContent();
                case Constants.ZevereContentTypes.Task.Pretty:
                    return Created($"{task.Id}", (TaskPrettyDto) task);
                case Constants.ZevereContentTypes.Task.Full:
                    return Created($"{task.Id}", (TaskFullDto) task);
                default:
                    return BadRequest();
            }
        }

        private IActionResult ValidateDueDate(DateTime due)
        {
            var now = DateTime.UtcNow;
            if (!(now < due && (due - now).TotalSeconds > 60))
            {
                return BadRequest();
            }

            return default;
        }

        private IActionResult EnsureSameAuthorizedUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest(); // ToDo use an error response generator helper class
            if (!User.Identity.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
                return Forbid();
            return default;
        }
    }
}