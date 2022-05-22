using System.Threading.Tasks;
using TaskManager.API.Commands;
using TaskManager.API.DTOs;
using TaskManager.Domain.CoreModels;

namespace TaskManager.API.Services
{
    public interface ITaskManagerService
    {
        Task<TaskData> GetTaskDetailsById(string id);
        Task<TaskResponse> AddNewTaskAsync(AddTaskCommandModel model);
        Task<TaskResponse> UpdateTaskAsync(UpdateTaskCommandModel model);
        Task<TaskResponse> DeleteTaskByIdAsync(string id);
    }
}
