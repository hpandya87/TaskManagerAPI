using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Domain.CoreModels;
using TaskManager.Domain.DBModels;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.Repositories;
using TaskManager.UnitTests.TestData;
using Xunit;

namespace TaskManager.UnitTests.Infrastructure.Repositories
{
    public class TaskRepositoryTest
    {
        private readonly Mock<TaskManagerDbContext> _mockTaskManagerDbContext;
        private readonly Mock<ILogger<TaskRepository>> _mockLogger;
        private readonly DbContextOptions<TaskManagerDbContext> _dbContextOptions;

        public TaskRepositoryTest()
        {
            _mockLogger = new Mock<ILogger<TaskRepository>>();
            _mockTaskManagerDbContext = new Mock<TaskManagerDbContext>();

            _dbContextOptions = new DbContextOptionsBuilder<TaskManagerDbContext>()
               .UseInMemoryDatabase(databaseName: "TaskManager")
               .Options;
        }

        #region Test Data Setup

        public static IEnumerable<object[]> MockAddTaskData() =>
                    new List<object[]> {
                new object[] { TaskTestData.TaskData() }
                    };

        #endregion

        #region Test Cases

        [Fact]
        public void It_Should_Return_True_If_Task_Exists_With_Same_Id_For_FindTaskByTaskId_Method()
        {
            using (var mocktaskManagerDbContext = new TaskManagerDbContext(_dbContextOptions))
            {
                //Arrange
                mocktaskManagerDbContext.TaskDetails.Add(TaskTestData.TaskDetailsDBData());
                mocktaskManagerDbContext.SaveChanges();

                //Act 
                var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext);
                var resonese = taskRepository.FindTaskByTaskId(TaskTestData.TaskData().Id);

                //Assert
                Assert.True(resonese);
            }
        }

        [Fact]
        public void It_Should_Return_False_If_Task_Not_Exists_With_Same_Id_For_FindTaskByTaskId_Method()
        {
            using (var mocktaskManagerDbContext = new TaskManagerDbContext(_dbContextOptions))
            {
                //Arrange
                var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext);

                //Act
                var resonese = taskRepository.FindTaskByTaskId("");

                //Assert
                Assert.False(resonese);
            }
        }

        [Fact]
        public void It_Should_Return_Count_Of_Pending_Task_With_High_Priority_By_DueDate()
        {
            using (var mocktaskManagerDbContext = new TaskManagerDbContext(_dbContextOptions))
            {
                //Arrange
                mocktaskManagerDbContext.TaskDetails.Add(TaskTestData.HighPriorityTaskDetailsDBData());
                mocktaskManagerDbContext.SaveChanges();

                var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext);

                //Act
                var resonese = taskRepository.GetCountOfPendingTaskWithHighPriorityByDueDate(TaskTestData.TaskData().DueDate);

                //Assert
                Assert.True(resonese > 0);
            }
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_Details_Successfully_For_GetTaskByTaskId_Method(TaskData taskRequestData)
        {
            using (var mocktaskManagerDbContext = new TaskManagerDbContext(_dbContextOptions))
            {
                //Arrange
                var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext);

                //Act
                var resonese = taskRepository.GetTaskByTaskId(taskRequestData.Id);

                //Assert
                Assert.NotNull(resonese);
                Assert.Equal(taskRequestData.Id, resonese.Result.Id);
            }
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Insert_New_Task_Successfully_For_InserTaskDetails_Method(TaskData addTaskRequestData)
        {
            using (var mocktaskManagerDbContext = new TaskManagerDbContext(_dbContextOptions))
            {
                //Arrange
                var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext);

                //Act
                addTaskRequestData.Id = Guid.NewGuid().ToString();
                var resonese = taskRepository.InserTaskDetails(addTaskRequestData);

                //Assert
                Assert.NotNull(resonese);
                Assert.Equal(addTaskRequestData.Id, resonese.Result);
            }
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_In_Case_Of_Error_While_Adding_New_Task_For_InserTaskDetails_Method(TaskData addTaskRequestData)
        {
            //Arrange
            var mocktaskManagerDbContext = new Mock<TaskManagerDbContext>();
            mocktaskManagerDbContext.Setup(x => x.SaveChanges()).Callback(() => throw new Exception());
            mocktaskManagerDbContext.Setup(x => x.TaskDetails).Returns(DbContextMock.GetQueryableMockDbSet<TaskDetail>(TaskTestData.TaskDetailsDBList()));
            var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext.Object);

            //Assert
            Assert.Throws<Exception>(() => taskRepository.InserTaskDetails(addTaskRequestData).Result);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Update_Task_Successfully_For_UpdateTaskDetails_Method(TaskData updateTaskRequestData)
        {
            using (var mocktaskManagerDbContext = new TaskManagerDbContext(_dbContextOptions))
            {
                //Arrange
                var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext);

                //Act
                var resonese = taskRepository.UpdateTaskDetails(updateTaskRequestData);

                //Assert
                Assert.NotNull(resonese);
                Assert.Equal(updateTaskRequestData.Id, resonese.Result);
            }
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_In_Case_Of_Error_While_Updating_Existing_Task_For_UpdateTaskDetails_Method(TaskData updateTaskRequestData)
        {
            //Arrange
            var mocktaskManagerDbContext = new Mock<TaskManagerDbContext>();
            mocktaskManagerDbContext.Setup(x => x.SaveChanges()).Callback(() => throw new Exception());
            mocktaskManagerDbContext.Setup(x => x.TaskDetails).Returns(DbContextMock.GetQueryableMockDbSet<TaskDetail>(TaskTestData.TaskDetailsDBList()));

            var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext.Object);

            //Assert
            Assert.Throws<Exception>(() => taskRepository.UpdateTaskDetails(updateTaskRequestData).Result);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Delete_Task_Successfully_For_DeleteTaskById_Method(TaskData deleteTaskRequestData)
        {
            using (var mocktaskManagerDbContext = new TaskManagerDbContext(_dbContextOptions))
            {
                //Arrange
                var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext);

                //Act
                var newTaskId = Guid.NewGuid().ToString();
                deleteTaskRequestData.Id = newTaskId;
                taskRepository.InserTaskDetails(deleteTaskRequestData);
                var resonese = taskRepository.DeleteTaskById(newTaskId);

                //Assert
                Assert.NotNull(resonese);
                Assert.True(resonese.Result);
            }
        }

        [Fact]
        public void It_Should_Return_False_If_Task_Not_Found_To_Delete_For_DeleteTaskById_Method()
        {
            using (var mocktaskManagerDbContext = new TaskManagerDbContext(_dbContextOptions))
            {
                //Arrange
                var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext);

                //Act
                var resonese = taskRepository.DeleteTaskById(Guid.NewGuid().ToString());

                //Assert
                Assert.NotNull(resonese);
                Assert.False(resonese.Result);
            }
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_In_Case_Of_Error_While_Deleting_Existing_Task_For_DeleteTaskById_Method(TaskData deleteTaskRequestData)
        {
            //Arrange
            var mocktaskManagerDbContext = new Mock<TaskManagerDbContext>();
            mocktaskManagerDbContext.Setup(x => x.SaveChanges()).Callback(() => throw new Exception());
            mocktaskManagerDbContext.Setup(x => x.TaskDetails).Returns(DbContextMock.GetQueryableMockDbSet<TaskDetail>(TaskTestData.TaskDetailsDBList()));

            var taskRepository = new TaskRepository(_mockLogger.Object, mocktaskManagerDbContext.Object);

            //Assert
            Assert.Throws<Exception>(() => taskRepository.DeleteTaskById(deleteTaskRequestData.Id).Result);
        }

        #endregion
    }

    public static class DbContextMock
    {
        public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));
            return dbSet.Object;
        }
    }
}
