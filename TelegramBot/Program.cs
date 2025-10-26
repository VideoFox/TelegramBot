using System.Text;
using HomeWorks;

namespace TelegramBot
{
    internal class Program
    {
        #region Поля и свойства
        // Пользователя
        public static ToDoUser? User { get; set; }

        // Список задач
        public static List<ToDoItem>? TaskList { get; set; }

        public const string Version = "1.02";

        private static int taskCountLimit;

        private static int taskLength;

        #endregion

        static void Main(string[] args)
        {
            try
            {
                // Вывод приветствия
                SetStartTerminalColor();

                //  Ввод числа максимального количества задач и обработка неверных результатов
                if (!GetTaskCountLimit("Введите максимальное количество задач<Enter-выход из программы>:", out taskCountLimit)) return;

                //  Ввод числа максимального количества задач и обработка неверных результатов
                if (!GetTaskCountLimit("Введите максимально допустимую длину задачи<Enter-выход из программы>:", out taskLength)) return;

                TaskList = new List<ToDoItem>();

                Console.WriteLine(
                    "\nИспользуйте \u001b[36m⬆️\u001b[0m и \u001b[36m⬇️\u001b[0m для навигации и нажмите \u001b[36m\u001b[36mEnter\u001b[0m для выбора:");
                Console.WriteLine();

                // Координаты курсора для возврата
                var cursorLeft = Console.CursorLeft;
                var cursorTop = Console.CursorTop;

                // Значение по умолчанию
                var option = 0;
                var decorator = "√  \u001b[32m";

                // Индекс последнего пункта меню
                int maxNumMenuItems = 3;

                bool isExit = false;

                while (!isExit)
                {
                    // Запуск меню
                    MenuItemsStart(cursorLeft, cursorTop, option, decorator, ref maxNumMenuItems);

                    // Чтение нажатых клавиш
                    var key = Console.ReadKey(false);

                    // Если не нажат Enter
                    if (!GetDownKey(ref option, ref key, ref maxNumMenuItems))
                        continue;

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();

                        // Выбор пользовательской команды
                        try
                        {
                            GetUserCommands(option, ref isExit);
                        }
                        catch (TaskCountLimitException ex)
                        {
                            Console.Clear();
                            Console.WriteLine(ex.Message);
                            ReturnToMainMenu();
                        }
                        catch (TaskLengthLimitException ex)
                        {
                            Console.Clear();
                            Console.WriteLine(ex.Message);
                            ReturnToMainMenu();
                        }
                        catch (DuplicateTaskException ex)
                        {
                            Console.Clear();
                            Console.WriteLine(ex.Message);
                            ReturnToMainMenu();
                        }
                    }
                }
            }
            catch (ArgumentException arg)
            {
                Console.WriteLine("Произошла ошибка ввода данных! " + arg.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла непредвиденная ошибка:");
                Console.WriteLine("Type: " + ex.GetType());
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
                Console.WriteLine("InnerException: " + ex.InnerException);
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Ввод числа максимального количества  и обработка неверных результатов
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static bool GetTaskCountLimit(string msg, out int resOut)
        {
            resOut = 0;

            while (true)
            {
                Console.WriteLine(msg);

                var str = Console.ReadLine();

                if (string.IsNullOrEmpty(str))
                {
                    Console.WriteLine("Программа отменена!");
                    return false;
                }

                try
                {
                    resOut = ParseAndValidateInt(str, 1, 100);
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return true;
        }

        /// <summary>
        /// Получение и проверка числа
        /// </summary>
        /// <param name="str"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static int ParseAndValidateInt(string? str, int min, int max)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Строка не может быть пустой или null.");

            if (!int.TryParse(str, out int result))
                throw new ArgumentException("Указанная строка не является числом!");

            if (result < min || result > max)
                throw new ArgumentException($"Число должно быть в диапазоне от {min} до {max}.");

            return result;
        }

        /// <summary>
        /// Проверяет, что строка не равна null, не пуста и содержит хотя бы один символ, отличный от пробела.
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void ValidateString(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Строка не может быть пустой, состоять только из пробелов или быть null.");

        }


        /// <summary>
        /// Выбор пользовательской команды
        /// </summary>
        /// <param name="option"></param>
        /// <param name="isExit"></param>
        private static void GetUserCommands(int option, ref bool isExit)
        {
            switch (option)
            {
                // /start
                case 0:
                    GetUserName();
                    break;

                // /help
                case 1:
                    GetHelp();
                    break;

                // /info
                case 2:
                    GetVersion();

                    break;

                case 3:
                    // /exit
                    if (string.IsNullOrEmpty(User?.TelegramUserName))
                    {
                        Console.WriteLine("\u001b[36mЗавершение работы!\u001b[0m");
                        isExit = true;
                    }
                    else
                    {
                        // /echo
                        GetEcho();
                    }

                    break;

                //  / addtask
                case 4:
                    AddTask();
                    break;

                // /showtasks   
                case 5:
                    ViewTasks();
                    break;

                // /showtasks   
                case 6:
                    ViewAllTasks();
                    break;

                // /removetask
                case 7:
                    RemoveTask();
                    break;

                // /completetask
                case 8:
                    CompleteTask();
                    break;


                // /exit
                case 9:

                    Console.WriteLine("\u001b[36mЗавершение работы!\u001b[0m");
                    isExit = true;
                    break;


            }
        }

        private static void ViewAllTasks()
        {
            Console.Clear();

            // Вывод текущих задач
            if (TaskList != null && TaskList.Count > 0)
            {
                Console.WriteLine("\u001b[36mСписок всех задач\u001b[0m");
                Console.WriteLine();
                int i = 1;
                foreach (var task in TaskList)
                {
                    Console.WriteLine($"{i}. ({task.State}) {task.Name} - {task.CreatedAt} - {task.Id}");
                    i++;
                }
            }
            else
            {
                Console.WriteLine("\u001b[36mСписок задач пуст!\u001b[0m");
            }

            // Возврат в главное меню
            ReturnToMainMenu();
        }

        /// <summary>
        ///    /completetask
        /// </summary>
        private static void CompleteTask()
        {
            Console.Clear();

            Console.WriteLine("Введите id задачи для завершения:");
            var outputStr = Console.ReadLine();

            if (!string.IsNullOrEmpty(outputStr))
            {
                try
                {
                    var outputId = Guid.Parse(outputStr);

                    var task = TaskList?.FirstOrDefault(t => t.Id == outputId);

                    if (task != null)
                    {
                        task.ChangeState(ToDoItemState.Completed);
                        task.StateChangedAt = DateTime.UtcNow;

                        Console.WriteLine($"Задача с Id {outputId} помечена как выполненная.");
                    }
                    else
                    {
                        Console.WriteLine($"Задача с Id {outputId} не найдена.");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Неверный формат Id задачи. Пожалуйста, введите корректный GUID.");
                }

            }
            else
            {
                Console.WriteLine("Id задачи не может быть пустым.");
            }



            // Возврат в главное меню
            ReturnToMainMenu();
        }

        /// <summary>
        /// Создать задачу
        /// </summary>
        private static void AddTask()
        {
            while (true)
            {
                if (TaskList != null && TaskList.Count >= taskCountLimit)
                {
                    throw new TaskCountLimitException(taskCountLimit);
                }

                Console.Clear();

                Console.WriteLine("Введите описание новой задачи:");
                var output = Console.ReadLine()?.Trim();

                try
                {
                    ValidateString(output);

                    if (!string.IsNullOrEmpty(output))
                    {
                        if (output.Length > taskLength)
                        {
                            throw new TaskLengthLimitException(output.Length, taskLength);
                        }

                        if (TaskList != null && TaskList.Any(x => x.Name == output))
                        {
                            throw new DuplicateTaskException(output);
                        }


                        if (User != null) TaskList?.Add(new ToDoItem(User, output));
                        Console.WriteLine("Задача добавлена в список");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.Write("Ввести заново? <Y/N>");
                    continue;
                }

                Console.WriteLine("Не указано описание задачи!");
                Console.Write("Ввести заново? <Y/N>");

                bool isExit;

                while (true)
                {
                    var keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.N)
                    {
                        isExit = true;
                        break;
                    }

                    if (keyInfo.Key == ConsoleKey.Y)
                    {
                        isExit = false;
                        Console.WriteLine();
                        break;
                    }
                }

                if (isExit)
                {
                    break;
                }
            }

            // Возврат в главное меню
            ReturnToMainMenu();
        }

        /// <summary>
        /// Просмотреть задачи
        /// </summary>
        private static void ViewTasks()
        {
            Console.Clear();

            // Вывод текущих задач
            PrintTaskList();

            // Возврат в главное меню
            ReturnToMainMenu();
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        private static void RemoveTask()
        {
            while (true)
            {
                Console.Clear();

                PrintTaskList();

                Console.WriteLine("Укажите номер задачи для удаления:");
                var output = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(output))
                {
                    try
                    {
                        var num = int.Parse(output);

                        TaskList.RemoveAt(num - 1);

                        Console.WriteLine($"Задача под номером {num} удалена");
                        Console.WriteLine();
                        break;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Указан неверный номер задачи! Попробуйте заново");
                        Console.WriteLine("Для продолжения нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Укажите целое число и попробуйте заново!");
                        Console.WriteLine("Для продолжения нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                }
                else break;
            }

            // Возврат в главное меню
            ReturnToMainMenu();
        }

        /// <summary>
        /// Вывод текущих задач
        /// </summary>
        private static void PrintTaskList()
        {
            if (TaskList != null && TaskList.Count > 0)
            {
                Console.WriteLine("\u001b[36mСписок текущих задач\u001b[0m");
                Console.WriteLine();

                int i = 1;

                foreach (var task in TaskList)
                {
                    if (task.State == ToDoItemState.Active)
                    {
                        Console.WriteLine($"{i}. {task.Name} {task.CreatedAt} {task.Id}");
                    }

                    i++;
                }
            }
            else
            {
                Console.WriteLine("\u001b[36mСписок текущих задач пуст!\u001b[0m");
            }
        }

        /// <summary>
        /// /echo
        /// </summary>
        private static void GetEcho()
        {
            Console.Write("/echo ");
            var str = Console.ReadLine();

            Console.WriteLine(str);

            ReturnToMainMenu();
        }

        /// <summary>
        /// О программе    /info
        /// </summary>
        private static void GetVersion()
        {
            Console.WriteLine($"Телеграм-бот Секретарь v.{Version}");
            ReturnToMainMenu();
        }

        /// <summary>
        /// Помощь /help
        /// </summary>
        private static void GetHelp()
        {
            Console.WriteLine("\u001b[36mДоступные команды:");
            Console.WriteLine();

            Console.WriteLine("\u001b[32m/start      \u001b[0m - Начало работы с ботом");
            Console.WriteLine("\u001b[32m/help        \u001b[0m- Справка");
            Console.WriteLine("\u001b[32m/info        \u001b[0m- Информация о программе");
            Console.WriteLine("\u001b[32m/echo        \u001b[0m- Эхо команды");
            Console.WriteLine("\u001b[32m/addtask     \u001b[0m- Добавить текущую задачу");
            Console.WriteLine("\u001b[32m/showtasks   \u001b[0m- Просмотр всех текущих задач");
            Console.WriteLine("\u001b[32m/showalltasks\u001b[0m- Просмотр всех задач");
            Console.WriteLine("\u001b[32m/removetask  \u001b[0m- Удалить задачу по ее номеру");
            Console.WriteLine("\u001b[32m/completetask\u001b[0m- Завершить задачу по ее номеру");
            Console.WriteLine("\u001b[32m/exit        \u001b[0m- Выход");

            ReturnToMainMenu();
        }

        /// <summary>
        /// Получение имени пользователя /start
        /// </summary>
        private static void GetUserName()
        {
            if (!string.IsNullOrEmpty(User?.TelegramUserName))
            {
                Console.Write("Вы уже вводили свое имя. Заменить<Y/N>");

                while (true)
                {
                    var keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.N)
                    {
                        SetStartTerminalColor();
                        break;
                    }

                    if (keyInfo.Key == ConsoleKey.Y)
                    {
                        // Ввод имени пользователя
                        User = new ToDoUser(InputUserName());
                        break;
                    }
                }

            }
            else
            {
                // Ввод имени пользователя
                User = new ToDoUser(InputUserName());
            }
        }

        /// <summary>
        /// Запуск меню
        /// </summary>
        /// <param name="cursorLeft"></param>
        /// <param name="cursorTop"></param>
        /// <param name="option"></param>
        /// <param name="decorator"></param>
        /// <param name="maxNumMenuItems"></param>
        private static void MenuItemsStart(int cursorLeft, int cursorTop, int option, string decorator, ref int maxNumMenuItems)
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);

            string outPutString4 = option == 3 ? decorator : "   ";
            if (User != null && !string.IsNullOrEmpty(User.TelegramUserName))
            {
                outPutString4 = option == 9 ? decorator : "   ";
            }

            string outPutString5 = option == 3 ? decorator : "   ";
            string outPutString6 = option == 4 ? decorator : "   ";
            string outPutString7 = option == 5 ? decorator : "   ";
            string outPutString8 = option == 6 ? decorator : "   ";
            string outPutString9 = option == 7 ? decorator : "   ";
            string outPutString10 = option == 8 ? decorator : "   ";

            Console.WriteLine((option == 0 ? decorator : "   ") + "/start\u001b[0m");
            Console.WriteLine((option == 1 ? decorator : "   ") + "/help\u001b[0m");
            Console.WriteLine((option == 2 ? decorator : "   ") + "/info\u001b[0m");

            // Появляется после ввода имени пользователя
            if (User != null && !string.IsNullOrEmpty(User.TelegramUserName))
            {
                maxNumMenuItems = 9;
                Console.WriteLine(outPutString5 + "/echo\u001b[0m");
                Console.WriteLine(outPutString6 + "/addtask\u001b[0m");
                Console.WriteLine(outPutString7 + "/showtasks\u001b[0m");
                Console.WriteLine(outPutString8 + "/showalltasks\u001b[0m");
                Console.WriteLine(outPutString9 + "/removetask\u001b[0m");
                Console.WriteLine(outPutString10 + "/completetask\u001b[0m");
            }

            Console.WriteLine(outPutString4 + "/exit\u001b[0m");


        }

        /// <summary>
        /// Получение нажатой клавиши
        /// </summary>
        /// <param name="option"></param>
        /// <param name="key"></param>
        /// <param name="maxNumMenuItems"></param>
        private static bool GetDownKey(ref int option, ref ConsoleKeyInfo key, ref int maxNumMenuItems)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    option = option == 0 ? maxNumMenuItems : option - 1;
                    break;
                case ConsoleKey.DownArrow:
                    option = option == maxNumMenuItems ? 0 : option + 1;
                    break;
                case ConsoleKey.Enter:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Ввод имени пользователя
        /// </summary>
        /// <returns></returns>
        private static string InputUserName()
        {
            string userName;
            Console.Clear();

            while (true)
            {
                Console.WriteLine("Введите свое имя:");
                var output = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(output))
                {
                    userName = output;
                    Console.WriteLine($"Привет {userName}");

                    // Возврат в главное меню
                    ReturnToMainMenu();
                    break;
                }

                Console.WriteLine("Имя не указано!");
            }

            return userName;
        }

        /// <summary>
        /// Возврат в главное меню
        /// </summary>
        private static void ReturnToMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("\u001b[36mДля возврата в меню нажмите любую клавишу\u001b[0m");
            Console.ReadKey();
            SetStartTerminalColor();
        }

        /// <summary>
        /// Вывод приветствия
        /// </summary>
        private static void SetStartTerminalColor()
        {
            Console.Clear();
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Вас приветствует бот-секретарь!");
            Console.ResetColor();
        }

    }
}
