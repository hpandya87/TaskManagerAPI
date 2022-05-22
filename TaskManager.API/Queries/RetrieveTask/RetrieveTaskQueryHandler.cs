using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.API.Services;
using TaskManager.Domain.CoreModels;

namespace TaskManager.API.Queries
{
    public class RetrieveTaskQueryHandler : IRequestHandler<RetrieveTaskQueryModel, TaskData>
    {
        private readonly ITaskManagerService _taskManagerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskService"></param>
        public RetrieveTaskQueryHandler(ITaskManagerService taskManagerService)
        {
            _taskManagerService = taskManagerService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TaskData> Handle(RetrieveTaskQueryModel request, CancellationToken cancellationToken)
        {
            return _taskManagerService.GetTaskDetailsById(request.Id);
        }
    }
}
