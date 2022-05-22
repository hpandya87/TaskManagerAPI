using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.API.Queries;
using TaskManager.API.Services;
using TaskManager.UnitTests.TestData;
using Xunit;

namespace TaskManager.UnitTests.API.Queries
{
    public class RetrieveTaskQueryHandlerTest
    {
        private readonly Mock<ITaskManagerService> _mockTaskManagerService;
        private readonly RetrieveTaskQueryHandler _retrieveTaskQueryHandler;

        public RetrieveTaskQueryHandlerTest() {
            _mockTaskManagerService = new Mock<ITaskManagerService>();
            _retrieveTaskQueryHandler = new RetrieveTaskQueryHandler(_mockTaskManagerService.Object);
        }

        #region Test Data Setup

        public static IEnumerable<object[]> MockRetrieveTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.RetrieveTaskQueryData() }
            };

        #endregion

        #region Retrieve Task Details Query Test Cases

        [Theory]
        [MemberData(nameof(MockRetrieveTaskData))]
        public void It_Should_Call_Service_GetTaskDetailsById_Method_When_RetrieveTaskQueryHandler_Is_Called(RetrieveTaskQueryModel retrieveTaskQueryModelRequest)
        {
            //Act
            var response = _retrieveTaskQueryHandler.Handle(retrieveTaskQueryModelRequest, new CancellationToken());

            //Assert
            _mockTaskManagerService.Verify(x => x.GetTaskDetailsById(It.IsAny<string>()));
        }

        [Theory]
        [MemberData(nameof(MockRetrieveTaskData))]
        public void It_Should_Handle_Retrieve_Task_Details_Request_Successfully(RetrieveTaskQueryModel retrieveTaskQueryModelRequest)
        {
            //Arrange
            _mockTaskManagerService.Setup(x => x.GetTaskDetailsById(It.IsAny<string>())).Returns(Task.FromResult(TaskTestData.TaskData()));

            //Act
            var response = _retrieveTaskQueryHandler.Handle(retrieveTaskQueryModelRequest, new CancellationToken());

            //Assert
            Assert.NotNull(response);
            Assert.Equal(retrieveTaskQueryModelRequest.Id, response.Result.Id);
            Assert.Equal(TaskTestData.TaskData().Name, response.Result.Name);
        }

        #endregion
    }
}
