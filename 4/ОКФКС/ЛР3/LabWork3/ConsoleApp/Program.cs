// This is a personal academic project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace ConsoleApp
{
    internal class Program
    {
        private static string unusedField = "test"; // V2008

        static void Main(string[] args)
        {
            // 1. NullReferenceException
            string str = null;
            if (str.Length > 0) // V3080
            {
                Console.WriteLine("Null reference");
            }

            // 2. Деление на ноль
            int zero = 0;
            int result = 10 / zero; // V3064

            // 3. Утечка памяти из-за неправильной подписки на события
            var processor = new DataProcessor();
            processor.ProcessCompleted += OnProcessCompleted;
            processor.ProcessCompleted += OnProcessCompleted; // V3115

            // 4. Потенциальное переполнение буфера
            int[] numbers = new int[10];
            for (int i = 0; i <= 10; i++) // V3106
            {
                numbers[i] = i;
            }

            // 5. SQL Injection
            string userInput = "1; DROP TABLE Users";
            string sql = "SELECT * FROM Users WHERE ID = " + userInput; // V5608

            using (var connection = new SqlConnection("Server=test;Database=test;"))
            {
                var command = new SqlCommand(sql, connection);
                // команда выполняется...
            }

            // 6. Неправильное сравнение объектов
            string s1 = "hello";
            string s2 = "world";
            if (s1 == s2) // V3038 (в некоторых контекстах)
            {
                Console.WriteLine("Strings are equal");
            }

            // 7. Возможная потеря исключения
            try
            {
                DangerousOperation();
            }
            catch (Exception ex) // V3000
            {
                // Пустой блок catch
            }

            // 8. Неправильная работа с IDisposable
            var stream = new MemoryStream();
            stream.WriteByte(1);
            // Забыли вызвать Dispose() // V3200

            // 9. Copy-paste ошибка
            int x = 10, y = 20;
            if (x > 5)
            {
                Console.WriteLine("x is greater than 5");
            }
            else if (x > 5) // V3021
            {
                Console.WriteLine("This will never execute");
            }

            // 10. Бессмысленные условия
            bool flag = true;
            if (flag == true) // V3022
            {
                Console.WriteLine("Redundant comparison");
            }

            // 11. Переменная используется до инициализации
            int uninitialized;
            if (uninitialized > 0) // V3027
            {
                Console.WriteLine("Uninitialized variable");
            }

            // 12. Неправильный порядок операций
            int a = 5, b = 10;
            if (a = b == false) // V3023, V3008
            {
                Console.WriteLine("Wrong operation order");
            }

            // 13. Потенциальная блокировка null
            object lockObj = null;
            lock (lockObj) // V3095
            {
                Console.WriteLine("Lock on null");
            }

            Console.ReadLine();
        }

        static void OnProcessCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Process completed");
        }

        static void DangerousOperation()
        {
            throw new InvalidOperationException("Something went wrong");
        }

        // 14. Виртуальный метод в конструкторе
        class BaseClass
        {
            protected BaseClass()
            {
                VirtualMethod(); // V3110
            }

            protected virtual void VirtualMethod()
            {
                Console.WriteLine("Base implementation");
            }
        }

        class DerivedClass : BaseClass
        {
            private string text;

            protected override void VirtualMethod()
            {
                Console.WriteLine(text.Length); // Может быть null
            }
        }

        // 15. Неправильная реализация Equals
        class BadEquals
        {
            private int value;

            public override bool Equals(object obj)
            {
                var other = obj as BadEquals;
                return value == other.value; // V3080 возможен null
            }

            public override int GetHashCode()
            {
                return value.GetHashCode();
            }
        }

        // 16. Возможное исключение при boxing/unboxing
        static void BoxingProblem()
        {
            object obj = 123;
            string str = (string)obj; // V3055
        }

        // 17. Неправильный флаг форматирования
        static void FormatStringIssue()
        {
            string formatted = string.Format("Value: {1}", 10); // V3025
        }

        // 18. Переменная присваивается сама себе
        static void SelfAssignment()
        {
            int value = 10;
            value = value; // V3005
        }

        // 19. Проверка на null после доступа
        static void NullCheckAfterAccess(List<string> list)
        {
            if (list.Count > 0 && list != null) // V3027
            {
                Console.WriteLine("List has items");
            }
        }

        // 20. Бесконечная рекурсия
        static void InfiniteRecursion(int n)
        {
            if (n > 0)
            {
                InfiniteRecursion(n); // V3110
            }
        }
    }

    class DataProcessor
    {
        public event EventHandler ProcessCompleted;

        public void StartProcess()
        {
            ProcessCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
