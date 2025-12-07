using HomeWorks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace TelegramBot
{
    public class UpdateHandler : IUpdateHandler
    {
        public const string Version = "1.03";
        private readonly IUserService userService;
        private readonly IToDoService toDoService;
        public long telegramUserId;
        public string? telegramUserName;
        public static ToDoUser? User { get; set; }

        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            this.userService = userService;
            this.toDoService = toDoService;
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            var chat = update.Message.Chat;
            var text = update.Message.Text.Trim();
            telegramUserId = update.Message.From.Id;
            telegramUserName = update.Message.From.Username;

            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    botClient.SendMessage(chat, "Пустое сообщение!");
                    return;
                }

                User = userService.GetUser(telegramUserId);
                bool isRegistered = User != null;

                // Только /help и /info доступны незарегистрированным
                if (!isRegistered && text != "/help" && text != "/info" && text != "/start")
                {
                    botClient.SendMessage(chat, "Для доступа к командам зарегистрируйтесь через /start.");
                    return;
                }

                if (text.StartsWith("/addtask "))
                {
                    HandleAddTask(botClient, chat, text.Substring(9).Trim());
                    return;
                }
                if (text.StartsWith("/removetask "))
                {
                    HandleRemoveTask(botClient, chat, text.Substring(12).Trim());
                    return;
                }

                switch (text)
                {
                    case "/start":
                        HandleStart(botClient, chat);
                        break;
                    case "/help":
                        HandleHelp(botClient, chat);
                        break;
                    case "/info":
                        botClient.SendMessage(chat, $"Телеграм-бот Секретарь v.{Version}");
                        break;
                    case "/showtasks":
                        HandleShowTasks(botClient, chat);
                        break;
                    case "/showalltasks":
                        HandleShowAllTasks(botClient, chat);
                        break;
                    case "/completetask":
                        HandleCompleteTask(botClient, chat);
                        break;
                    case "/exit":
                        botClient.SendMessage(chat, "Завершение работы!");
                        break;
                    default:
                        botClient.SendMessage(chat, "Неизвестная команда. Введите /help для списка команд.");
                        break;
                }
            }
            catch (TaskCountLimitException ex)
            {
                botClient.SendMessage(chat, ex.Message);
            }
            catch (TaskLengthLimitException ex)
            {
                botClient.SendMessage(chat, ex.Message);
            }
            catch (DuplicateTaskException ex)
            {
                botClient.SendMessage(chat, ex.Message);
            }
            catch (Exception ex)
            {
                botClient.SendMessage(chat, $"Ошибка: {ex.Message}");
            }
        }

        private void HandleStart(ITelegramBotClient botClient, Chat chat)
        {
            var user = userService.GetUser(telegramUserId);
            if (user != null)
            {
                botClient.SendMessage(chat, $"Вы уже зарегистрированы как {user.TelegramUserName}.");
            }
            else
            {
                user = userService.RegisterUser(telegramUserId, telegramUserName ?? "User");
                botClient.SendMessage(chat, $"Регистрация успешна! Привет, Пользователь!");
            }
            User = user;
        }

        private void HandleHelp(ITelegramBotClient botClient, Chat chat)
        {
            botClient.SendMessage(chat,
                "Доступные команды:\n" +
                "/start - Начало работы с ботом\n" +
                "/help - Справка\n" +
                "/info - Информация о программе\n" +
                "/addtask <имя задачи> - Добавить задачу\n" +
                "/showtasks - Просмотр всех текущих задач\n" +
                "/showalltasks - Просмотр всех задач\n" +
                "/removetask <номер> - Удалить задачу по номеру\n" +
                "/completetask - Завершить последнюю задачу\n" +
                "/exit - Выход");
        }

        private void HandleAddTask(ITelegramBotClient botClient, Chat chat, string taskName)
        {
            if (User == null)
            {
                botClient.SendMessage(chat, "Пользователь не найден.");
                return;
            }
            var item = toDoService.AddTask(User, taskName);
            botClient.SendMessage(chat, $"Задача '{item.Name}' добавлена.");
        }

        private void HandleShowTasks(ITelegramBotClient botClient, Chat chat)
        {
            if (User == null)
            {
                botClient.SendMessage(chat, "Пользователь не найден.");
                return;
            }
            var tasks = toDoService.GetActiveByUserId(User.UserId);
            if (tasks.Count > 0)
            {
                var msg = string.Join("\n", tasks.Select((t, i) => $"{i + 1}. {t.Name} {t.CreatedAt} {t.Id}"));
                botClient.SendMessage(chat, "Список текущих задач:\n" + msg);
            }
            else
            {
                botClient.SendMessage(chat, "Список текущих задач пуст!");
            }
        }

        private void HandleShowAllTasks(ITelegramBotClient botClient, Chat chat)
        {
            if (User == null)
            {
                botClient.SendMessage(chat, "Пользователь не найден.");
                return;
            }
            var tasks = toDoService.GetAllByUserId(User.UserId);
            if (tasks.Count > 0)
            {
                var msg = string.Join("\n", tasks.Select((t, i) => $"{i + 1}. ({t.State}) {t.Name} - {t.CreatedAt} - {t.Id}"));
                botClient.SendMessage(chat, "Список всех задач:\n" + msg);
            }
            else
            {
                botClient.SendMessage(chat, "Список задач пуст!");
            }
        }

        private void HandleRemoveTask(ITelegramBotClient botClient, Chat chat, string numberStr)
        {
            if (User == null)
            {
                botClient.SendMessage(chat, "Пользователь не найден.");
                return;
            }
            if (!int.TryParse(numberStr, out int num))
            {
                botClient.SendMessage(chat, "Некорректный номер задачи.");
                return;
            }
            var tasks = toDoService.GetAllByUserId(User.UserId);
            if (num < 1 || num > tasks.Count)
            {
                botClient.SendMessage(chat, "Номер задачи вне диапазона.");
                return;
            }
            var id = tasks[num - 1].Id;
            toDoService.Delete(id);
            botClient.SendMessage(chat, $"Задача под номером {num} удалена.");
        }

        private void HandleCompleteTask(ITelegramBotClient botClient, Chat chat)
        {
            if (User == null)
            {
                botClient.SendMessage(chat, "Пользователь не найден.");
                return;
            }
            var tasks = toDoService.GetAllByUserId(User.UserId);
            if (tasks.Count == 0)
            {
                botClient.SendMessage(chat, "Список задач пуст!");
                return;
            }
            var id = tasks.Last().Id;
            toDoService.CompleteTask(id);
            botClient.SendMessage(chat, $"Задача с Id {id} помечена как выполненная.");
        }
    }
}
