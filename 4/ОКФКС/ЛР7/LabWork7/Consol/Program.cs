using System.IO.Pipes;
using System.Text;

namespace ScriptClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var pipe = new NamedPipeClientStream(".", "mypipe", PipeDirection.Out);
                pipe.Connect();
                var writer = new StreamWriter(pipe);
                writer.WriteLine("Message(\"Concosl start!\")");
                writer.Flush();
                writer.Close();
                pipe.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}