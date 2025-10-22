using System.Net.Http;

namespace LabWork4
{
    public class HttpRetry
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const int MaxRetries = 3;
        private const int BaseDelayMs = 1000;

        public static async void Task1()
        {
            string url = " https://www.arcotel.ru/";

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
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    retryCount++;
                    Console.WriteLine($"Ошибка запроса (попытка {retryCount}/{MaxRetries}): {ex.Message}");

                    if (retryCount < MaxRetries)
                    {
                        int delayMs = BaseDelayMs * (int)Math.Pow(2, retryCount - 1);
                        Console.WriteLine($"Повтор через {delayMs}мс...");
                        await Task.Delay(delayMs);
                    }
                }
            }

            throw lastException;
        }
    }
}
