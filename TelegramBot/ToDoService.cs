using HomeWorks;

namespace TelegramBot
{
    public class ToDoService : IToDoService
    {
        private readonly Dictionary<Guid, List<ToDoItem>> tasks = new();
        private readonly int taskCountLimit;
        private readonly int taskLengthLimit;

        public ToDoService(int taskCountLimit, int taskLengthLimit)
        {
            this.taskCountLimit = taskCountLimit;
            this.taskLengthLimit = taskLengthLimit;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return tasks.TryGetValue(userId, out var list) ? list : new List<ToDoItem>();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return GetAllByUserId(userId).Where(x => x.State == ToDoItemState.Active).ToList();
        }

        public ToDoItem AddTask(ToDoUser user, string name)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Имя задачи не может быть пустым.");
            if (name.Length > taskLengthLimit) throw new TaskLengthLimitException(name.Length, taskLengthLimit);

            var userTasks = tasks.GetValueOrDefault(user.UserId) ?? new List<ToDoItem>();
            if (userTasks.Count >= taskCountLimit) throw new TaskCountLimitException(taskCountLimit);
            if (userTasks.Any(x => x.Name == name)) throw new DuplicateTaskException(name);

            var item = new ToDoItem(user, name);
            userTasks.Add(item);
            tasks[user.UserId] = userTasks;
            return item;
        }

        public void CompleteTask(Guid id)
        {
            foreach (var list in tasks.Values)
            {
                var item = list.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    item.ChangeState(ToDoItemState.Completed);
                    item.StateChangedAt = DateTime.UtcNow;
                    return;
                }
            }
        }

        public void Delete(Guid id)
        {
            foreach (var list in tasks.Values)
            {
                var item = list.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    list.Remove(item);
                    return;
                }
            }
        }
    }
}
