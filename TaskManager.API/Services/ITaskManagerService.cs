using System.Threading.Tasks;
using TaskManager.API.Commands;
using TaskManager.API.DTOs;

namespace TaskManager.API.Services
{
    public interface ITaskManagerService
    {
        Task<TaskResponse> AddNewTaskAsync(AddTaskCommandModel model);
        Task<TaskResponse> UpdateTaskAsync(UpdateTaskCommandModel model);
    }
}
