using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class AddTaskCommandHandler : IRequestHandler<AddTaskCommandModel, TaskResponse>
    {
        private readonly ITaskManagerService _taskManagerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskService"></param>
        public AddTaskCommandHandler(ITaskManagerService taskManagerService)
        {
            _taskManagerService = taskManagerService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TaskResponse> Handle(AddTaskCommandModel request, CancellationToken cancellationToken)
        {
            return _taskManagerService.AddNewTaskAsync(request);
        }
    }
}
