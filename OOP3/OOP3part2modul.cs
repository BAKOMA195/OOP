using System;
using System.IO; // Для работы с файлами (альтернатива Properties.Settings)
using System.Windows.Forms; // Для Application.UserAppDataPath

namespace OOP3part2
{
    public class ValuesModel
    {
        //Константы
        public const int MIN_VALUE = 0;
        public const int MAX_VALUE = 100;
        private const string SAVE_FILE_NAME = "values_state.txt"; // Имя файла для сохранения

        //Приватные поля для хранения значений
        private int _a;
        private int _b;
        private int _c;

        //Публичные свойства для доступа (только чтение)
        public int A => _a;
        public int B => _b;
        public int C => _c;

        //Событие для уведомления об изменениях
        public event EventHandler ValuesChanged;

        //Конструктор
        public ValuesModel()
        {
            // Загрузка значений при создании объекта модели
            LoadValues();
        }

        //Методы для изменения значений (контролируемые моделью)

        public void SetA(int newValue)
        {
            // 1. Ограничиваем новое значение A пределами [MIN_VALUE, MAX_VALUE]
            newValue = Clamp(newValue, MIN_VALUE, MAX_VALUE);

            // 2. Сохраняем текущие значения для проверки изменений
            int oldA = _a;
            int oldB = _b;
            int oldC = _c;

            // 3. Применяем новое значение A
            int targetA = newValue;
            int targetB = _b;
            int targetC = _c;

            // 4. Разрешающее поведение: корректируем B и C, если нужно
            if (targetA > targetB)
            {
                targetB = targetA; // B не может быть меньше A
            }
            if (targetB > targetC)
            {
                targetC = targetB; // C не может быть меньше B (который мог измениться)
            }

            // 5. Окончательно ограничиваем B и C (на случай, если они вышли за MAX_VALUE)
            targetB = Clamp(targetB, MIN_VALUE, MAX_VALUE);
            targetC = Clamp(targetC, MIN_VALUE, MAX_VALUE);

            // 6. Обновляем внутренние значения и уведомляем, если что-то изменилось
            UpdateInternalValues(targetA, targetB, targetC, oldA, oldB, oldC);
        }

        public void SetB(int newValue)
        {
            // 1. Сохраняем текущие значения для проверки изменений
            int oldA = _a;
            int oldB = _b;
            int oldC = _c;

            // 2. Ограничивающее поведение: корректируем newValue, чтобы оно было в пределах [A, C] и [MIN_VALUE, MAX_VALUE]
            int targetB = Clamp(newValue, _a, _c); // Сначала в пределах [A, C]
            targetB = Clamp(targetB, MIN_VALUE, MAX_VALUE); // Затем в общих пределах

            // 3. Обновляем внутренние значения (A и C не меняются) и уведомляем, если B изменилось
            UpdateInternalValues(_a, targetB, _c, oldA, oldB, oldC);
        }

        public void SetC(int newValue)
        {
            // 1. Ограничиваем новое значение C пределами [MIN_VALUE, MAX_VALUE]
            newValue = Clamp(newValue, MIN_VALUE, MAX_VALUE);

            // 2. Сохраняем текущие значения для проверки изменений
            int oldA = _a;
            int oldB = _b;
            int oldC = _c;

            // 3. Применяем новое значение C
            int targetA = _a;
            int targetB = _b;
            int targetC = newValue;

            // 4. Разрешающее поведение: корректируем B и A, если нужно
            if (targetC < targetB)
            {
                targetB = targetC; // B не может быть больше C
            }
            if (targetB < targetA)
            {
                targetA = targetB; // A не может быть больше B (который мог измениться)
            }

            // 5. Окончательно ограничиваем A и B (на случай, если они вышли за MIN_VALUE)
            targetA = Clamp(targetA, MIN_VALUE, MAX_VALUE);
            targetB = Clamp(targetB, MIN_VALUE, MAX_VALUE);

            // 6. Обновляем внутренние значения и уведомляем, если что-то изменилось
            UpdateInternalValues(targetA, targetB, targetC, oldA, oldB, oldC);
        }


        //Вспомогательные методы

        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void UpdateInternalValues(int newA, int newB, int newC, int oldA, int oldB, int oldC)
        {
            // Проверяем, изменилось ли хотя бы одно значение
            if (_a != newA || _b != newB || _c != newC)
            {
                _a = newA;
                _b = newB;
                _c = newC;
                // Уведомляем подписчиков ТОЛЬКО ЕСЛИ были реальные изменения
                OnValuesChanged();
            }
            // Важно: Если значения не изменились (например, при попытке ввести для B недопустимое значение),
            // событие не вызывается, и UI не будет обновляться без необходимости.
        }

        protected virtual void OnValuesChanged()
        {
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }


        //Сохранение и загрузка

        private string GetSaveFilePath()
        {
            // Используем папку данных приложения пользователя для хранения настроек
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Можно добавить подпапку для вашего приложения
            string appFolderPath = Path.Combine(appDataPath, "MySimpleMVCApp"); // Назовите как хотите
            Directory.CreateDirectory(appFolderPath); // Создаем папку, если её нет
            return Path.Combine(appFolderPath, SAVE_FILE_NAME);
        }

        public void SaveValues()
        {
            try
            {
                string filePath = GetSaveFilePath();
                // Сохраняем значения в текстовый файл, каждое на новой строке
                File.WriteAllLines(filePath, new string[] {
                    _a.ToString(),
                    _b.ToString(),
                    _c.ToString()
                });
            }
            catch (Exception ex)
            {
                // Обработка ошибок сохранения (например, вывести сообщение)
                Console.WriteLine($"Ошибка сохранения значений: {ex.Message}");
                // В реальном приложении можно показать MessageBox
                // MessageBox.Show($"Не удалось сохранить значения: {ex.Message}", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadValues()
        {
            // Значения по умолчанию, если файл не найден или поврежден
            int loadedA = 0;
            int loadedB = 1; // Начальные значения, удовлетворяющие A <= B <= C
            int loadedC = 2;

            string filePath = GetSaveFilePath();

            if (File.Exists(filePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(filePath);
                    if (lines.Length >= 3 &&
                        int.TryParse(lines[0], out int tempA) &&
                        int.TryParse(lines[1], out int tempB) &&
                        int.TryParse(lines[2], out int tempC))
                    {
                        loadedA = tempA;
                        loadedB = tempB;
                        loadedC = tempC;
                    }
                    else
                    {
                        Console.WriteLine($"Предупреждение: Файл '{filePath}' поврежден или имеет неверный формат. Используются значения по умолчанию.");
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок чтения
                    Console.WriteLine($"Ошибка загрузки значений: {ex.Message}. Используются значения по умолчанию.");
                    // Можно показать MessageBox
                    // MessageBox.Show($"Не удалось загрузить сохраненные значения: {ex.Message}\nБудут использованы значения по умолчанию.", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            //Важно: Применяем начальные значения через внутренний метод,
            // чтобы гарантировать корректность и ограничения при первой загрузке

            // 1. Ограничиваем загруженные значения
            loadedA = Clamp(loadedA, MIN_VALUE, MAX_VALUE);
            loadedB = Clamp(loadedB, MIN_VALUE, MAX_VALUE);
            loadedC = Clamp(loadedC, MIN_VALUE, MAX_VALUE);

            // 2. Гарантируем порядок A <= B <= C при загрузке
            // Используем логику, похожую на SetA и SetC, но без вызова события на каждом шаге
            if (loadedA > loadedB) loadedB = loadedA;
            if (loadedB > loadedC) loadedC = loadedB;

            // 3. Устанавливаем внутренние поля НАПРЯМУЮ (без вызова SetA/B/C)
            _a = loadedA;
            _b = loadedB;
            _c = loadedC;

            // 4. Вызываем ОДНО уведомление ПОСЛЕ загрузки и проверки всех значений
            OnValuesChanged();
        }
    }
}
