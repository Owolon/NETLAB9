namespace NET_LAB9W
{
    public partial class Form1 : Form
    {
        private string filePath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Нет активного файла для экспорта!");
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Бинарные файлы (*.bin)|*.bin";
                sfd.FileName = Path.GetFileName(filePath);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(filePath, sfd.FileName, true);
                    MessageBox.Show("Файл успешно экспортирован!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта: " + ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Бинарные файлы (*.bin)|*.bin|Все файлы (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string projectDir = GetProjectDirectory();
                    string fileName = Path.GetFileName(ofd.FileName);
                    string newPath = Path.Combine(projectDir, fileName);

                    if (File.Exists(newPath))
                        File.Delete(newPath);

                    using (FileStream source = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (FileStream dest = new FileStream(newPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        source.CopyTo(dest);
                    }

                    filePath = newPath;
                    textBox2.Text = Path.GetFileNameWithoutExtension(fileName);
                    textBox5.Text = "Файл: " + filePath;

                    MessageBox.Show("Файл успешно импортирован и установлен как активный!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка импорта: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Файл не найден!");
                    return;
                }

                textBox4.Clear();
                List<double> numbers = new List<double>();

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        numbers.Add(reader.ReadDouble());
                    }
                }

                for (int i = 0; i < numbers.Count; i++)
                    textBox4.AppendText($"{i}: {numbers[i]}\r\n");

                MessageBox.Show("Чтение завершено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(textBox1.Text, out int count) || count <= 0)
                {
                    MessageBox.Show("Введите корректное количество чисел.");
                    return;
                }

                string fileName = textBox2.Text.Trim();
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    MessageBox.Show("Введите имя файла!");
                    return;
                }

                fileName += ".bin";

                string projectDir = GetProjectDirectory();
                filePath = Path.Combine(projectDir, fileName);

                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    for (int i = 0; i < count; i++)
                    {
                        double value = rnd.NextDouble() * rnd.Next(-5000, 5000) * Math.Sin(rnd.NextDouble());
                        value = Math.Round(value, 2);
                        writer.Write(value);
                    }
                }

                textBox5.Text = "Файл: " + filePath;
                MessageBox.Show("Файл успешно создан!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

        }
        private string GetProjectDirectory()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo di = new DirectoryInfo(dir);

            while (di != null && di.GetFiles("*.csproj").Length == 0)
                di = di.Parent;

            return di?.FullName ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Сначала создайте файл!");
                    return;
                }

                if (!double.TryParse(textBox3.Text, out double number))
                {
                    MessageBox.Show("Введите корректное число.");
                    return;
                }

                List<double> numbers = new List<double>();

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                        numbers.Add(reader.ReadDouble());
                }

                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                int pos = rnd.Next(0, numbers.Count);

                numbers.Insert(pos, number);

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    foreach (double n in numbers)
                        writer.Write(n);
                }

                MessageBox.Show($"Число {number} вставлено в позицию {pos}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string inputName = textBox2.Text.Trim();

                if (string.IsNullOrWhiteSpace(inputName))
                {
                    MessageBox.Show("Введите новое имя файла!");
                    return;
                }

                if (!inputName.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
                    inputName += ".bin";

                string projectDir = GetProjectDirectory();
                string oldFullPath = filePath;
                string newFullPath = Path.Combine(projectDir, inputName);

                if (string.Equals(Path.GetFileName(oldFullPath), inputName, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Имя файла уже установлено: {inputName}\nПуть: {oldFullPath}");
                    return;
                }

                if (File.Exists(oldFullPath))
                {
                    DialogResult renameResult = MessageBox.Show(
                        $"Файл с текущим именем найден:\n{oldFullPath}\n\nПереименовать текущий файл в новое имя?",
                        "Подтверждение",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (renameResult == DialogResult.Yes)
                    {
                        if (File.Exists(newFullPath))
                        {
                            DialogResult overwriteResult = MessageBox.Show(
                                $"Файл с именем {inputName} уже существует. Перезаписать?",
                                "Подтверждение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                            if (overwriteResult == DialogResult.Yes)
                                File.Delete(newFullPath);
                            else
                                return;
                        }

                        File.Move(oldFullPath, newFullPath);
                        filePath = newFullPath;
                        MessageBox.Show($"Файл переименован.\nНовое имя: {inputName}\nПуть: {filePath}");
                    }
                    else
                    {
                        string oldName = Path.GetFileName(filePath);
                        filePath = newFullPath;
                        textBox5.Text = "Файл: " + filePath;
                        MessageBox.Show($"Имя файла изменено (без переименования старого файла):\n{oldName} → {inputName}");
                    }
                }
                else
                {
                    filePath = newFullPath;
                    textBox5.Text = "Файл: " + filePath;
                    MessageBox.Show($"Имя файла установлено: {inputName}\nПуть: {filePath}");
                }

                textBox2.Text = Path.GetFileNameWithoutExtension(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения имени файла: " + ex.Message);
            }
        }

        private void ShowFileInfo()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Файл не найден!");
                    return;
                }

                FileInfo info = new FileInfo(filePath);

                string fileDetails =
                    $"Имя: {info.Name}\r\n" +
                    $"Путь: {info.FullName}\r\n" +
                    $"Размер: {info.Length} байт\r\n" +
                    $"Создан: {info.CreationTime}\r\n" +
                    $"Последний доступ: {info.LastAccessTime}\r\n" +
                    $"Последняя запись: {info.LastWriteTime}\r\n";

                MessageBox.Show(fileDetails, "Информация о файле");
                string infoPath = Path.Combine(GetProjectDirectory(), "file_info.txt");

                File.WriteAllText(infoPath, fileDetails);

                MessageBox.Show($"Информация о файле сохранена в:\n{infoPath}", "Сохранено");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShowFileInfo();
        }

        private void ResetAll()
        {
            try
            {
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();

                filePath = "";

                MessageBox.Show("Все поля и активный файл сброшены.", "Сброс выполнен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сбросе: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ResetAll();
        }
    }
}
