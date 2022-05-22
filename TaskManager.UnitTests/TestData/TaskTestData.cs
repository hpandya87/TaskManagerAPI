using System;
using System.Collections.Generic;
using TaskManager.API.Commands;
using TaskManager.API.Constants;
using TaskManager.API.DTOs;
using TaskManager.Domain.CoreModels;
using TaskManager.Domain.DBModels;

namespace TaskManager.UnitTests.TestData
{
    public static class TaskTestData
    {
        public static AddTaskCommandModel AddTaskCommandData() =>
            new AddTaskCommandModel(
                "101", "Task 101", "Sample Task 1", DateTime.Today.ToString(ApiConstants.DateFormat), DateTime.Today.ToString(ApiConstants.DateFormat), 
                DateTime.Today.ToString(ApiConstants.DateFormat), "Low", "New");

        public static UpdateTaskCommandModel UpdateTaskCommandData() =>
           new UpdateTaskCommandModel(
               "101", "Task 101", "Sample Task 1", DateTime.Today.ToString(ApiConstants.DateFormat), DateTime.Today.ToString(ApiConstants.DateFormat), 
               DateTime.Today.ToString(ApiConstants.DateFormat), "Low", "New");

        public static TaskData TaskData() =>
            new TaskData("101", "Task 101", "Test Task", DateTime.Today.Date, DateTime.Today.Date, DateTime.Today.Date, "Low", "New");

        public static TaskResponse TaskResponseData() =>
            new TaskResponse("101", "Success");

        public static TaskDetail TaskDetailsDBData() =>
            new TaskDetail
            {
                Id = "101",
                Name = "Task 101",
                Description = "Sample Task 1",
                DueDate = DateTime.Today.Date,
                StartDate = DateTime.Today.Date,
                EndDate = DateTime.Today.Date,
                Priority = "Low",
                Status = "InProgress",
                CreatedOn = DateTime.Today,
                UpdatedOn = DateTime.Today

            };

        public static TaskDetail HighPriorityTaskDetailsDBData() =>
            new TaskDetail
            {
                Id = "102",
                Name = "Task 102",
                Description = "Sample Task 2",
                DueDate = DateTime.Today.Date,
                StartDate = DateTime.Today.Date,
                EndDate = DateTime.Today.Date,
                Priority = "High",
                Status = "New"
            };

        public static List<TaskDetail> TaskDetailsDBList()
        {
            var lstTaskDetails = new List<TaskDetail>();
            lstTaskDetails.Add(
            new TaskDetail
            {
                Id = "101",
                Name = "Task 101",
                Description = "Sample Task 1",
                DueDate = DateTime.Today.Date,
                StartDate = DateTime.Today.Date,
                EndDate = DateTime.Today.Date,
                Priority = "Low",
                Status = "InProgress"
            });

            return lstTaskDetails;
        }
    }
}
