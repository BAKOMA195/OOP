using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OOP3part1.Form1;

namespace OOP3part1
{

    public partial class Form1 : Form
    {

        private List<CCircle> circles = new List<CCircle>(); // Список кругов

        public Form1()
        {
            InitializeComponent();
            this.MouseClick += Form1_MouseClick; // Подписываемся на клик мыши
            this.Paint += Form1_Paint; // Подписываемся на перерисовку
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public class CCircle
        {
            public int X;
            public int Y;
            public int R = 20;
            public CCircle(int x, int y)
            {
                X = x; Y = y;
            }

            public void Draw(Graphics g)
            {
                g.FillEllipse(Brushes.Magenta, X - R, Y - R, R * 2, R * 2);
            }

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            circles.Add(new CCircle(e.X, e.Y)); // Добавляем круг по координатам клика
            this.Invalidate(); // Перерисовываем форму
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var circle in circles)
            {
                circle.Draw(e.Graphics); // Рисуем все круги из списка
            }
        }
    }
}
