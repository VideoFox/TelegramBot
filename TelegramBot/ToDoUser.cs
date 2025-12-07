namespace HomeWorks
{
    public class ToDoUser
    {
        public Guid UserId { get; }
        public string TelegramUserName { get; set; }
        public DateTime RegisteredAt { get; }
        public long TelegramUserId { get; set; }

        public ToDoUser(string telegramUserName, long telegramUserId)
        {
            UserId = Guid.NewGuid();
            TelegramUserName = telegramUserName;
            TelegramUserId = telegramUserId;
            RegisteredAt = DateTime.UtcNow;
        }
    }
}
