//using System;
//using System.Linq;
//using System.Net;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Borzoo.Data.Abstractions;
//using Borzoo.Data.Abstractions.Entities;
//using Borzoo.Web.Models;
//using Borzoo.Web.Models.Task;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//
//namespace Borzoo.Web.Controllers
//{
//    [Route("/zv/users/{userName}/[controller]")]
//    [Authorize]
//    public class TasksController : Controller
//    {
//        private readonly ITaskItemRepository _taskItemRepo;
//
//        public TasksController(ITaskItemRepository taskItemRepo)
//        {
//            _taskItemRepo = taskItemRepo;
//        }
//
//        [HttpPost]
//        [Consumes(Constants.ZevereContentTypes.Task.Creation)]
//        [ProducesResponseType(typeof(TaskPrettyDto), StatusCodes.Status201Created)]
//        [ProducesResponseType(typeof(TaskFullDto), StatusCodes.Status201Created)]
//        [ProducesResponseType(typeof(EmptyContentDto), StatusCodes.Status204NoContent)]
//        public async Task<IActionResult> Post([FromRoute] string userName, [FromBody] TaskCreationDto dto)
//        {
//            var result = EnsureAuthorizedUser(userName);
//            if (result != null)
//                return result;
//            if (dto is default || !TryValidateModel(dto))
//                return StatusCode((int) HttpStatusCode.BadRequest);
//            if (dto.DueBy != default)
//            {
//                dto.DueBy = dto.DueBy.ToUniversalTime();
//                result = ValidateDueDate(dto.DueBy);
//                if (result != null)
//                    return result;
//            }
//
//            if (dto.Id != default)
//            {
//                try
//                {
//                    await _taskItemRepo.GetByNameAsync(dto.Id);
//                    return StatusCode(StatusCodes.Status409Conflict);
//                }
//                catch (EntityNotFoundException)
//                {
//                }
//            }
//
//            var task = (TaskItem) dto;
//
//            await _taskItemRepo.AddAsync(task);
//
//            string contentType = HttpContext.Request.Headers["Accept"].SingleOrDefault()?.ToLowerInvariant();
//            switch (contentType)
//            {
//                case Constants.ZevereContentTypes.Empty:
//                    return NoContent();
//                case Constants.ZevereContentTypes.Task.Pretty:
//                    return Created($"{task.Id}", (TaskPrettyDto) task);
//                case Constants.ZevereContentTypes.Task.Full:
//                    return Created($"{task.Id}", (TaskFullDto) task);
//                default:
//                    return BadRequest();
//            }
//        }
//
//        [HttpGet]
//        [ProducesResponseType(typeof(TaskPrettyDto), StatusCodes.Status200OK)]
//        [ProducesResponseType(typeof(TaskFullDto), StatusCodes.Status200OK)]
//        public async Task<IActionResult> GetAllTasks([FromRoute] string userName)
//        {
//            var result = EnsureAuthorizedUser(userName);
//            if (result != null)
//                return result;
//
//            var tasks = await _taskItemRepo.GetTaskListItemsAsync();
//
//            string contentType = HttpContext.Request.Headers["Accept"].SingleOrDefault()?.ToLowerInvariant();
//            switch (contentType)
//            {
//                case Constants.ZevereContentTypes.Task.Pretty:
//                    return Ok(tasks.Select(t => (TaskPrettyDto) t));
//                case Constants.ZevereContentTypes.Task.Full:
//                    return Ok(tasks.Select(t => (TaskFullDto) t));
//                default:
//                    return BadRequest();
//            }
//        }
//
//        [HttpGet("{taskName}")]
//        [ProducesResponseType(typeof(TaskPrettyDto), StatusCodes.Status200OK)]
//        [ProducesResponseType(typeof(TaskFullDto), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> GetTask([FromRoute] string userName, [FromRoute] string taskName)
//        {
//            IActionResult result = EnsureAuthorizedUser(userName);
//            if (result != null)
//                return result;
//
//            if (!Regex.IsMatch(taskName, Constants.Regexes.TaskId))
//                return NotFound();
//
//            TaskItem taskItem;
//            try
//            {
//                taskItem = await _taskItemRepo.GetByNameAsync(taskName);
//            }
//            catch (EntityNotFoundException)
//            {
//                return NotFound();
//            }
//
//            string contentType = HttpContext.Request.Headers["Accept"].SingleOrDefault()?.ToLowerInvariant();
//            switch (contentType)
//            {
//                case Constants.ZevereContentTypes.Task.Pretty:
//                    return Ok((TaskPrettyDto) taskItem);
//                case Constants.ZevereContentTypes.Task.Full:
//                    return Ok((TaskFullDto) taskItem);
//                default:
//                    return BadRequest();
//            }
//        }
//
//        [HttpHead("{taskName}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> Head([FromRoute] string userName, [FromRoute] string taskName)
//        {
//            IActionResult result = EnsureAuthorizedUser(userName);
//            if (result != null)
//                return result;
//
//            if (!Regex.IsMatch(taskName, Constants.Regexes.TaskId))
//                return NotFound();
//
//            try
//            {
//                await _taskItemRepo.GetByNameAsync(taskName);
//                result = NoContent();
//            }
//            catch (EntityNotFoundException)
//            {
//                result = NotFound(); // ToDo use error class
//            }
//
//            return result;
//        }
//
//        private IActionResult ValidateDueDate(DateTime due)
//        {
//            var now = DateTime.UtcNow;
//            if (!(now < due && (due - now).TotalSeconds > 60))
//            {
//                return BadRequest();
//            }
//
//            return default;
//        }
//
//        private IActionResult EnsureAuthorizedUser(string userName)
//        {
//            if (string.IsNullOrWhiteSpace(userName))
//                return BadRequest(); // ToDo use an error response generator helper class
//            if (!User.Identity.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
//                return Forbid();
//
////            _taskItemRepo.TaskListName = userName;
//            return default;
//        }
//    }
//}