namespace LabWork4
{
    public class HttpRetry
    {
        private static readonly HttpClient httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        private const int MaxRetries = 3;
        private const int BaseDelayMs = 1000;

        public static async Task Task1()
        {
            string url = "https://www.arcotel.ru/".Trim();

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0 Safari/537.36");

            try
            {
                string response = await RetryHttpRequestAsync(url);
                Console.WriteLine("Успешный ответ:");
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка после {MaxRetries} попыток: {ex.Message}");
            }
        }

        private static async Task<string> RetryHttpRequestAsync(string url)
        {
            int retryCount = 0;
            Exception lastException = null;

            while (retryCount < MaxRetries)
            {
                try
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new HttpRequestException($"HTTP Error: {response.StatusCode}");
                    }
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
                {
                    lastException = ex;
                    retryCount++;
                    Console.WriteLine($"Ошибка запроса (попытка {retryCount}/{MaxRetries}): {ex.Message}");

                    if (retryCount < MaxRetries)
                    {
                        int delayMs = BaseDelayMs * (int)Math.Pow(2, retryCount - 1); // экспоненциальная задержка
                        Console.WriteLine($"Повтор через {delayMs} мс...");
                        await Task.Delay(delayMs);
                    }
                }
            }

            throw lastException ?? new InvalidOperationException("Неизвестная ошибка при выполнении HTTP-запроса.");
        }
    }
}