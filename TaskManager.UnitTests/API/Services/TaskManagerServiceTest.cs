using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.API.Commands;
using TaskManager.API.Constants;
using TaskManager.API.Exceptions;
using TaskManager.API.Services;
using TaskManager.Domain.CoreModels;
using TaskManager.Domain.DBModels;
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

        public static IEnumerable<object[]> MockDeleteTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.DeleteTaskCommandData() }
            };

        #endregion

        #region Get TaskDetails Test Cases
        [Fact]
        public void It_Should_Return_Task_Details_Succssfully_If_Exists_For_GetTaskDetails_Method()
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.GetTaskByTaskId(It.IsAny<string>())).Returns(Task.FromResult(TaskTestData.TaskDetailsDBData()));

            //Act
            var response = _taskManagerService.GetTaskDetailsById(TaskTestData.TaskDetailsDBData().Id);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(TaskTestData.TaskDetailsDBData().Id, response.Result.Id);
            Assert.Equal(TaskTestData.TaskDetailsDBData().Name, response.Result.Name);
        }

        [Fact]
        public void It_Should_Throw_Exception_If_Task_Not_Exists_For_Given_Id_For_GetTaskDetails_Method()
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.GetTaskByTaskId(It.IsAny<string>())).Returns(Task.FromResult((TaskDetail)null));
            
            //Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _taskManagerService.GetTaskDetailsById("101"));
            Assert.Contains(String.Format(Messages.TaskNotExistsMessage, "101"), exception.Result.Message);
        }

        #endregion

        #region Add Task Test Cases

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_If_Task_already_Exists_With_Same_Id_For_AddNewTaskAsync_Method(AddTaskCommandModel addTaskCommandModelRequest)
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
            Assert.Equal(ResponseStatus.Success.ToString(), response.Result.Status);
            Assert.Equal(Messages.TaskAddedSuccessfully, response.Result.Message);
        }

        #endregion

        #region Update Task Test Cases

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_Task_Not_Found_Exception_If_Task_Not_Exists_With_Id_For_UpdateTaskAsync_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(false);

            //Act & Assert
            var exception = Assert.ThrowsAsync<TaskNotFoundException>(() => _taskManagerService.UpdateTaskAsync(updateTaskCommandModelRequest));
            Assert.Contains(String.Format(Messages.TaskNotExistsMessage, updateTaskCommandModelRequest.Id), exception.Result.Message);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Update_Task_Successfully_In_UpdateTaskAsync_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(true);
            _mockTaskRepository.Setup(x => x.UpdateTaskDetails(It.IsAny<TaskData>()));

            //Act
            var response = _taskManagerService.UpdateTaskAsync(updateTaskCommandModelRequest);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(updateTaskCommandModelRequest.Id, response.Result.Id);
            Assert.Equal(ResponseStatus.Success.ToString(), response.Result.Status);
            Assert.Equal(Messages.TaskUpdatedSuccessfully, response.Result.Message);
        }

        #endregion

        #region Delete Task Test Cases

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Throw_Task_Not_Found_Exception_If_Task_Not_Exists_With_Id_For_DeleteTaskByIdAsync_Method(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(false);

            //Act & Assert
            var exception = Assert.ThrowsAsync<TaskNotFoundException>(() => _taskManagerService.DeleteTaskByIdAsync(deleteTaskCommandModelRequest.Id));
            Assert.Contains(String.Format(Messages.TaskNotExistsMessage, deleteTaskCommandModelRequest.Id), exception.Result.Message);
        }

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Delete_Task_Successfully_In_DeleteTaskByIdAsync_Method(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(true);
            _mockTaskRepository.Setup(x => x.DeleteTaskById(It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var response = _taskManagerService.DeleteTaskByIdAsync(deleteTaskCommandModelRequest.Id);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(deleteTaskCommandModelRequest.Id, response.Result.Id);
            Assert.Equal(ResponseStatus.Success.ToString(), response.Result.Status);
            Assert.Equal(Messages.TaskDeletedSuccessfully, response.Result.Message);
        }

        [Fact]
        public void It_Should_Failure_Response_On_Task_Not_Deleted_Successfully_In_DeleteTaskByIdAsync_Method()
        {
            //Arrange
            _mockTaskRepository.Setup(x => x.FindTaskByTaskId(It.IsAny<string>())).Returns(true);

            //Act
            var response = _taskManagerService.DeleteTaskByIdAsync(string.Empty);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(ResponseStatus.Failure.ToString(), response.Result.Status);
            Assert.Equal(Messages.TaskDeleteFailure, response.Result.Message);
        }

        #endregion
    }
}
