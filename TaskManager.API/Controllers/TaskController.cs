using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TaskManager.API.Commands;
using TaskManager.API.Exceptions;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TaskController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        public TaskController(ILogger<TaskController> logger, IMediator mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Add New Task Endpoint
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddTaskCommandModel request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return Ok(response);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update Existing Task Endpoint
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateTaskCommandModel request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return Ok(response);
            }
            catch (TaskNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
