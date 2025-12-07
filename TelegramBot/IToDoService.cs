using HomeWorks;

namespace TelegramBot
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem AddTask(ToDoUser user, string name);
        void CompleteTask(Guid id);
        void Delete(Guid id);
    }
}
