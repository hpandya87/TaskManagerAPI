using System;
using System.Threading.Tasks;
using TaskManager.API.Commands;
using TaskManager.API.Constants;
using TaskManager.API.DTOs;
using TaskManager.API.Exceptions;
using TaskManager.Domain.CoreModels;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;

namespace TaskManager.API.Services
{
    public class TaskManagerService : ITaskManagerService
    {
        private readonly ITaskRepository _taskRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskRepository"></param>
        public TaskManagerService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TaskData> GetTaskDetailsById(string id)
        {
            //Validate Task Id before adding.
            var taskDetail = await _taskRepository.GetTaskByTaskId(id);
            if (taskDetail != null)
            {
                return new TaskData(
                    taskDetail.Id,
                    taskDetail.Name,
                    taskDetail.Description,
                    taskDetail.DueDate,
                    taskDetail.StartDate,
                    taskDetail.EndDate,
                    taskDetail.Priority,
                    taskDetail.Status
                );
            }
            else
                throw new ArgumentOutOfRangeException(String.Format(Messages.TaskNotExistsMessage, id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TaskResponse> AddNewTaskAsync(AddTaskCommandModel model)
        {
            //Validate Task Id before adding.
            var taskExists = _taskRepository.FindTaskByTaskId(model.Id);
            if (taskExists)
                throw new ArgumentOutOfRangeException(Messages.TaskAlreadyExistsMessage);

            var pendingTaskCount = _taskRepository.GetCountOfPendingTaskWithHighPriorityByDueDate(Convert.ToDateTime(model.DueDate));
            if (pendingTaskCount >= ApiConstants.MaxHighPriorityTaskCount)
            {
                throw new ArgumentOutOfRangeException(Messages.PendingTaskWithHighPriorityForSameDueDateMessage);
            }

            //Map Task Data
            var taskDetails = new TaskData(model.Id, model.Name, model.Description, Convert.ToDateTime(model.DueDate),
                Convert.ToDateTime(model.StartDate), Convert.ToDateTime(model.EndDate), model.Priority, model.Status);
            await _taskRepository.InserTaskDetails(taskDetails);
            var response = new TaskResponse(taskDetails.Id, ResponseStatus.Success.ToString(), Messages.TaskAddedSuccessfully);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TaskResponse> UpdateTaskAsync(UpdateTaskCommandModel model)
        {
            //Validate Task before updating.
            var taskExists = _taskRepository.FindTaskByTaskId(model.Id);
            if (!taskExists)
                throw new TaskNotFoundException(String.Format(Messages.TaskNotExistsMessage, model.Id));

            //Map Task Data
            var taskDetails = new TaskData(model.Id, model.Name, model.Description, Convert.ToDateTime(model.DueDate),
                Convert.ToDateTime(model.StartDate), Convert.ToDateTime(model.EndDate), model.Priority, model.Status);
            await _taskRepository.UpdateTaskDetails(taskDetails);
            var response = new TaskResponse(taskDetails.Id, ResponseStatus.Success.ToString(), Messages.TaskUpdatedSuccessfully);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TaskResponse> DeleteTaskByIdAsync(string id)
        {
            //Validate Task before updating.
            var taskExists = _taskRepository.FindTaskByTaskId(id);
            if (!taskExists) 
                throw new TaskNotFoundException(String.Format(Messages.TaskNotExistsMessage, id));

            var result = await _taskRepository.DeleteTaskById(id);
            if (result)
                return new TaskResponse(id, ResponseStatus.Success.ToString(), Messages.TaskDeletedSuccessfully);
            else
                return new TaskResponse(id, ResponseStatus.Failure.ToString(), Messages.TaskDeleteFailure);
        }
    }
}
