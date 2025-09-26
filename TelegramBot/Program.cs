using System.Text;

namespace TelegramBot
{
    internal class Program
    {
        // Имя пользователя
        public static string UserName { get; set; } = "";

        // Список задач
        public static List<string> TaskList { get; set; }

        public const string Version = "1.02";

        static void Main(string[] args)
        {
            // Вывод приветствия
            SetStartTerminalColor();

            TaskList = new List<string>();

            Console.WriteLine("\nИспользуйте \u001b[36m⬆️\u001b[0m и \u001b[36m⬇️\u001b[0m для навигации и нажмите \u001b[36m\u001b[36mEnter\u001b[0m для выбора:");
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
                    GetUserCommands(option, ref isExit);
                }
            }

            Console.ReadLine();
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
                    if (string.IsNullOrEmpty(UserName))
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

                // /removetask
                case 6:
                    RemoveTask();
                    break;

                // /exit
                case 7:

                    Console.WriteLine("\u001b[36mЗавершение работы!\u001b[0m");
                    isExit = true;
                    break;
            }
        }
        
        /// <summary>
        /// Создать задачу
        /// </summary>
        private static void AddTask()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("Введите описание новой задачи:");
                var output = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(output))
                {
                    TaskList.Add(output);
                    Console.WriteLine("Задача добавлена в список");
                    break;
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
            if (TaskList.Count > 0)
            {
                Console.WriteLine("\u001b[36mСписок текущих задач\u001b[0m");
                Console.WriteLine();

                int i = 1;

                foreach (var task in TaskList)
                {
                    Console.WriteLine(i + ". " + task);
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
            Console.WriteLine("\u001b[32m/removetask  \u001b[0m- Удалить задачу по ее номеру");
            Console.WriteLine("\u001b[32m/exit        \u001b[0m- Выход");
            
            ReturnToMainMenu();
        }

        /// <summary>
        /// Получение имени пользователя /start
        /// </summary>
        private static void GetUserName()
        {
            if (!string.IsNullOrEmpty(UserName))
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
                        UserName = InputUserName();
                        break;
                    }
                }

            }
            else
            {
                // Ввод имени пользователя
                UserName = InputUserName();
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
            if (!string.IsNullOrEmpty(UserName))
            {
                outPutString4 = option == 7 ? decorator : "   ";
            }

            string outPutString5 = option == 3 ? decorator : "   ";
            string outPutString6 = option == 4 ? decorator : "   ";
            string outPutString7 = option == 5 ? decorator : "   ";
            string outPutString8 = option == 6 ? decorator : "   ";

            Console.WriteLine((option == 0 ? decorator : "   ") + "/start\u001b[0m");
            Console.WriteLine((option == 1 ? decorator : "   ") + "/help\u001b[0m");
            Console.WriteLine((option == 2 ? decorator : "   ") + "/info\u001b[0m");

            // Появляется после ввода имени пользователя
            if (!string.IsNullOrEmpty(UserName))
            {
                maxNumMenuItems = 7;
                Console.WriteLine(outPutString5 + "/echo\u001b[0m");
                Console.WriteLine(outPutString6 + "/addtask\u001b[0m");
                Console.WriteLine(outPutString7 + "/showtasks\u001b[0m");
                Console.WriteLine(outPutString8 + "/removetask\u001b[0m");

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
