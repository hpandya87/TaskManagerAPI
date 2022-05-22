using Moq;
using System;
using System.Collections.Generic;
using TaskManager.API.Commands;
using TaskManager.API.Constants;
using TaskManager.API.Exceptions;
using TaskManager.API.Services;
using TaskManager.Domain.CoreModels;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using TaskManager.UnitTests.TestData;
using Xunit;

namespace TaskManager.UnitTests.API.Services
{
    public class TaskManagerServiceTest
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly TaskManagerService _taskManagerService;

        public TaskManagerServiceTest()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _taskManagerService = new TaskManagerService(_mockTaskRepository.Object);
        }

        #region Test Data Setup

        public static IEnumerable<object[]> MockAddTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.AddTaskCommandData() }
            };

        public static IEnumerable<object[]> MockUpdateTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.UpdateTaskCommandData() }
            };

        #endregion

        #region Add Task Test Cases

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_If_Task_already_Exists_With_Same_Id_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(true);

            //Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _taskManagerService.AddNewTaskAsync(addTaskCommandModelRequest));
            Assert.Contains(Messages.TaskAlreadyExistsMessage, exception.Result.Message);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_If_More_Than_100_Unfinished_Task_Exists_With_HighPriority_For_DueDate_While_Adding_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.GetCountOfPendingTaskWithHighPriorityByDueDate(It.IsAny<DateTime>())).Returns(100);

            //Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _taskManagerService.AddNewTaskAsync(addTaskCommandModelRequest));
            Assert.Contains(Messages.PendingTaskWithHighPriorityForSameDueDateMessage, exception.Result.Message);

        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Add_New_Task_Successfully_In_AddNewTaskAsync_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(false);
            _mockTaskRepository.Setup(x => x.GetCountOfPendingTaskWithHighPriorityByDueDate(It.IsAny<DateTime>())).Returns(0);
            _mockTaskRepository.Setup(x => x.InserTaskDetails(It.IsAny<TaskData>()));

            //Act
            var response = _taskManagerService.AddNewTaskAsync(addTaskCommandModelRequest);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(addTaskCommandModelRequest.Id, response.Result.Id);
            Assert.Equal(ResponseStatus.Success.ToString(),response.Result.Status);

        }

        #endregion

        #region Update Task Test Cases

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_Task_Not_Found_Exception_If_Task_Not_Exists_With_Id_For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(false);

            //Act & Assert
            var exception = Assert.ThrowsAsync<TaskNotFoundException>(() => _taskManagerService.UpdateTaskAsync(updateTaskCommandModelRequest));
            Assert.Contains(String.Format(Messages.TaskNotExistsMessage,updateTaskCommandModelRequest.Id), exception.Result.Message);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Update_Task_Successfully_In_UpdateTaskAsync_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(true);
            
            //Act
            var response = _taskManagerService.UpdateTaskAsync(updateTaskCommandModelRequest);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(updateTaskCommandModelRequest.Id, response.Result.Id);
            Assert.Equal(ResponseStatus.Success.ToString(), response.Result.Status);

        }
       
        #endregion
    }
}
