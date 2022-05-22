namespace TaskManager.API.DTOs
{
    public class TaskResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public TaskResponse(string id, string status, string message)
        {
            Id = id;
            Status = status;
            Message = message;
        }
    }
}
