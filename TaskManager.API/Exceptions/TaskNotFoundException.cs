using System;

namespace TaskManager.API.Exceptions
{
    [Serializable]
    public class TaskNotFoundException : Exception
    {
        /// <summary>
        /// Task Not Found Exception
        /// </summary>
        public TaskNotFoundException() { 
        }

        /// <summary>
        /// Task Not Found Exception
        /// </summary>
        /// <param name="message"></param>
        public TaskNotFoundException(string message) : base(message) { 
        }
    }
}
