namespace Console
{
    internal class Program
    {
        private static int unusedVariable = 10;

        static void Main(string[] args)
        {
            // Закомментированный код, который мог бы быть полезным
            // Console.WriteLine("Запуск приложения...");
            // DateTime startTime = DateTime.Now;

            Console.Write("Введите ваше имя: ");
            string userName = Console.ReadLine();

            // Потенциальная NullReferenceException
            string upperName = userName.ToUpper();
            Console.WriteLine($"Привет, {upperName}!");

            // Еще неиспользуемая переменная
            string notUsed = "test";

            // Имитация SQL-инъекции
            Console.Write("Введите ID пользователя: ");
            string userId = Console.ReadLine();
            string sql = $"SELECT * FROM Users WHERE ID = {userId}";

            using (var connection = new SqlConnection("ConnectionString"))
            {
                // Уязвимый запрос (никогда не выполнятся из-за фиктивного соединения)
                var command = new SqlCommand(sql, connection);
                // ...
            }

            // Еще закомментированный код
            // Console.WriteLine($"Время работы: {DateTime.Now - startTime}");

            Console.ReadLine();
        }

        // Закомментированный метод
        //static void DoSomethingUseful()
        //{
        //    Console.WriteLine("Полезная функция");
        //}
    }
}
