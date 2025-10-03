namespace HomeWorks
{
    internal class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit) : base($"Превышено максимальное количество задач равное: {taskCountLimit}.")
        {
        }


        public TaskCountLimitException(string message)
            : base(message)
        {
        }

        public TaskCountLimitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


    }
}
