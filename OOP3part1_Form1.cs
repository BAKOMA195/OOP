using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOP3._1 // Определение пространства имен для проекта
{
    public partial class Form1 : Form // Главная форма приложения
    {
        private MyStorage storage; // Хранилище всех объектов на форме (кругов)

        public Form1() // Конструктор формы
        {
            InitializeComponent();       
            //this.DoubleBuffered = true;  // Включаем двойную буферизацию, чтобы избежать мерцания при перерисовке
            this.KeyPreview = true;      // Разрешаем форме перехватывать события клавиш до элементов управления (например, для Delete)
            storage = new MyStorage();   // Создаём контейнер для хранения кругов

            // Подписываемся на события формы
            this.MouseDown += Form1_MouseDown; // Обработка нажатия мыши
            this.Paint += Form1_Paint;         // Обработка перерисовки формы
            this.KeyDown += Form1_KeyDown;     // Обработка нажатия клавиш
            this.Resize += (s, e) => this.Invalidate(); // Перерисовка при изменении размера формы для корректного отображения
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // Обработка нажатия мыши
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // Реагируем только на нажатие левой кнопки мыши
            if (e.Button == MouseButtons.Left)
            {
                // Проверяем, зажата ли клавиша Ctrl в момент клика
                bool ctrl = (ModifierKeys & Keys.Control) == Keys.Control;

                // Флаг, показывающий, попали ли мы хотя бы по одному кругу
                bool hitAny = false;

                // Если Ctrl НЕ нажат, то перед выделением новых кругов нужно снять выделение со всех существующих
                if (!ctrl)
                {
                    storage.ClearSelection();
                }

                // Проходим по всем кругам в хранилище.

                foreach (var shape in storage.All()/*.Reverse()*/) // <--- УБРАТЬ ЧТОБЫ ВЫДЕЛИТЬ ВЕРХНИЙ
                {
                    // Проверяем, находится ли точка клика внутри текущего круга
                    if (shape.HitTest(e.Location))
                    {
                        // Если попали, переключаем состояние выделения этого круга
                        // Если Ctrl нажат (ctrl == true), выделение инвертируется
                        // Если Ctrl не нажат (ctrl == false), круг просто выделяется (если не был выделен)
                        shape.ToggleSelect(ctrl);

                        // Устанавливаем флаг, что мы попали хотя бы по одному кругу
                        hitAny = true;


                        break; // <-- УБРАТЬ ЧТОБЫ ВЫДЕЛИТЬ НЕСКОЛЬКО


                    }
                }

                // Если после проверки всех кругов оказалось, что мы не попали ни по одному
                if (!hitAny)
                {
                    // Если Ctrl не был нажат при клике в пустое место, то выделение уже было сброшено ранее.
                    // Если Ctrl был нажат, то существующее выделение сохраняется.
                    // В обоих случаях создаем новый круг в точке клика.
                    storage.Add(new CCircle(e.Location));
                }

                // Запрашиваем перерисовку формы, чтобы отобразить изменения (новое выделение или новый круг)
                this.Invalidate();
            }
        }

        // Отрисовка всех кругов на форме
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            storage.DrawAll(e.Graphics);
        }

        // Обработка нажатия клавиш
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Delete
            if (e.KeyCode == Keys.Delete)
            {
                // Удаляем все выделенные круги из хранилища
                storage.RemoveSelected();
                // Запрашиваем перерисовку формы для отображения изменений
                this.Invalidate();
            }
        }
    }

    // Абстрактный класс для всех фигур
    public abstract class Shape
    {
        public abstract void Draw(Graphics g);               // Метод для отрисовки фигуры
        public abstract bool HitTest(Point p);               // Метод для проверки попадания точки в фигуру
        public abstract void ToggleSelect(bool ctrlPressed); // Метод для переключения выделения
        public abstract bool IsSelected 
        { 
            get;                                             // Свойство для проверки, выделена ли фигура
        }            
        public abstract void Deselect();                     // Метод для принудительного снятия выделения
    }

    // Класс круга, наследуется от Shape
    public class CCircle : Shape
    {
        private const int Radius = 30;       // Радиус для всех кругов
        private Point center;                // Координаты центра круга
        private bool selected = false;       // Флаг выделен ли данный круг

        // Конструктор круга
        public CCircle(Point center)
        {
            this.center = center;
            this.selected = false; // <-- ТРУ ЕСЛИ ХОЧЕШЬ ВЫДЕЛЯТЬ СРАЗУ СОЗДАННЫЙ КРУГ
        }

        // Метод отрисовки круга
        public override void Draw(Graphics g)
        {
            // Создаем прямоугольник, описывающий круг

            Rectangle rect = new Rectangle(center.X - Radius, center.Y - Radius, Radius * 2, Radius * 2);

            // Рисования контура: красное для выделенных, черное для остальных

            Pen pen = selected ? Pens.Red : Pens.Magenta;

            // Рисуем контур круга (эллипс)
            g.DrawEllipse(pen, rect);

            // Если круг выделен, закрашиваем его полупрозрачным красным цветом для наглядности
            if (selected)
            {
                // Используем цвет с альфа-каналом (50) для полупрозрачности
                g.FillEllipse(new SolidBrush(Color.FromArgb(50, Color.Red)), rect);
            }
        }

        // Реализация метода проверки попадания точки в круг
        public override bool HitTest(Point p)
        {
            // Вычисляем квадраты разностей координат точки клика и центра круга
            int dx = p.X - center.X;
            int dy = p.Y - center.Y;
            // Проверяем по теореме Пифагора: квадрат расстояния от точки до центра должен быть меньше или равен квадрату радиуса
            return dx * dx + dy * dy <= Radius * Radius;
        }

        // Реализация метода переключения выделения
        public override void ToggleSelect(bool ctrlPressed)
        {
            // Если Ctrl НЕ был нажат то круг выделяется
            if (!ctrlPressed)
            {
                selected = true;
            }

            // Если Ctrl БЫЛ нажат, то состояние выделения инвертируется
            else
            {
                selected = !selected;
            }
        }

        // Реализация метода для снятия выделения
        public override void Deselect()
        {
            selected = false; // Просто устанавливаем флаг выделения в false
        }

        public override bool IsSelected => selected;
    }

    // Контейнер для кругов
    public class MyStorage
    {
        // Список для хранения всех фигур на форме
        private List<Shape> shapes = new List<Shape>();

        // Метод для добавления новой фигуры в хранилище
        public void Add(Shape shape) => shapes.Add(shape);

        // Метод для удаления всех выделенных фигур
        public void RemoveSelected()
        {
            shapes = shapes.Where(s => !s.IsSelected).ToList();
        }

        // Метод для снятия выделения со всех фигур в хранилище
        public void ClearSelection()
        {
            // Проходим по всем фигурам и вызываем у каждой метод Deselect()
            foreach (var shape in shapes)
            {
                shape.Deselect(); // Используем новый абстрактный метод Deselect
            }
        }

        // Метод для проверки, находится ли хотя бы одна фигура под указанной точкой
        public bool HitAny(Point p)
        {
            return shapes.Any(s => s.HitTest(p));
        }

        // Метод для отрисовки всех фигур в хранилище
        public void DrawAll(Graphics g)
        {
            // Проходим по всем фигурам и вызываем у каждой метод Draw
            foreach (var shape in shapes)
            {
                shape.Draw(g);
            }
        }

        // Метод для получения доступа ко всем фигурам в хранилище
        public IEnumerable<Shape> All()
        {
            // Возвращаем текущий список фигур
            return shapes;
        }
    }
}
