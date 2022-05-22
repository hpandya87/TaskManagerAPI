using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.API.Commands;
using TaskManager.API.Services;
using TaskManager.Domain.Enums;
using TaskManager.UnitTests.TestData;
using Xunit;

namespace TaskManager.UnitTests.API.Commands
{
    public class AddTaskCommandHandlerTest
    {
        private readonly Mock<ITaskManagerService> _mockTaskManagerService;
        private readonly AddTaskCommandHandler _addTaskCommandHandler;

        public AddTaskCommandHandlerTest()
        {
            _mockTaskManagerService = new Mock<ITaskManagerService>();
            _addTaskCommandHandler = new AddTaskCommandHandler(_mockTaskManagerService.Object);
        }

        #region Test Data Setup

        public static IEnumerable<object[]> MockAddTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.AddTaskCommandData() }
            };

        #endregion

        #region Add Task Test Cases

        [Fact]
        public void It_Should_Call_Service_AddNewTaskAsync_Method_When_AddTaskCommandHandler_Is_Called()
        {
            //Arrange
            
            //Act
            var response = _addTaskCommandHandler.Handle(null, new CancellationToken());

            //Assert
            _mockTaskManagerService.Verify(x => x.AddNewTaskAsync(It.IsAny<AddTaskCommandModel>()));
       }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Handle_Add_Task_Request_Successfully(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskManagerService.Setup(x => x.AddNewTaskAsync(It.IsAny<AddTaskCommandModel>())).Returns(Task.FromResult(TaskTestData.TaskResponseData()));

            //Act
            var response = _addTaskCommandHandler.Handle(addTaskCommandModelRequest, new CancellationToken());

            //Assert
            Assert.NotNull(response);
            Assert.Equal(addTaskCommandModelRequest.Id,response.Result.Id);
            Assert.Equal(ResponseStatus.Success.ToString(),response.Result.Status);
        }

        #endregion
    }
}
