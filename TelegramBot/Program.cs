using System.Text;
using System.Threading.Channels;

namespace TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Вывод приветствия
            SetStartTerminalColor();

            // Имя пользователя
            string userName = "";

            string version = "1.00";

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

            while (true)
            {
                Console.SetCursorPosition(cursorLeft, cursorTop);

                string outPutString1 = option == 0 ? decorator : "   ";
                string outPutString2 = option == 1 ? decorator : "   ";
                string outPutString3 = option == 2 ? decorator : "   ";

                string outPutString4 = option == 3 ? decorator : "   ";
                if (!string.IsNullOrEmpty(userName))
                {
                    outPutString4 = option == 4 ? decorator : "   ";
                }

                string outPutString5 = option == 3 ? decorator : "   ";

                Console.WriteLine(outPutString1 + "/start\u001b[0m");
                Console.WriteLine(outPutString2 + "/help\u001b[0m");
                Console.WriteLine(outPutString3 + "/info\u001b[0m");

                // Появляется после ввода имени пользователя
                if (!string.IsNullOrEmpty(userName))
                {
                    maxNumMenuItems = 4;
                    Console.WriteLine(outPutString5 + "/echo\u001b[0m");
                }

                Console.WriteLine(outPutString4 + "/exit\u001b[0m");


                // Чтение нажатых клавиш
                ConsoleKeyInfo key = Console.ReadKey(false);

                if (key.Key == ConsoleKey.UpArrow)
                {
                    option = option == 0 ? maxNumMenuItems : option - 1;
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    option = option == maxNumMenuItems ? 0 : option + 1;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    // /start
                    if (option == 0)
                    {
                        if (!string.IsNullOrEmpty(userName))
                        {
                            Console.Clear();
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
                                    userName = GetUserName();
                                    break;
                                }
                            }

                        }
                        else
                        {
                            // Ввод имени пользователя
                            userName = GetUserName();
                        }


                    }

                    // /help
                    if (option == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("Тут будет справочная информация о боте");
                        ReturnToMainMenu();
                    }

                    // /info
                    if (option == 2)
                    {
                        Console.Clear();
                        Console.WriteLine($"Телеграм-бот Секретарь v.{version}");
                        ReturnToMainMenu();
                    }

                    // /exit
                    if (option == maxNumMenuItems)
                    {
                        Console.Clear();
                        Console.WriteLine("\u001b[36mЗавершение работы!\u001b[0m");
                        break;
                    }

                    // /echo
                    if (!string.IsNullOrEmpty(userName) && option == 3)
                    {
                        Console.Clear();
                        Console.WriteLine($"Привет {userName}");
                        Console.WriteLine("Не совсем понял, что должно быть. Пока так. При замечании дорааботаю");
                        ReturnToMainMenu();
                    }
                }
            }

            Console.ReadLine();

        }

        /// <summary>
        /// Ввод имени пользователя
        /// </summary>
        /// <returns></returns>
        private static string GetUserName()
        {
            string userName;
            while (true)
            {
                Console.Clear();
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
