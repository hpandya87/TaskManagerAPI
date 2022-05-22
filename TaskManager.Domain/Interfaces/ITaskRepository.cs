using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Domain.CoreModels;

namespace TaskManager.Domain.Interfaces
{
    public interface ITaskRepository
    {
        bool FindTaskByTaskId(string Id);

        int GetCountOfPendingTaskWithHighPriorityByDueDate(DateTime dueDate);

        Task<string> InserTaskDetails(TaskData taskData);

        Task<string> UpdateTaskDetails(TaskData taskData);
    }
}
