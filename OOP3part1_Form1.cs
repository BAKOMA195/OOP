using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace OOP3part1
{
    public partial class Form1 : Form
    {
        private List<CCircle> circles = new List<CCircle>(); // Список кругов

        public Form1()
        {
            InitializeComponent();
            this.MouseClick += Form1_MouseClick; // Обрабатываем клики мыши
            this.Paint += Form1_Paint; // Перерисовка формы
            this.KeyDown += Form1_KeyDown; // Обработка клавиш
            this.KeyPreview = true; // Позволяет форме перехватывать клавиши
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public class CCircle
        {
            public int X;
            public int Y;
            public int R = 20;
            public bool IsSelected = false; // Флаг выделения

            public CCircle(int x, int y)
            {
                X = x;
                Y = y;
            }

            // Проверка, находится ли точка внутри круга
            public bool ContainsPoint(int x, int y)
            {
                int dx = x - X;
                int dy = y - Y;
                return dx * dx + dy * dy <= R * R;
            }

            // Отрисовка круга (красный, если выделен)
            public void Draw(Graphics g)
            {
                Brush brush = IsSelected ? Brushes.Red : Brushes.Magenta;
                g.FillEllipse(brush, X - R, Y - R, R * 2, R * 2);
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // ПКМ - создаем новый круг
            {
                circles.Add(new CCircle(e.X, e.Y));
            }
            else if (e.Button == MouseButtons.Left) // ЛКМ - выделение
            {
                bool found = false; // Флаг, был ли клик по кругу
                bool ctrlPressed = ModifierKeys.HasFlag(Keys.Control); // Проверяем, зажат ли Ctrl
                bool ctrlAndLeftMousePressed = ModifierKeys.HasFlag(Keys.Control) && e.Button == MouseButtons.Left;

                foreach (var circle in circles)
                {
                    if (circle.ContainsPoint(e.X, e.Y))
                    {
                        found = true;
                        if (!ctrlAndLeftMousePressed)
                        {
                            circle.IsSelected = !circle.IsSelected; // Переключаем выделение
                        }
                        if (ctrlAndLeftMousePressed)
                        {
                            circle.IsSelected = true; // Обычное выделение
                        }
                        if (!ctrlPressed)
                        {
                            circle.IsSelected = true;
                        }
                    }
                    else if (!ctrlAndLeftMousePressed) // Если Ctrl не зажат - снимаем выделение с остальных
                    {
                        circle.IsSelected = false;
                    }
                }

                // Если кликнули на пустое место без Ctrl, снимаем выделение со всех
                if (!found && !ctrlAndLeftMousePressed)
                {
                    foreach (var circle in circles)
                    {
                        circle.IsSelected = false;
                    }
                }
            }

            this.Invalidate(); // Перерисовываем форму
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var circle in circles)
            {
                circle.Draw(e.Graphics);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) // Удаление выделенных кругов
            {
                circles.RemoveAll(c => c.IsSelected);
                this.Invalidate();
            }
        }
    }
}
