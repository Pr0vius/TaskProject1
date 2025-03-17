using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ms.task.application.Commands;
using ms.task.application.Queries;
using ms.task.application.Requests;
using System.Security.Claims;
using ms.task.api.Helpers;
using System.Net;
using ms.task.api.Responses;
using ms.task.domain.Entities;

namespace ms.task.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {

                var userId = GetUserId();
                var data = await _mediator.Send(new GetAllTasksQuery(userId));
                return ApiResponse<List<UserTask>>.Success(data, "Tasks list", HttpStatusCode.OK);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException)
            {
                throw new ExceptionResponse(ex.Message, HttpStatusCode.Unauthorized);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest taskRequest)
        {
            try
            {
                var userId = GetUserId();
                var data = await _mediator.Send(new CreateTaskCommand(userId, taskRequest.Name, taskRequest.Description));
                return ApiResponse<UserTask>.Success(data, "Task created", HttpStatusCode.Created);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException) { throw new ExceptionResponse(ex.Message, HttpStatusCode.Unauthorized); }
        }

        [HttpGet("{TaskId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid TaskId)
        {
            try
            {
                var userId = GetUserId();
                var data = await _mediator.Send(new GetTaskByIdQuery(userId, TaskId));
                return ApiResponse<UserTask>.Success(data, $"Task with id {TaskId}", HttpStatusCode.OK);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException) { throw new ExceptionResponse(ex.Message, HttpStatusCode.Unauthorized); }
            catch (Exception ex) when (ex is KeyNotFoundException) { throw new ExceptionResponse(ex.Message, HttpStatusCode.BadRequest); }
        }


        [HttpPut("{TaskId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask(Guid TaskId)
        {
            try
            {
                var userId = GetUserId();
                var data = await _mediator.Send(new ChangeTaskStateCommand(userId, TaskId));
                return ApiResponse<UserTask>.Success(data, "Task updated", HttpStatusCode.OK);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException) { throw new ExceptionResponse(ex.Message, HttpStatusCode.Unauthorized); }
            catch (Exception ex) when (ex is KeyNotFoundException) { throw new ExceptionResponse(ex.Message, HttpStatusCode.BadRequest); }

        }

        [HttpDelete("{TaskId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(Guid TaskId)
        {
            try
            {
                var userId = GetUserId();
                var data = await _mediator.Send(new DeleteTaskCommand(userId, TaskId));
                return ApiResponse<object?>.Success(null, "Task deleted", HttpStatusCode.OK);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException) { throw new ExceptionResponse(ex.Message, HttpStatusCode.Unauthorized); }
            catch (Exception ex) when (ex is KeyNotFoundException) { throw new ExceptionResponse(ex.Message, HttpStatusCode.BadRequest); }

        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");


            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario.");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("El ID del usuario no es un GUID válido.");
            }

            return userId;
        }
    }
}
