using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static string directory = AppDomain.CurrentDomain.BaseDirectory;
    static string fileName = "numbers.bin";
    static string fullPath => Path.Combine(directory, fileName);

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Файлы сохраняются в папку проекта:");
        Console.WriteLine(directory);

        int choice;
        do
        {
            Console.WriteLine("1. Задать имя файла");
            Console.WriteLine("2. Посмотреть содержимое бинарного файла");
            Console.WriteLine("3. Создать бинарный файл");
            Console.WriteLine("4. Найти последнее число");
            Console.WriteLine("5. Вставить число (сложный рандом)");
            Console.WriteLine("6. Копировать файл");
            Console.WriteLine("7. Информация о файле");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");

            choice = SafeIntInput();

            switch (choice)
            {
                case 1: SetFileName(); break;
                case 2: ShowBinaryContent(); break;
                case 3: CreateBinaryFile(); break;
                case 4: GetLastNumber(); break;
                case 5: InsertRandom(); break;
                case 6: CopyFile(); break;
                case 7: FileInfo(); break;
            }

        } while (choice != 0);
    }

    static int SafeIntInput()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out int value))
                return value;

            Console.Write("Ошибка! Введите целое число: ");
        }
    }

    static double SafeDoubleInput()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (double.TryParse(input, out double value))
                return value;

            Console.Write("Ошибка! Введите действительное число: ");
        }
    }

    static string SafeFileNameInput()
    {
        while (true)
        {
            string name = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(name))
            {
                Console.Write("Имя файла не может быть пустым, попробуйте снова: ");
                continue;
            }

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                Console.Write("Имя содержит запрещённые символы, попробуйте снова: ");
                continue;
            }

            return name;
        }
    }

    static void SetFileName()
    {
        Console.Write("Введите новое имя файла (например: data.bin или data): ");
        string inputName = SafeFileNameInput();

        // добавляем расширение .bin если пользователь его не ввёл
        if (!inputName.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
            inputName += ".bin";

        string oldFull = fullPath; // путь до изменения (основан на старом fileName)
        string newFull = Path.Combine(directory, inputName);

        // если уже совпадает — просто сообщаем
        if (string.Equals(fileName, inputName, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Имя файла уже установлено: {fileName}");
            Console.WriteLine($"Полный путь: {fullPath}");
            return;
        }

        // если старый файл существует, предложим переименовать его
        if (File.Exists(oldFull))
        {
            Console.WriteLine($"Найден существующий файл с текущим именем:\n{oldFull}");
            Console.Write("Переименовать текущий файл в новое имя? (y/n): ");
            string ans = Console.ReadLine().Trim().ToLower();
            if (ans == "y" || ans == "yes")
            {
                try
                {
                    // если целевой файл уже существует — спросим, перезаписать ли его
                    if (File.Exists(newFull))
                    {
                        Console.WriteLine($"Файл с именем {inputName} уже существует.");
                        Console.Write("Перезаписать существующий файл? (y/n): ");
                        string ans2 = Console.ReadLine().Trim().ToLower();
                        if (ans2 == "y" || ans2 == "yes")
                        {
                            File.Delete(newFull);
                        }
                        else
                        {
                            Console.WriteLine("Переименование отменено. Имя не изменено.");
                            return;
                        }
                    }

                    File.Move(oldFull, newFull);
                    fileName = inputName;
                    Console.WriteLine($"Файл переименован. Новое имя: {fileName}");
                    Console.WriteLine($"Путь к файлу: {fullPath}");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка переименования файла: {ex.Message}");
                    Console.WriteLine("Имя не изменено.");
                    return;
                }
            }
            else
            {
                // пользователь не хочет переименовывать файл, но всё ещё может изменить имя переменной
                fileName = inputName;
                Console.WriteLine($"Имя файла изменено (без переименования старого файла): {fileName}");
                Console.WriteLine($"Теперь методы будут искать файл: {fullPath}");
                return;
            }
        }
        else
        {
            // старого файла нет — просто меняем имя
            fileName = inputName;
            Console.WriteLine($"Имя файла установлено: {fileName}");
            Console.WriteLine($"Полный путь: {fullPath}");
        }
    }

    static void CreateBinaryFile()
    {
        Console.Write("Введите количество чисел: ");
        int n = SafeIntInput();

        while (n <= 0)
        {
            Console.Write("Количество должно быть > 0: ");
            n = SafeIntInput();
        }

        double[] numbers = new double[n];

        Console.WriteLine("Введите числа:");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"[{i + 1}] = ");
            numbers[i] = SafeDoubleInput();
        }

        try
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
            {
                foreach (double d in numbers)
                    bw.Write(d);
            }

            Console.WriteLine($"\nФайл успешно создан:");
            Console.WriteLine(fullPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи файла: {ex.Message}");
        }
    }

    static void GetLastNumber()
    {
        if (!CheckFileExists()) return;

        try
        {
            using (BinaryReader br = new BinaryReader(File.Open(fullPath, FileMode.Open)))
            {
                if (br.BaseStream.Length < sizeof(double))
                {
                    Console.WriteLine("Файл пуст.");
                    return;
                }

                br.BaseStream.Seek(-sizeof(double), SeekOrigin.End);

                double last = br.ReadDouble();
                Console.WriteLine("Последнее число: " + last);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения файла: {ex.Message}");
        }
    }

    static void InsertRandom()
    {
        if (!CheckFileExists()) return;

        List<double> nums = new List<double>();

        try
        {
            using (BinaryReader br = new BinaryReader(File.Open(fullPath, FileMode.Open)))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                    nums.Add(br.ReadDouble());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка чтения: " + ex.Message);
            return;
        }

        if (nums.Count == 0)
        {
            Console.WriteLine("В файле нет чисел.");
            return;
        }

        Console.Write("Введите число для вставки: ");
        double newNum = SafeDoubleInput();

        Random rnd = new Random();

        int baseIndex = rnd.Next(nums.Count + 1);
        int offset = rnd.Next(-2, 3);
        int chaos = (int)(rnd.NextDouble() * rnd.Next(-5, 6));

        int pos = Math.Clamp(baseIndex + offset + chaos, 0, nums.Count);

        nums.Insert(pos, newNum);

        try
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
            {
                foreach (double d in nums)
                    bw.Write(d);
            }

            Console.WriteLine($"Число вставлено в позицию {pos}");
            Console.WriteLine($"Файл обновлён: {fullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи файла: {ex.Message}");
        }
    }

    static void CopyFile()
    {
        if (!CheckFileExists()) return;

        Console.Write("Введите имя копии файла: ");
        string newName = SafeFileNameInput();

        // добавляем .bin если нужно
        if (!newName.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
            newName += ".bin";

        string newPath = Path.Combine(directory, newName);

        try
        {
            // если целевой файл существует — спросим перезаписать
            if (File.Exists(newPath))
            {
                Console.WriteLine($"Файл {newName} уже существует.");
                Console.Write("Перезаписать его? (y/n): ");
                string ans = Console.ReadLine().Trim().ToLower();
                if (!(ans == "y" || ans == "yes"))
                {
                    Console.WriteLine("Копирование отменено.");
                    return;
                }
            }

            File.Copy(fullPath, newPath, true);
            Console.WriteLine($"Файл успешно скопирован:\n{newPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка копирования: {ex.Message}");
        }
    }

    static void FileInfo()
    {
        if (!CheckFileExists()) return;

        try
        {
            FileInfo info = new FileInfo(fullPath);

            Console.WriteLine("\nИнформация о файле:");
            Console.WriteLine($"Имя: {info.Name}");
            Console.WriteLine($"Путь: {info.FullName}");
            Console.WriteLine($"Размер: {info.Length} байт");
            Console.WriteLine($"Создан: {info.CreationTime}");
            Console.WriteLine($"Последний доступ: {info.LastAccessTime}");
            Console.WriteLine($"Последняя запись: {info.LastWriteTime}");

            string infoPath = Path.Combine(directory, "file_info.txt");

            File.WriteAllText(infoPath,
                $"Имя: {info.Name}\n" +
                $"Путь: {info.FullName}\n" +
                $"Размер: {info.Length} байт\n" +
                $"Создан: {info.CreationTime}\n" +
                $"Последний доступ: {info.LastAccessTime}\n" +
                $"Последняя запись: {info.LastWriteTime}\n");

            Console.WriteLine("\nИнформация сохранена:");
            Console.WriteLine(infoPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка получения данных: " + ex.Message);
        }
    }

    static bool CheckFileExists()
    {
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"Файл не найден:\n{fullPath}");
            return false;
        }
        return true;
    }

    static void ShowBinaryContent()
    {
        try
        {
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("Файл не найден!");
                return;
            }

            List<double> numbers = new List<double>();

            using (BinaryReader reader = new BinaryReader(File.Open(fullPath, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                    numbers.Add(reader.ReadDouble());
            }

            Console.WriteLine("\n===== Содержимое файла =====");

            for (int i = 0; i < numbers.Count; i++)
                Console.WriteLine($"{i}: {numbers[i]}");

            Console.WriteLine("\n===== Конец файла =====");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка чтения файла: " + ex.Message);
        }
    }
}
