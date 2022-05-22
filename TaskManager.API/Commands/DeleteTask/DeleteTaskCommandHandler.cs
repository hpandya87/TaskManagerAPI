using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Commands
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommandModel, TaskResponse>
    {
        private readonly ITaskManagerService _taskManagerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskService"></param>
        public DeleteTaskCommandHandler(ITaskManagerService taskManagerService)
        {
            _taskManagerService = taskManagerService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TaskResponse> Handle(DeleteTaskCommandModel request, CancellationToken cancellationToken)
        {
            return _taskManagerService.DeleteTaskByIdAsync(request.Id);
        }
    }
}
