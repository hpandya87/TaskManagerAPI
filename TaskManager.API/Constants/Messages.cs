namespace TaskManager.API.Constants
{
    public static class Messages
    {
        public const string IdMaximumAllowedLengthMessage = "Id value cannot exceed more than 50 characters.";
        public const string DateWithInvalidFormatMassage = "Invalid DueDate, StartDate, or EndDate. Please provide date with valid mm/dd/yyyy or mm-dd-yyyy format.";
        public const string DueDateValidateMassage = "Due Date should be equal to today or future date.";
        public const string StartDateValidateMessage = "Start Date should be equal to or before Due Date.";
        public const string EndDateValidateMessage = "End Date should be after or equal to Start Date.";
        public const string PriorityValidationMessage = "Please provide valid Priority value. Accepted values are Low, Medium or High.";
        public const string StatusValidationMessage = "Please provide valid Status value. Accepted values are New, InProgress or Finished.";
        public const string TaskAlreadyExistsMessage = "Task with the same Id already exists.";
        public const string TaskNotExistsMessage = "Task Id {0} does not exists.";
        public const string PendingTaskWithHighPriorityForSameDueDateMessage = "There are alreday 100 task pending with High Priority status for same DueDate. Kindly changes the priority or DueDate to add new task.";
        public const string TaskAddedSuccessfully = "Task added successfully.";
        public const string TaskUpdatedSuccessfully = "Task updated successfully.";
        public const string TaskDeletedSuccessfully = "Task deleted successfully.";
        public const string TaskDeleteFailure = "Error while deleting task.";
    }
}
