using System; // Подключение пространства имен для базовых типов и функционала .NET
using System.Collections.Generic; // Для работы с коллекциями, такими как List
using System.Drawing; // Для работы с графикой (например, отрисовка фигур)
using System.Linq; // Для использования LINQ-запросов
using System.Windows.Forms; // Для работы с формами и элементами управления WinForms

namespace OOP3._1 // Определение пространства имен для проекта
{
    public partial class Form1 : Form // Главная форма приложения
    {
        private MyStorage storage; // Хранилище всех объектов на форме (кругов)

        // Флаг: true = выделять все круги под курсором; false = только один
        private bool selectMultipleInOverlap = true; // ← Можешь поменять на false по указанию преподавателя

        public Form1() // Конструктор формы
        {
            InitializeComponent();       // Инициализация компонентов WinForms (автоматически созданных)
            this.DoubleBuffered = true;  // Включаем двойную буферизацию, чтобы избежать мерцания при перерисовке
            this.KeyPreview = true;      // Разрешаем форму перехватывать события клавиш (например, Del)
            storage = new MyStorage();   // Создаём контейнер для хранения кругов

            // Подписываемся на события формы
            this.MouseDown += Form1_MouseDown; // Обработка нажатия мыши
            this.Paint += Form1_Paint;         // Обработка перерисовки формы
            this.KeyDown += Form1_KeyDown;     // Обработка нажатия клавиш
            this.Resize += (s, e) => this.Invalidate(); // Перерисовка при изменении размера формы
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Пустой метод загрузки формы (можно добавить дополнительную логику при необходимости)
        }

        // Обработка нажатия мыши
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Проверяем, что нажата левая кнопка мыши
            {
                bool ctrl = (ModifierKeys & Keys.Control) == Keys.Control; // Проверяем, нажат ли Ctrl

                if (selectMultipleInOverlap) // Если включен режим множественного выделения
                {
                    bool hitAny = false; // Флаг, указывающий, был ли найден хотя бы один круг под курсором
                    foreach (var shape in storage.All()) // Проходим по всем фигурам в хранилище
                    {
                        if (shape.HitTest(e.Location)) // Проверяем, находится ли курсор над фигурой
                        {
                            if (!ctrl)
                                storage.ClearSelection(); // Если Ctrl не нажат, сбрасываем выделение у всех фигур
                            shape.ToggleSelect(ctrl);     // Переключаем выделение текущей фигуры
                            hitAny = true;               // Устанавливаем флаг, что найден хотя бы один круг
                        }
                    }

                    // Если ни один круг не попал под курсор — создаём новый
                    if (!hitAny)
                    {
                        if (!ctrl)
                            storage.ClearSelection(); // Если Ctrl не нажат, сбрасываем выделение
                        storage.Add(new CCircle(e.Location)); // Добавляем новый круг в хранилище
                    }
                }
                else // Если включен режим одиночного выделения
                {
                    storage.SelectAt(e.Location, ctrl); // Выбираем первый круг под курсором

                    // Если ни один круг не попал под курсор — создаём новый
                    if (!storage.HitAny(e.Location))
                        storage.Add(new CCircle(e.Location)); // Добавляем новый круг в хранилище
                }

                this.Invalidate(); // Перерисовываем форму для обновления отображения
            }
        }

        // Отрисовка всех объектов на форме
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            storage.DrawAll(e.Graphics); // Рисуем все круги из хранилища
        }

        // Обработка нажатия клавиш
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) // Если нажата клавиша Delete
            {
                storage.RemoveSelected(); // Удаляем все выделенные круги из хранилища
                this.Invalidate();        // Перерисовываем форму для обновления отображения
            }
        }
    }

    // Абстрактный класс для всех фигур
    public abstract class Shape
    {
        public abstract void Draw(Graphics g);               // Метод для отрисовки фигуры
        public abstract bool HitTest(Point p);               // Метод для проверки попадания точки в фигуру
        public abstract void ToggleSelect(bool multi);       // Метод для переключения выделения
        public abstract bool IsSelected { get; }             // Свойство для проверки, выделена ли фигура
    }

    // Класс круга
    public class CCircle : Shape
    {
        private const int Radius = 30;       // Радиус круга
        private Point center;                // Центр круга
        private bool selected = false;       // Флаг, указывающий, выделен ли круг

        public CCircle(Point center) // Конструктор круга
        {
            this.center = center;           // Устанавливаем центр круга
        }

        public override void Draw(Graphics g) // Отрисовка круга
        {
            Rectangle rect = new Rectangle(center.X - Radius, center.Y - Radius, Radius * 2, Radius * 2); // Прямоугольник для круга
            Pen pen = selected ? Pens.Red : Pens.Black; // Определяем цвет пера (красный для выделенных, чёрный для остальных)

            g.DrawEllipse(pen, rect); // Рисуем окружность

            if (selected) // Если круг выделен, закрашиваем его полупрозрачным красным цветом
                g.FillEllipse(new SolidBrush(Color.FromArgb(50, Color.Red)), rect);
        }

        public override bool HitTest(Point p) // Проверка попадания точки в круг
        {
            int dx = p.X - center.X; // Разница по оси X между точкой и центром круга
            int dy = p.Y - center.Y; // Разница по оси Y между точкой и центром круга
            return dx * dx + dy * dy <= Radius * Radius; // Проверяем по формуле расстояния до центра
        }

        public override void ToggleSelect(bool multi) // Переключение выделения
        {
            if (!multi)
                selected = true;     // Без Ctrl — всегда выделяем
            else
                selected = !selected; // С Ctrl — инвертируем выделение
        }

        public void Deselect() // Сброс выделения
        {
            selected = false; // Устанавливаем флаг выделения в false
        }

        public override bool IsSelected => selected; // Свойство для проверки выделения
    }

    // Кастомный контейнер фигур
    public class MyStorage
    {
        private List<Shape> shapes = new List<Shape>(); // Список для хранения фигур

        public void Add(Shape shape) => shapes.Add(shape); // Добавление новой фигуры в список

        public void RemoveSelected() // Удаление всех выделенных фигур
        {
            shapes = shapes.Where(s => !s.IsSelected).ToList(); // Оставляем только невыделенные фигуры
        }

        public void ClearSelection() // Снятие выделения со всех фигур
        {
            foreach (var shape in shapes)
                if (shape is CCircle circle)
                    circle.Deselect(); // Сбрасываем выделение для каждого круга
        }

        public void SelectAt(Point p, bool ctrl) // Выбор фигуры под курсором
        {
            bool anySelected = false; // Флаг, указывающий, была ли выбрана хотя бы одна фигура

            foreach (var shape in shapes) // Проходим по всем фигурам
            {
                if (shape.HitTest(p)) // Если курсор над фигурой
                {
                    if (!ctrl)
                        ClearSelection(); // Если Ctrl не нажат, сбрасываем выделение у всех фигур
                    shape.ToggleSelect(ctrl); // Переключаем выделение текущей фигуры
                    anySelected = true; // Устанавливаем флаг, что фигура выбрана
                    break; // Выбираем только одну фигуру
                }
            }

            if (!anySelected && !ctrl)
                ClearSelection(); // Если ни одна фигура не выбрана и Ctrl не нажат, сбрасываем выделение
        }

        public bool HitAny(Point p) // Проверка, есть ли хоть одна фигура под курсором
        {
            return shapes.Any(s => s.HitTest(p)); // Возвращаем true, если найдена хотя бы одна фигура
        }

        public void DrawAll(Graphics g) // Отрисовка всех фигур
        {
            foreach (var shape in shapes)
                shape.Draw(g); // Рисуем каждую фигуру
        }

        public IEnumerable<Shape> All() // Получение всех фигур из хранилища
        {
            return shapes; // Возвращаем список фигур
        }
    }
}
