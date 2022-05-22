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
    public class DeleteTaskCommandHandlerTest
    {
        private readonly Mock<ITaskManagerService> _mockTaskManagerService;
        private readonly DeleteTaskCommandHandler _deleteTaskCommandHandler;

        public DeleteTaskCommandHandlerTest()
        {
            _mockTaskManagerService = new Mock<ITaskManagerService>();
            _deleteTaskCommandHandler = new DeleteTaskCommandHandler(_mockTaskManagerService.Object);
        }

        #region Test Data Setup

        public static IEnumerable<object[]> MockDeleteTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.DeleteTaskCommandData() }
            };

        #endregion

        #region Delete Task Command Test Cases

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Call_Service_DeleteTaskByIdAsync_Method_When_DeleteTaskCommandHandler_Is_Called(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Act
            var response = _deleteTaskCommandHandler.Handle(deleteTaskCommandModelRequest, new CancellationToken());

            //Assert
            _mockTaskManagerService.Verify(x => x.DeleteTaskByIdAsync(It.IsAny<string>()));
        }

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Handle_Delete_Task_Request_Successfully(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Arrange
            _mockTaskManagerService.Setup(x => x.DeleteTaskByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(TaskTestData.TaskResponseData()));

            //Act
            var response = _deleteTaskCommandHandler.Handle(deleteTaskCommandModelRequest, new CancellationToken());

            //Assert
            Assert.NotNull(response);
            Assert.Equal(deleteTaskCommandModelRequest.Id, response.Result.Id);
            Assert.Equal(ResponseStatus.Success.ToString(), response.Result.Status);
        }

        #endregion
    }
}
