// Файл: Form1.cs

using System;
using System.Globalization; // Для NumberStyles при парсинге
using System.Windows.Forms;

namespace OOP3part2 // Убедитесь, что пространство имен совпадает с вашим проектом
{
    public partial class Form1 : Form
    {
        // --- Модель данных ---
        private readonly ValuesModel model;

        // --- Флаг для предотвращения рекурсивных обновлений UI ---
        private bool isUpdatingUI = false;

        public Form1()
        {
            InitializeComponent();

            // --- Инициализация Модели ---
            model = new ValuesModel(); // Модель создается и сама загружает данные

            // --- Настройка контролов (пределы) ---
            InitializeControls();

            // --- Подписка на событие изменения модели ---
            model.ValuesChanged += Model_ValuesChanged;

            // --- Подписка контролов на события ---
            // TextBox (используем событие Leave для валидации и обновления модели)
            textBox1.Leave += TextBox_Leave;
            textBox2.Leave += TextBox_Leave;
            textBox3.Leave += TextBox_Leave;

            // NumericUpDown
            numericUpDown1.ValueChanged += NumericUpDown_ValueChanged;
            numericUpDown2.ValueChanged += NumericUpDown_ValueChanged;
            numericUpDown3.ValueChanged += NumericUpDown_ValueChanged;

            // TrackBar
            trackBar1.Scroll += TrackBar_Scroll;
            trackBar2.Scroll += TrackBar_Scroll;
            trackBar3.Scroll += TrackBar_Scroll;

            // --- Первичное отображение данных из модели ---
            // Вызываем обновление UI после подписки на события модели и настройки контролов.
            UpdateUIFromModel();

            // --- Сохранение при закрытии ---
            this.FormClosing += Form1_FormClosing;
        }

        private void InitializeControls()
        {
            // NumericUpDown
            numericUpDown1.Minimum = ValuesModel.MIN_VALUE;
            numericUpDown1.Maximum = ValuesModel.MAX_VALUE;
            numericUpDown2.Minimum = ValuesModel.MIN_VALUE;
            numericUpDown2.Maximum = ValuesModel.MAX_VALUE;
            numericUpDown3.Minimum = ValuesModel.MIN_VALUE;
            numericUpDown3.Maximum = ValuesModel.MAX_VALUE;

            // TrackBar
            trackBar1.Minimum = ValuesModel.MIN_VALUE;
            trackBar1.Maximum = ValuesModel.MAX_VALUE;
            trackBar2.Minimum = ValuesModel.MIN_VALUE;
            trackBar2.Maximum = ValuesModel.MAX_VALUE;
            trackBar3.Minimum = ValuesModel.MIN_VALUE;
            trackBar3.Maximum = ValuesModel.MAX_VALUE;
        }


        private void Model_ValuesChanged(object sender, EventArgs e)
        {
            // Проверяем, нужно ли вызывать обновление (избегаем лишних вызовов, если UI уже обновляется)
            if (!isUpdatingUI)
            {
                UpdateUIFromModel();
            }
        }

        private void UpdateUIFromModel()
        {
            // Устанавливаем флаг, чтобы избежать зацикливания событий
            isUpdatingUI = true;

            try // Используем try-finally для гарантии сброса флага
            {
                // --- Обновление контролов для A ---
                // Обновляем только если значение реально отличается, чтобы избежать мерцания/лишних событий
                if (textBox1.Text != model.A.ToString())
                    textBox1.Text = model.A.ToString();
                if (numericUpDown1.Value != model.A)
                    numericUpDown1.Value = model.A;
                if (trackBar1.Value != model.A)
                    trackBar1.Value = model.A;

                // --- Обновление контролов для B ---
                if (textBox2.Text != model.B.ToString())
                    textBox2.Text = model.B.ToString();
                if (numericUpDown2.Value != model.B)
                    numericUpDown2.Value = model.B;
                if (trackBar2.Value != model.B)
                    trackBar2.Value = model.B;

                // --- Обновление контролов для C ---
                if (textBox3.Text != model.C.ToString())
                    textBox3.Text = model.C.ToString();
                if (numericUpDown3.Value != model.C)
                    numericUpDown3.Value = model.C;
                if (trackBar3.Value != model.C)
                    trackBar3.Value = model.C;
            }
            finally
            {
                // Сбрасываем флаг ПОСЛЕ обновления всех контролов
                isUpdatingUI = false;
            }
        }


        // --- Обработчики событий от контролов ---

        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (isUpdatingUI) return; // Игнорируем, если UI обновляется программно

            TextBox tb = sender as TextBox;
            if (tb == null) return;

            // Пытаемся распарсить значение
            if (int.TryParse(tb.Text, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int value))
            {
                // Успешно распарсили, передаем значение в модель
                try
                {
                    if (tb == textBox1)
                    {
                        model.SetA(value);
                        // Для A и C модель сама вызовет ValuesChanged, если нужно обновить другие контролы
                    }
                    else if (tb == textBox2)
                    {
                        model.SetB(value);
                        // ---> ИСПРАВЛЕНО: Принудительно обновить UI ПОСЛЕ попытки изменить B <---
                        // Это необходимо, чтобы TextBox немедленно отобразил результат
                        // ограничивающего поведения модели (если B было скорректировано до A или C).
                        UpdateUIFromModel();
                    }
                    else if (tb == textBox3)
                    {
                        model.SetC(value);
                        // Для A и C модель сама вызовет ValuesChanged, если нужно обновить другие контролы
                    }
                    // Если модель не вызвала ValuesChanged (значение не изменилось или было ограничено до текущего),
                    // то для B мы все равно обновили UI вызовом UpdateUIFromModel() выше.
                    // Для A и C дополнительное обновление не требуется, т.к. их поведение разрешающее.
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при установке значения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // В случае ошибки синхронизируем UI с текущим состоянием модели
                    UpdateUIFromModel();
                }
            }
            else
            {
                // Ввод некорректен (не число или пусто).
                // Просто игнорируем. TextBox будет обновлен до корректного значения
                // из модели при следующем срабатывании Model_ValuesChanged или
                // принудительном вызове UpdateUIFromModel (например, из другого контрола).
                Console.WriteLine($"Некорректный ввод в {tb.Name}: '{tb.Text}'");
                // Если нужно немедленно вернуть старое значение в TextBox:
                // UpdateUIFromModel();
            }
        }

        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (isUpdatingUI) return; // Игнорируем, если UI обновляется программно

            NumericUpDown nud = sender as NumericUpDown;
            if (nud == null) return;

            int value = (int)nud.Value; // Значение всегда корректно в NumericUpDown

            try
            {
                if (nud == numericUpDown1)
                {
                    model.SetA(value);
                }
                else if (nud == numericUpDown2)
                {
                    model.SetB(value);
                    // ---> ИСПРАВЛЕНО: Принудительно обновить UI ПОСЛЕ попытки изменить B <---
                    UpdateUIFromModel();
                }
                else if (nud == numericUpDown3)
                {
                    model.SetC(value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при установке значения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUIFromModel(); // Синхронизация UI в случае ошибки
            }
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            if (isUpdatingUI) return; // Игнорируем, если UI обновляется программно

            TrackBar trb = sender as TrackBar;
            if (trb == null) return;

            int value = trb.Value; // Значение всегда корректно в TrackBar

            try
            {
                if (trb == trackBar1)
                {
                    model.SetA(value);
                }
                else if (trb == trackBar2)
                {
                    model.SetB(value);
                    // ---> ИСПРАВЛЕНО: Принудительно обновить UI ПОСЛЕ попытки изменить B <---
                    UpdateUIFromModel();
                }
                else if (trb == trackBar3)
                {
                    model.SetC(value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при установке значения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUIFromModel(); // Синхронизация UI в случае ошибки
            }
        }

        // --- Сохранение данных при закрытии ---
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Вызываем метод сохранения в модели
            model.SaveValues();
        }
    }
}
