using System.ComponentModel;

namespace TaskManager.Domain.Enums
{
    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum Status
    {
        New = 1,
        InProgress = 2,
        Finished = 3,
    }

    public enum ResponseStatus
    {
        Success,
        Failure
    }
}
