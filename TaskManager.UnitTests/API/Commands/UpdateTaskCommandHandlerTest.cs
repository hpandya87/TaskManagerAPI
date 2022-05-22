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
    public class UpdateTaskCommandHandlerTest
    {
        private readonly Mock<ITaskManagerService> _mockTaskManagerService;

        public UpdateTaskCommandHandlerTest()
        {
            _mockTaskManagerService = new Mock<ITaskManagerService>();
        }

        #region Test Data Setup

        public static IEnumerable<object[]> MockAddTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.UpdateTaskCommandData() }
            };

        #endregion

        #region Update Task Test Cases

        [Fact]
        public void It_Should_Call_Service_UpdateTaskAsync_Method_When_UpdateTaskCommandHandler_Is_Called()
        {
            //Arrange
            var updateTaskCommandHandler = new UpdateTaskCommandHandler(_mockTaskManagerService.Object);

            //Act
            var response = updateTaskCommandHandler.Handle(null, new CancellationToken());

            //Assert
            _mockTaskManagerService.Verify(x => x.UpdateTaskAsync(It.IsAny<UpdateTaskCommandModel>()));
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Handle_Update_Task_Request_Successfully(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskManagerService.Setup(x => x.UpdateTaskAsync(It.IsAny<UpdateTaskCommandModel>())).Returns(Task.FromResult(TaskTestData.TaskResponseData()));
            var updateTaskCommandHandler = new UpdateTaskCommandHandler(_mockTaskManagerService.Object);

            //Act
            var response = updateTaskCommandHandler.Handle(updateTaskCommandModelRequest, new CancellationToken());

            //Assert
            Assert.NotNull(response);
            Assert.Equal(updateTaskCommandModelRequest.Id, response.Result.Id);
            Assert.Equal(ResponseStatus.Success.ToString(), response.Result.Status);
        }

        #endregion 
    }
}
