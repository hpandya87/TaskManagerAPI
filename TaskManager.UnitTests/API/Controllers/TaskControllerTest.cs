using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.API.Commands;
using TaskManager.API.Constants;
using TaskManager.API.Controllers;
using TaskManager.API.DTOs;
using TaskManager.API.Exceptions;
using TaskManager.API.Queries;
using TaskManager.Domain.CoreModels;
using TaskManager.Domain.Enums;
using TaskManager.UnitTests.TestData;
using Xunit;

namespace TaskManager.UnitTests.API.Controllers
{
    public class TaskControllerTest
    {
        private readonly Mock<IMediator> _mockMediatR;
        private readonly Mock<ILogger<TaskController>> _mockLogger;
        private readonly TaskController _taskController;
        private readonly List<ValidationResult> _validationResults;

        public TaskControllerTest()
        {
            _mockMediatR = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<TaskController>>();
            _taskController = new TaskController(_mockLogger.Object, _mockMediatR.Object);
            _validationResults = new List<ValidationResult>();
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

        public static IEnumerable<object[]> MockRetrieveTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.RetrieveTaskQueryData() }
            };

        public static IEnumerable<object[]> MockDeleteTaskData() =>
            new List<object[]> {
                new object[] { TaskTestData.DeleteTaskCommandData() }
            };

        #endregion

        #region Get Task Test Cases

        [Fact]
        public void It_Should_Call_Get_Task_Method_In_TaskController()
        {
            //Act
            var response = _taskController.Get(null);

            //Assert
            _mockMediatR.Verify(x => x.Send(It.IsAny<RetrieveTaskQueryModel>(), new CancellationToken()));
        }

        [Theory]
        [MemberData(nameof(MockRetrieveTaskData))]
        public void It_Should_Return_Task_Id_Required_While_Retrieving_Task_Details(RetrieveTaskQueryModel retrieveTaskQueryModelRequest)
        {
            //Arrange
            retrieveTaskQueryModelRequest.Id = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(retrieveTaskQueryModelRequest, new ValidationContext(retrieveTaskQueryModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Id field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockRetrieveTaskData))]
        public void It_Should_Return_Task_Id_Maximum_Length_Error_While_Retrieving_Task_Details(RetrieveTaskQueryModel retrieveTaskQueryModelRequest)
        {
            //Arrange
            retrieveTaskQueryModelRequest.Id = "01011900123321312313123312321321312312313213123131231231231123";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(retrieveTaskQueryModelRequest, new ValidationContext(retrieveTaskQueryModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.IdMaximumAllowedLengthMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockRetrieveTaskData))]
        public void It_Should_Return_Task_Details_Successfully_For_Get_Task_Method(RetrieveTaskQueryModel retrieveTaskQueryModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<RetrieveTaskQueryModel>(), new CancellationToken())).Returns(Task.FromResult(TaskTestData.TaskData()));

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(retrieveTaskQueryModelRequest, new ValidationContext(retrieveTaskQueryModelRequest), _validationResults, true);
            var response = (OkObjectResult)_taskController.Get(retrieveTaskQueryModelRequest).Result;

            //Assert
            Assert.True(validationResult);
            Assert.NotNull(response.Value);
            Assert.Equal(retrieveTaskQueryModelRequest.Id, ((TaskData)response.Value).Id);
        }

        [Theory]
        [MemberData(nameof(MockRetrieveTaskData))]
        public void It_Should_Throw_Bad_Request_Exception_If_Task_Not_Exists_With_Id_For_Get_Task_Method(RetrieveTaskQueryModel retrieveTaskQueryModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<RetrieveTaskQueryModel>(), new CancellationToken())).Throws(new ArgumentOutOfRangeException());

            //Act
            var response = (ObjectResult)_taskController.Get(retrieveTaskQueryModelRequest).Result;

            //Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(MockRetrieveTaskData))]
        public void It_Should_Throw_Exception_In_Case_Of_Error_For_Get_Task_Method(RetrieveTaskQueryModel retrieveTaskQueryModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<RetrieveTaskQueryModel>(), new CancellationToken())).Throws(new Exception());

            //Act
            var response = (ObjectResult)_taskController.Get(retrieveTaskQueryModelRequest).Result;

            //Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        #endregion

        #region Add Task Test Cases

        [Fact]
        public void It_Should_Add_Task_Method_In_TaskController_Is_Called()
        {
            //Act
            var response = _taskController.Add(null);

            //Assert
            _mockMediatR.Verify(x => x.Send(It.IsAny<AddTaskCommandModel>(), new CancellationToken()));
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_Id_Required_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.Id = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Id field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_Name_Required_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.Name = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Name field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_DueDate_Required_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.DueDate = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The DueDate field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_DueDate_Invalid_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.DueDate = "01011900";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.DateWithInvalidFormatMassage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_StartDate_Required_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.StartDate = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The StartDate field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_StartDate_Invalid_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.StartDate = "01011900";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.DateWithInvalidFormatMassage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_EndDate_Required_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.EndDate = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The EndDate field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_EndDate_Invalid_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.EndDate = "01011900";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.DateWithInvalidFormatMassage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_Priority_Required_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.Priority = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Priority field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Return_Task_Status_Required_While_Adding_New_Task(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.Status = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Status field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_If_DueDate_Is_In_Past_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.DueDate = System.DateTime.Today.AddDays(-1).ToString(ApiConstants.DateFormat);
            addTaskCommandModelRequest.StartDate = System.DateTime.Today.AddDays(-3).ToString(ApiConstants.DateFormat);
            addTaskCommandModelRequest.EndDate = System.DateTime.Today.AddDays(-2).ToString(ApiConstants.DateFormat);

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.DueDateValidateMassage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_If_StartDate_Is_Earlier_Than_DueDate_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.StartDate = System.DateTime.Today.AddDays(1).ToString(ApiConstants.DateFormat);
            addTaskCommandModelRequest.EndDate = System.DateTime.Today.AddDays(2).ToString(ApiConstants.DateFormat);

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.StartDateValidateMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_If_EndDate_Is_Earlier_Than_StartDate_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.EndDate = System.DateTime.Today.AddDays(-1).ToString(ApiConstants.DateFormat);

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.EndDateValidateMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_If_Priority_Value_Passed_Incorrect_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.Priority = "TEST";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.PriorityValidationMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_If_Status_Value_Passed_Incorrect_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            addTaskCommandModelRequest.Status = "TEST";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.StatusValidationMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Add_Task_Successfully_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<AddTaskCommandModel>(), new CancellationToken())).Returns(Task.FromResult(TaskTestData.TaskResponseData()));

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(addTaskCommandModelRequest, new ValidationContext(addTaskCommandModelRequest), _validationResults, true);
            var response = (OkObjectResult)_taskController.Add(addTaskCommandModelRequest).Result;

            //Assert
            Assert.True(validationResult);
            Assert.NotNull(response);
            Assert.Equal(addTaskCommandModelRequest.Id, ((TaskResponse)response.Value).Id);
            Assert.Equal(ResponseStatus.Success.ToString(), ((TaskResponse)response.Value).Status);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Bad_Request_Exception_If_Task_Already_Exists_With_Same_Id_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<AddTaskCommandModel>(), new CancellationToken())).Throws(new ArgumentOutOfRangeException());

            //Act
            var response = (ObjectResult)_taskController.Add(addTaskCommandModelRequest).Result;

            //Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(MockAddTaskData))]
        public void It_Should_Throw_Exception_In_Case_Of_Error_For_AddTask_Method(AddTaskCommandModel addTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<AddTaskCommandModel>(), new CancellationToken())).Throws(new Exception());

            //Act
            var response = (ObjectResult)_taskController.Add(addTaskCommandModelRequest).Result;

            //Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        #endregion

        #region Update Task Test Cases

        [Fact]
        public void It_Should_Update_Task_Method_In_TaskController_Is_Called()
        {
            //Act
            var response = _taskController.Update(null);

            //Assert
            _mockMediatR.Verify(x => x.Send(It.IsAny<UpdateTaskCommandModel>(), new CancellationToken()));
        }


        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_Id_Is_Required_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.Id = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Id field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_Name_Required_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.Name = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Name field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_DueDate_Required_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.DueDate = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The DueDate field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_DueDate_Is_Invalid_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.DueDate = "01011900";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.DateWithInvalidFormatMassage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_StartDate_Required_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.StartDate = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The StartDate field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_StartDate_Is_Invalid_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.StartDate = "01011900";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.DateWithInvalidFormatMassage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_EndDate_Required_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.EndDate = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The EndDate field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_EndDate_Is_Invalid_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.EndDate = "01011900";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.DateWithInvalidFormatMassage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_Priority_Required_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.Priority = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Priority field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void Task_Status_Required_While_Updating_Task(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.Status = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Status field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_Exception_If_DueDate_Is_In_Past_For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.DueDate = System.DateTime.Today.AddDays(-1).ToString(ApiConstants.DateFormat);
            updateTaskCommandModelRequest.StartDate = System.DateTime.Today.AddDays(-3).ToString(ApiConstants.DateFormat);
            updateTaskCommandModelRequest.EndDate = System.DateTime.Today.AddDays(-2).ToString(ApiConstants.DateFormat);

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.DueDateValidateMassage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_Exception_If_StartDate_Is_Earlier_Than_DueDate_For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.StartDate = System.DateTime.Today.AddDays(1).ToString(ApiConstants.DateFormat);
            updateTaskCommandModelRequest.EndDate = System.DateTime.Today.AddDays(2).ToString(ApiConstants.DateFormat);

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.StartDateValidateMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_Exception_If_EndDate_Is_Earlier_Than_StartDate_For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.EndDate = System.DateTime.Today.AddDays(-1).ToString(ApiConstants.DateFormat);

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.EndDateValidateMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_Exception_If_Priority_Value_Passed_Incorrect_For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.Priority = "TEST";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.PriorityValidationMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_Exception_If_Status_Value_Passed_Incorrect_For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            updateTaskCommandModelRequest.Status = "TEST";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.StatusValidationMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Add_Task_Successfully_For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<UpdateTaskCommandModel>(), new CancellationToken())).Returns(Task.FromResult(TaskTestData.TaskResponseData()));

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(updateTaskCommandModelRequest, new ValidationContext(updateTaskCommandModelRequest), _validationResults, true);
            var response = (OkObjectResult)_taskController.Update(updateTaskCommandModelRequest).Result;

            //Assert
            Assert.True(validationResult);
            Assert.NotNull(response);
            Assert.Equal(updateTaskCommandModelRequest.Id, ((TaskResponse)response.Value).Id);
            Assert.Equal(ResponseStatus.Success.ToString(), ((TaskResponse)response.Value).Status);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_TaskNotFoundException_Exception_If_Task_Not_Exists_For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<UpdateTaskCommandModel>(), new CancellationToken())).Throws(new TaskNotFoundException());

            //Act
            var response = (ObjectResult)_taskController.Update(updateTaskCommandModelRequest).Result;

            //Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(MockUpdateTaskData))]
        public void It_Should_Throw_Exception_In_Case_Of_Error__For_UpdateTask_Method(UpdateTaskCommandModel updateTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<UpdateTaskCommandModel>(), new CancellationToken())).Throws(new Exception());

            //Act
            var response = (ObjectResult)_taskController.Update(updateTaskCommandModelRequest).Result;

            //Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        #endregion

        #region Delete Task Test Cases
        [Fact]
        public void It_Should_Call_Delete_Task_Method_In_TaskController()
        {
            //Act
            var response = _taskController.Delete(null);

            //Assert
            _mockMediatR.Verify(x => x.Send(It.IsAny<DeleteTaskCommandModel>(), new CancellationToken()));
        }

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Return_Task_Id_Required_While_Deleting_Task_Details(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Arrange
            deleteTaskCommandModelRequest.Id = string.Empty;

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(deleteTaskCommandModelRequest, new ValidationContext(deleteTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Contains("The Id field is required.", _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Return_Task_Id_Maximum_Length_Error_While_Deleting_Task(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Arrange
            deleteTaskCommandModelRequest.Id = "01011900123321312313123312321321312312313213123131231231231123";

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(deleteTaskCommandModelRequest, new ValidationContext(deleteTaskCommandModelRequest), _validationResults, true);

            //Assert
            Assert.False(validationResult);
            Assert.Single(_validationResults);
            Assert.Equal(Messages.IdMaximumAllowedLengthMessage, _validationResults[0].ErrorMessage);
        }

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Delete_Task_Successfully_For_Delete_Task_Method(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<DeleteTaskCommandModel>(), new CancellationToken())).Returns(Task.FromResult(TaskTestData.TaskResponseData()));

            //Act
            _validationResults.Clear();
            var validationResult = Validator.TryValidateObject(deleteTaskCommandModelRequest, new ValidationContext(deleteTaskCommandModelRequest), _validationResults, true);
            var response = (OkObjectResult)_taskController.Delete(deleteTaskCommandModelRequest).Result;

            //Assert
            Assert.True(validationResult);
            Assert.NotNull(response.Value);
            Assert.Equal(deleteTaskCommandModelRequest.Id, ((TaskResponse)response.Value).Id);
        }

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Throw_Bad_Request_Exception_If_Task_Not_Exists_With_Id_For_Delete_Task_Method(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<DeleteTaskCommandModel>(), new CancellationToken())).Throws(new ArgumentOutOfRangeException());

            //Act
            var response = (ObjectResult)_taskController.Delete(deleteTaskCommandModelRequest).Result;

            //Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(MockDeleteTaskData))]
        public void It_Should_Throw_Exception_In_Case_Of_Error_For_Delete_Task_Method(DeleteTaskCommandModel deleteTaskCommandModelRequest)
        {
            //Arrange
            _mockMediatR.Setup(x => x.Send(It.IsAny<DeleteTaskCommandModel>(), new CancellationToken())).Throws(new Exception());

            //Act
            var response = (ObjectResult)_taskController.Delete(deleteTaskCommandModelRequest).Result;

            //Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        #endregion
    }
}
