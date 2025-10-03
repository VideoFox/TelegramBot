namespace HomeWorks
{
    internal class DuplicateTaskException : Exception
    {
        public DuplicateTaskException()
        {
        }

        public DuplicateTaskException(string task)
            : base($"Задача ‘{task}’ уже существует")
        {
        }

        public DuplicateTaskException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
