namespace LabWork4
{
    public class TemperatureAnalyzer
    {
        public static void Task2()
        {
            List<double> smoothData = new List<double>
            {
            20.1, 20.3, 20.5, 20.7, 20.9, 21.1, 21.3, 21.5, 21.7, 21.9,
            22.1, 22.3, 22.5, 22.7, 22.9, 23.1, 23.3, 23.5, 23.7, 23.9
            };

            List<double> anomalousData = new List<double>
            {
            20.1, 20.3, 35.6, 20.7, 20.9, 21.1, 21.3, 8.2, 21.7, 21.9,
            22.1, 40.5, 22.5, 22.7, 22.9, 5.1, 23.3, 23.5, 23.7, 23.9
            };

            Console.WriteLine("=== Плавный набор данных ===");
            TemperatureAnalyzer.AnalyzeDataset(smoothData);

            Console.WriteLine("\n=== Набор с аномалиями ===");
            TemperatureAnalyzer.AnalyzeDataset(anomalousData);
        }



        public static void AnalyzeDataset(List<double> temperatures)
        {
            Console.WriteLine($"Среднее значение температуры:");

            double meanWithOutliers = temperatures.Average();
            Console.WriteLine($"- С учетом выбросов: {meanWithOutliers:F2}°C");

            double meanWithoutOutliers = CalculateMeanWithoutOutliers(temperatures);
            Console.WriteLine($"- Без учета выбросов: {meanWithoutOutliers:F2}°C");

            double difference = Math.Abs(meanWithOutliers - meanWithoutOutliers);
            Console.WriteLine($"Разница: {difference:F2}°C");

            var outliers = FindOutliers(temperatures);
            if (outliers.Any())
            {
                Console.WriteLine($"Обнаружены выбросы: {string.Join(", ", outliers.Select(o => $"{o:F1}°C"))}");
            }
            else
            {
                Console.WriteLine("Выбросы не обнаружены");
            }
        }

        static double CalculateMeanWithoutOutliers(List<double> temperatures)
        {
            if (temperatures.Count == 0) return 0;

            double mean = temperatures.Average();
            double stdDev = CalculateStandardDeviation(temperatures, mean);

            List<double> filtered = temperatures
                .Where(t => Math.Abs(t - mean) <= 2 * stdDev)
                .ToList();

            return filtered.Average();
        }

        static List<double> FindOutliers(List<double> temperatures)
        {
            if (temperatures.Count == 0) return new List<double>();

            double mean = temperatures.Average();
            double stdDev = CalculateStandardDeviation(temperatures, mean);

            return temperatures
                .Where(t => Math.Abs(t - mean) > 2 * stdDev)
                .ToList();
        }

        static double CalculateStandardDeviation(List<double> values, double mean)
        {
            double sumOfSquares = values.Sum(value => Math.Pow(value - mean, 2));
            return Math.Sqrt(sumOfSquares / values.Count);
        }

    }
}
