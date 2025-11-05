using System.Xml.Linq;

internal class Program
{

    private static readonly string PASSWORD = "SuperSecret123";
    private static void Main(string[] args)
    {
        var obj = new MyClass.Class();
        Console.WriteLine(obj.SomeMethod());

        Console.WriteLine("Введите пароль:");
        string input = Console.ReadLine();

        if (input == PASSWORD)
        {
            Console.WriteLine("Авторизация успешна!");
        }
        else
        {
            Console.WriteLine("Неверный пароль.");
        }

        Console.ReadKey();
    }
}