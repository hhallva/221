using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace LabWork4
{
    class FileStorageApp
    {
        private string mainDirectory;
        private string mirrorDirectory;

        public static void Task3()
        {
            FileStorageApp app = new FileStorageApp();
            app.Run();
        }

        public void Run()
        {
            Console.WriteLine("=== Файловое хранилище с зеркалированием и проверкой чек-сумм ===\n");

            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Выбрать каталоги");
                Console.WriteLine("2. Загрузить файл");
                Console.WriteLine("3. Выгрузить файл");
                Console.WriteLine("4. Проверить целостность файлов");
                Console.WriteLine("5. Выход");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SelectDirectories();
                        break;
                    case "2":
                        UploadFile();
                        break;
                    case "3":
                        DownloadFile();
                        break;
                    case "4":
                        CheckIntegrity();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        private void SelectDirectories()
        {
            Console.Write("Введите путь к основному каталогу: ");
            mainDirectory = Console.ReadLine();

            Console.Write("Введите путь к зеркальному каталогу: ");
            mirrorDirectory = Console.ReadLine();

            try
            {
                // Создаем каталоги если они не существуют
                Directory.CreateDirectory(mainDirectory);
                Directory.CreateDirectory(mirrorDirectory);

                Console.WriteLine("Каталоги успешно выбраны и созданы!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании каталогов: {ex.Message}");
            }
        }

        private void UploadFile()
        {
            if (!DirectoriesSelected())
                return;

            Console.Write("Введите путь к файлу для загрузки: ");
            string sourcePath = Console.ReadLine();

            if (!File.Exists(sourcePath))
            {
                Console.WriteLine("Файл не существует!");
                return;
            }

            string fileName = Path.GetFileName(sourcePath);

            try
            {
                // 5.3.1 Сохраняем файл в оба каталога
                string mainFilePath = Path.Combine(mainDirectory, fileName);
                string mirrorFilePath = Path.Combine(mirrorDirectory, fileName);

                File.Copy(sourcePath, mainFilePath, true);
                File.Copy(sourcePath, mirrorFilePath, true);

                // Создаем файлы чек-сумм
                string checksum = CalculateChecksum(sourcePath);

                string mainChecksumPath = Path.Combine(mainDirectory, fileName + ".checksum");
                string mirrorChecksumPath = Path.Combine(mirrorDirectory, fileName + ".checksum");

                File.WriteAllText(mainChecksumPath, checksum);
                File.WriteAllText(mirrorChecksumPath, checksum);

                Console.WriteLine($"Файл '{fileName}' успешно загружен с чек-суммой: {checksum}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            }
        }

        private void DownloadFile()
        {
            if (!DirectoriesSelected())
                return;

            Console.Write("Введите имя файла для выгрузки: ");
            string fileName = Console.ReadLine();

            string mainFilePath = Path.Combine(mainDirectory, fileName);
            string mirrorFilePath = Path.Combine(mirrorDirectory, fileName);
            string mainChecksumPath = Path.Combine(mainDirectory, fileName + ".checksum");
            string mirrorChecksumPath = Path.Combine(mirrorDirectory, fileName + ".checksum");

            try
            {
                // 5.3.2 Проверяем существование файлов
                if (!File.Exists(mainFilePath) || !File.Exists(mirrorFilePath))
                {
                    Console.WriteLine("Файл не найден в хранилище!");
                    return;
                }

                // Проверяем чек-суммы
                string mainChecksum = File.Exists(mainChecksumPath) ? File.ReadAllText(mainChecksumPath) : null;
                string mirrorChecksum = File.Exists(mirrorChecksumPath) ? File.ReadAllText(mirrorChecksumPath) : null;

                string mainFileChecksum = CalculateChecksum(mainFilePath);
                string mirrorFileChecksum = CalculateChecksum(mirrorFilePath);

                Console.WriteLine("Проверка целостности файлов...");
                Console.WriteLine($"Основной файл чек-сумма: {mainFileChecksum}");
                Console.WriteLine($"Зеркальный файл чек-сумма: {mirrorFileChecksum}");
                Console.WriteLine($"Сохраненная чек-сумма основного: {mainChecksum}");
                Console.WriteLine($"Сохраненная чек-сумма зеркального: {mirrorChecksum}");

                bool mainFileValid = mainFileChecksum == mainChecksum;
                bool mirrorFileValid = mirrorFileChecksum == mirrorChecksum;

                string correctFilePath = null;

                if (mainFileValid && mirrorFileValid)
                {
                    Console.WriteLine("Оба файла корректны! Используем основной файл.");
                    correctFilePath = mainFilePath;
                }
                else if (!mainFileValid && mirrorFileValid)
                {
                    Console.WriteLine("⚠️ Основной файл поврежден! Используем зеркальный файл.");
                    correctFilePath = mirrorFilePath;

                    // Восстанавливаем основной файл из зеркального
                    File.Copy(mirrorFilePath, mainFilePath, true);
                    File.WriteAllText(mainChecksumPath, mirrorChecksum);
                    Console.WriteLine("Основной файл восстановлен из зеркального!");
                }
                else if (mainFileValid && !mirrorFileValid)
                {
                    Console.WriteLine("⚠️ Зеркальный файл поврежден! Используем основной файл.");
                    correctFilePath = mainFilePath;

                    // Восстанавливаем зеркальный файл из основного
                    File.Copy(mainFilePath, mirrorFilePath, true);
                    File.WriteAllText(mirrorChecksumPath, mainChecksum);
                    Console.WriteLine("Зеркальный файл восстановлен из основного!");
                }
                else
                {
                    Console.WriteLine("❌ Оба файла повреждены! Восстановление невозможно.");
                    return;
                }

                // Сохраняем файл
                Console.Write("Введите путь для сохранения файла: ");
                string destinationPath = Console.ReadLine();

                File.Copy(correctFilePath, destinationPath, true);
                Console.WriteLine($"Файл успешно выгружен в: {destinationPath}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выгрузке файла: {ex.Message}");
            }
        }

        private void CheckIntegrity()
        {
            if (!DirectoriesSelected())
                return;

            Console.WriteLine("\n=== Проверка целостности всех файлов ===");

            try
            {
                var mainFiles = Directory.GetFiles(mainDirectory, "*.*")
                    .Where(f => !f.EndsWith(".checksum"))
                    .ToArray();

                foreach (string filePath in mainFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    string mirrorFilePath = Path.Combine(mirrorDirectory, fileName);
                    string mainChecksumPath = filePath + ".checksum";
                    string mirrorChecksumPath = mirrorFilePath + ".checksum";

                    if (!File.Exists(mirrorFilePath))
                    {
                        Console.WriteLine($"❌ {fileName} - зеркальный файл отсутствует");
                        continue;
                    }

                    string mainChecksum = File.Exists(mainChecksumPath) ? File.ReadAllText(mainChecksumPath) : null;
                    string mirrorChecksum = File.Exists(mirrorChecksumPath) ? File.ReadAllText(mirrorChecksumPath) : null;

                    string mainFileChecksum = CalculateChecksum(filePath);
                    string mirrorFileChecksum = CalculateChecksum(mirrorFilePath);

                    bool mainValid = mainFileChecksum == mainChecksum;
                    bool mirrorValid = mirrorFileChecksum == mirrorChecksum;

                    if (mainValid && mirrorValid)
                    {
                        Console.WriteLine($"✅ {fileName} - оба файла корректны");
                    }
                    else if (!mainValid && mirrorValid)
                    {
                        Console.WriteLine($"⚠️ {fileName} - основной файл поврежден, зеркальный OK");
                    }
                    else if (mainValid && !mirrorValid)
                    {
                        Console.WriteLine($"⚠️ {fileName} - зеркальный файл поврежден, основной OK");
                    }
                    else
                    {
                        Console.WriteLine($"❌ {fileName} - оба файла повреждены");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке целостности: {ex.Message}");
            }
        }

        private string CalculateChecksum(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }
        }

        private bool DirectoriesSelected()
        {
            if (string.IsNullOrEmpty(mainDirectory) || string.IsNullOrEmpty(mirrorDirectory))
            {
                Console.WriteLine("Сначала выберите каталоги!");
                return false;
            }
            return true;
        }
    }
}
