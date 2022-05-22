namespace TaskManager.API.Constants
{
    public static class Messages
    {
        public const string DateWithInvalidFormatMassage = "Invalid DueDate, StartDate, or EndDate. Please provide date with valid mm/dd/yyyy or mm-dd-yyyy format.";
        public const string DueDateValidateMassage = "Due Date should be equal to today or future date.";
        public const string StartDateValidateMessage = "Start Date should be equal to or before Due Date.";
        public const string EndDateValidateMessage = "End Date should be after or equal to Start Date.";
        public const string PriorityValidationMessage = "Please provide valid Priority value. Accepted values are Low, Medium or High.";
        public const string StatusValidationMessage = "Please provide valid Status value. Accepted values are New, InProgress or Finished.";
        public const string TaskAlreadyExistsMessage = "Task with the same Id already exists. Please try to provide diffrent Id or update the existing task.";
        public const string TaskNotExistsMessage = "Task with Id {0} does not exists. Please try to add a new task.";
        public const string PendingTaskWithHighPriorityForSameDueDateMessage = "There are alreday 100 task pending with High Priority status for same DueDate. Kindly changes the priority or DueDate to add new task.";
    }
}
