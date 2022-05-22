using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Domain.CoreModels;
using TaskManager.Domain.DBModels;

namespace TaskManager.Domain.Interfaces
{
    public interface ITaskRepository
    {
        bool FindTaskByTaskId(string Id);

        int GetCountOfPendingTaskWithHighPriorityByDueDate(DateTime dueDate);

        Task<TaskDetail> GetTaskByTaskId(string id);

        Task<string> InserTaskDetails(TaskData taskData);

        Task<string> UpdateTaskDetails(TaskData taskData);

        Task<bool> DeleteTaskById(string id);
    }
}
