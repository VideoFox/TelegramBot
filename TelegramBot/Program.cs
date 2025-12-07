using System.Text;
using HomeWorks;
using Otus.ToDoList.ConsoleBot;

namespace TelegramBot
{
    internal class Program
    {
        public static int taskCount;

        public static int taskLength;
        
        static void Main(string[] args)
        {
            try
            {
                // Вывод приветствия
                SetStartTerminalColor();

                //  Ввод числа максимального количества задач и обработка неверных результатов
                if (!GetTaskCountLimit("Введите максимальное количество задач<Enter-выход из программы>:", out taskCount)) return;

                //  Ввод числа максимального количества задач и обработка неверных результатов
                if (!GetTaskCountLimit("Введите максимально допустимую длину задачи<Enter-выход из программы>:", out taskLength)) return;

                // Запуск TelegramBot клиента с UpdateHandler
                var botClient = new ConsoleBotClient();
                var updateHandler = new UpdateHandler(new UserService(),new ToDoService(taskCount,taskLength));
                botClient.StartReceiving(updateHandler);
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
