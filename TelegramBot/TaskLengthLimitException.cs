namespace HomeWorks
{
    internal class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit):base($"Длина задачи ‘{ taskLength}’ превышает максимально допустимое значение { taskLengthLimit }")
        {
        }

        public TaskLengthLimitException(string message)
            : base(message)
        {
        }

        public TaskLengthLimitException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
