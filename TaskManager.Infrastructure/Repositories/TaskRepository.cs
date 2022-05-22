using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Domain.CoreModels;
using TaskManager.Domain.DBModels;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagerDbContext _taskManagerDbContext;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(ILogger<TaskRepository> logger, TaskManagerDbContext taskManagerDbContext)
        {
            _taskManagerDbContext = taskManagerDbContext;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool FindTaskByTaskId(string id)
        {
            return _taskManagerDbContext.TaskDetails.Any(x => x.Id == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public int GetCountOfPendingTaskWithHighPriorityByDueDate(DateTime dueDate)
        {
            int pendingTaskCount = 0;

            var tasks = _taskManagerDbContext.TaskDetails.Where(x =>
                    x.DueDate.Date == dueDate.Date &&
                    x.Priority == Priority.High.ToString() &&
                    x.Status != Status.Finished.ToString()).ToList();
            if (tasks.Count > 0)
            {
                pendingTaskCount = tasks.Count;
            }

            return pendingTaskCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public Task<string> InserTaskDetails(TaskData addTaskRequestData)
        {
            try
            {
                var taskDetails = new TaskDetail
                {
                    Id = addTaskRequestData.Id,
                    Name = addTaskRequestData.Name,
                    Description = addTaskRequestData.Description,
                    DueDate = addTaskRequestData.DueDate,
                    StartDate = addTaskRequestData.StartDate,
                    EndDate = addTaskRequestData.EndDate,
                    Priority = addTaskRequestData.Priority,
                    Status = addTaskRequestData.Status
                };

                _taskManagerDbContext.TaskDetails.Add(taskDetails);
                _taskManagerDbContext.SaveChanges();

                return Task.FromResult(taskDetails.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public Task<string> UpdateTaskDetails(TaskData updateTaskRequestData)
        {
            try
            {
                var result = _taskManagerDbContext.TaskDetails.FirstOrDefault(x => x.Id == updateTaskRequestData.Id);
                if (result != null)
                {
                    result.Name = updateTaskRequestData.Name;
                    result.Description = updateTaskRequestData.Description;
                    result.DueDate = updateTaskRequestData.DueDate;
                    result.StartDate = updateTaskRequestData.StartDate;
                    result.EndDate = updateTaskRequestData.EndDate;
                    result.Priority = updateTaskRequestData.Priority;
                    result.Status = updateTaskRequestData.Status;
                    result.CreatedOn = result.CreatedOn;
                    result.UpdatedOn = DateTime.Now;
                    _taskManagerDbContext.TaskDetails.Update(result);
                    _taskManagerDbContext.SaveChanges();
                }
                return Task.FromResult(result.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}
