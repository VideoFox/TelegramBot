using HomeWorks;

namespace TelegramBot
{
    public class UserService : IUserService
    {
        private readonly Dictionary<long, ToDoUser> users = new();

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            if (!users.ContainsKey(telegramUserId))
            {
                var user = new ToDoUser(telegramUserName, telegramUserId);
                users[telegramUserId] = user;
            }
            return users[telegramUserId];
        }

        public ToDoUser? GetUser(long telegramUserId)
        {
            users.TryGetValue(telegramUserId, out var user);
            return user;
        }
    }
}
