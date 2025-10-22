using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace LabWork7
{
    public class ScriptingHost
    {
        private readonly EventService _eventService;
        private readonly ScriptOptions scriptOptions;

        public ScriptingHost()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .ToArray();

            scriptOptions = ScriptOptions.Default
                .WithReferences(assemblies)
                .WithImports(
                    "System",
                    "System.Collections.Generic",
                    "System.Linq"
                );
        }

        public async Task ExecuteAsync(string code)
        {
            await CSharpScript.EvaluateAsync(code, scriptOptions, new Automation());
        }

        public async Task Run(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (var server = new NamedPipeServerStream("mypipe", PipeDirection.In))
                    {
                        CancellationToken cancellationToken = default;
                        server.WaitForConnection();
                        await HandleClient(server, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка канала: {ex.Message}");
                    await Task.Delay(500, token);
                }
            }

        }

        private async Task HandleClient(NamedPipeServerStream server, CancellationToken cancellationToken)
        {
            try
            {
                using (var reader = new StreamReader(server, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: false))
                {
                    string? command = await reader.ReadLineAsync();
                    if (command != null)
                    {
                        await ExecuteAsync(command);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки клиента: {ex.Message}");
            }
        }



    }

}