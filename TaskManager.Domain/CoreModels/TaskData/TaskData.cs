using System;

namespace TaskManager.Domain.CoreModels
{
    public class TaskData
    {   
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }

        public TaskData(string id, string name, string description, DateTime dueDate, DateTime startDate, DateTime endDate
            , string priority, string status)
        {
            Id = id;
            Name = name;
            Description = description;
            DueDate = dueDate;
            StartDate = startDate;
            EndDate = endDate;
            Priority = priority;
            Status = status;
        }
    }
}
