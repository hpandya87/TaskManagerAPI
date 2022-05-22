using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Commands
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommandModel, TaskResponse>
    {
        private readonly ITaskManagerService _taskManagerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskService"></param>
        public UpdateTaskCommandHandler(ITaskManagerService taskManagerService)
        {
            _taskManagerService = taskManagerService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TaskResponse> Handle(UpdateTaskCommandModel request, CancellationToken cancellationToken)
        {
            return _taskManagerService.UpdateTaskAsync(request);
        }
    }
}
