using System;
using System.Drawing;
using System.Drawing.Drawing2D; // Для AntiAlias

namespace OOP4
{
    // Перечисление для типов фигур
    public enum ShapeType
    {
        Circle,
        Square,
        Rectangle,
        Ellipse,
        Triangle,
        Line
    }

    // Базовый абстрактный класс для всех фигур
    public abstract class Shape
    {
        // Общие свойства для всех фигур
        public Point Position { get; protected set; } // Положение
        public Size ShapeSize { get; protected set; } // Размер
        public Color FillColor { get; set; } // Цвет заливки
        public bool IsSelected { get; set; } // Флаг выделения

        // Конструктор базового класса
        protected Shape(Point position, Size size, Color color)
        {
            Position = position;
            ShapeSize = size;
            FillColor = color;
            IsSelected = false;
        }

        // Абстрактный метод рисования
        public abstract void Draw(Graphics g);

        // Виртуальный метод для проверки попадания точки внутрь фигуры
        public virtual bool ContainsPoint(Point p)
        {
            // Проверка попадания в ограничивающий прямоугольник
            return GetBounds().Contains(p);
        }

        // Виртуальный метод для получения ограничивающего прямоугольника
        public virtual Rectangle GetBounds()
        {
            return new Rectangle(Position, ShapeSize);
        }

        // Метод для перемещения фигуры
        public virtual void Move(int dx, int dy)
        {
            Position = new Point(Position.X + dx, Position.Y + dy);
        }

        // Метод для изменения размера фигуры
        public virtual void Resize(int dw, int dh)
        {
            // Убедимся, что размер не становится отрицательным или нулевым
            int newWidth = Math.Max(1, ShapeSize.Width + dw);
            int newHeight = Math.Max(1, ShapeSize.Height + dh);
            ShapeSize = new Size(newWidth, newHeight);
        }

        // Метод для проверки находится ли фигура полностью внутри заданной области
        public bool IsWithinBounds(Rectangle containerBounds)
        {
            return containerBounds.Contains(GetBounds());
        }

        // Вспомогательный метод для рисования рамки выделения
        protected void DrawSelection(Graphics g)
        {
            if (IsSelected)
            {
                using (Pen selectionPen = new Pen(Color.Blue, 2))
                {
                    selectionPen.DashStyle = DashStyle.Dash;
                    // Рисуем рамку чуть больше самой фигуры для наглядности
                    Rectangle bounds = GetBounds();
                    bounds.Inflate(3, 3); // Увеличиваем рамку
                    g.DrawRectangle(selectionPen, bounds);
                }
            }
        }
    }

    // Классы-наследники фигур

    public class Circle : Shape
    {
        public Circle(Point center, int radius, Color color): base(new Point(center.X - radius, center.Y - radius), new Size(radius * 2, radius * 2), color)
        { }

        public override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias; // Красивее :)
            using (Brush brush = new SolidBrush(FillColor))
            {
                g.FillEllipse(brush, GetBounds());
            }
            DrawSelection(g); // Рисуем рамку, если выделен
        }

        public override bool ContainsPoint(Point p)
        {
            // Проверка попадания в круг
            Point center = new Point(Position.X + ShapeSize.Width / 2, Position.Y + ShapeSize.Height / 2);
            int radius = ShapeSize.Width / 2;
            // Используем квадрат расстояния для избежания извлечения корня
            return Math.Pow(p.X - center.X, 2) + Math.Pow(p.Y - center.Y, 2) <= Math.Pow(radius, 2);
        }

        public override void Resize(int dw, int dh)
        {
            // Изменяем радиус на основе большего изменения
            int delta = Math.Max(dw, dh);
            int newDiameter = Math.Max(2, ShapeSize.Width + delta); // Диаметр минимум 2
            // Корректируем Position, чтобы центр остался на месте
            int radiusChange = (newDiameter - ShapeSize.Width) / 2;
            Position = new Point(Position.X - radiusChange, Position.Y - radiusChange);
            ShapeSize = new Size(newDiameter, newDiameter);
        }
    }

    public class Square : Shape
    {
        public Square(Point topLeft, int side, Color color) : base(topLeft, new Size(side, side), color)
        { }

        public override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush brush = new SolidBrush(FillColor))
            {
                g.FillRectangle(brush, GetBounds());
            }
            DrawSelection(g);
        }

        public override void Resize(int dw, int dh)
        {
            // Используем большее изменение, чтобы сохранить пропорции
            int delta = Math.Max(dw, dh);
            int newSide = Math.Max(1, ShapeSize.Width + delta);
            ShapeSize = new Size(newSide, newSide);
            // Position не меняем растет от верхнего левого угла
        }
    }

    public class RectangleShape : Shape
    {
        public RectangleShape(Point topLeft, Size size, Color color) : base(topLeft, size, color)
        { }

        public override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush brush = new SolidBrush(FillColor))
            {
                g.FillRectangle(brush, GetBounds());
            }
            DrawSelection(g);
        }
        // Resize и ContainsPoint наследуются от Shape и подходят
    }

    public class Ellipse : Shape
    {
        public Ellipse(Point topLeft, Size size, Color color) : base(topLeft, size, color)
        { }

        public override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush brush = new SolidBrush(FillColor))
            {
                g.FillEllipse(brush, GetBounds());
            }
            DrawSelection(g);
        }

    }

    public class Triangle : Shape
    {
        // Задаем треугольник тремя точками
        public Point P1 { get; private set; }
        public Point P2 { get; private set; }
        public Point P3 { get; private set; }

        public Triangle(Point center, int size, Color color)
            : base(new Point(center.X - size / 2, center.Y - (int)(size * Math.Sqrt(3) / 4)), // Верхний левый угол
                   new Size(size, (int)(size * Math.Sqrt(3) / 2)), // Размер
                   color)
        {
            // Рассчитаем вершины равностороннего треугольника вокруг центра
            double height = size * Math.Sqrt(3) / 2;
            P1 = new Point(center.X, center.Y - (int)(height * 2 / 3)); // Верхняя вершина
            P2 = new Point(center.X - size / 2, center.Y + (int)(height / 3)); // Левая нижняя
            P3 = new Point(center.X + size / 2, center.Y + (int)(height / 3)); // Правая нижняя

            // Обновим Position и ShapeSize
            UpdateBoundsFromPoints();
        }

        // Обновляет Position и ShapeSize на основе текущих P1, P2, P3
        private void UpdateBoundsFromPoints()
        {
            int minX = Math.Min(P1.X, Math.Min(P2.X, P3.X));
            int minY = Math.Min(P1.Y, Math.Min(P2.Y, P3.Y));
            int maxX = Math.Max(P1.X, Math.Max(P2.X, P3.X));
            int maxY = Math.Max(P1.Y, Math.Max(P2.Y, P3.Y));
            Position = new Point(minX, minY);
            // Убедимся, что размер не нулевой, чтобы избежать проблем с делением
            ShapeSize = new Size(Math.Max(1, maxX - minX), Math.Max(1, maxY - minY));
        }

        // Метод рисования треугольника
        public override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush brush = new SolidBrush(FillColor))
            {
                g.FillPolygon(brush, new Point[] { P1, P2, P3 });
            }
            DrawSelection(g); // Рисуем рамку, если выделен
        }

        // Перемещение треугольника
        public override void Move(int dx, int dy)
        {
            // Двигаем все три точки
            P1 = new Point(P1.X + dx, P1.Y + dy);
            P2 = new Point(P2.X + dx, P2.Y + dy);
            P3 = new Point(P3.X + dx, P3.Y + dy);
            UpdateBoundsFromPoints(); // Обновляем Position и Size
        }

        // Изменение размера треугольника
        public override void Resize(int dw, int dh)
        {
            // Вычисляем текущий центр тяжести треугольника
            PointF center = new PointF((P1.X + P2.X + P3.X) / 3.0f,
                                       (P1.Y + P2.Y + P3.Y) / 3.0f);

            // Вычисляем коэффициент масштабирования
            // Сохраняем пропорции, используя большее из изменений (dw, dh)
            int currentSizeApprox = Math.Max(1, ShapeSize.Width);
            int delta = Math.Max(dw, dh); // Насколько хотим изменить размер
            int newSizeApprox = currentSizeApprox + delta; // Новый предполагаемый размер

            // Не позволяем размеру стать слишком маленьким
            const int minTriangleSize = 10;
            if (newSizeApprox < minTriangleSize)
            {
                // Если пытаемся уменьшить до размера меньше минимального, устанавливаем минимальный
                if (delta < 0)
                {
                    newSizeApprox = minTriangleSize;
                }
                else // Если пытаемся увеличить, но размер уже меньше минимального ничего не делаем
                {
                    return;
                }
            }

            // Предотвращаем деление на ноль
            if (currentSizeApprox <= 0) return;

            float scale = (float)newSizeApprox / currentSizeApprox; // Коэффициент масштабирования
            if (scale <= 0) return; // Масштаб должен быть положительным

            // Масштабируем каждую вершину относительно вычисленного центра
            P1 = ScalePoint(P1, center, scale);
            P2 = ScalePoint(P2, center, scale);
            P3 = ScalePoint(P3, center, scale);

            // Обновляем Position и ShapeSize на основе новых координат вершин
            UpdateBoundsFromPoints();
        }

        // Вспомогательный метод для масштабирования точки относительно центра
        private Point ScalePoint(Point p, PointF center, float scale)
        {
            // Находим вектор от центра к точке
            float vecX = p.X - center.X;
            float vecY = p.Y - center.Y;

            // Умножаем вектор на коэффициент масштабирования
            vecX *= scale;
            vecY *= scale;

            // Находим новые координаты: центр + масштабированный вектор
            float newX = center.X + vecX;
            float newY = center.Y + vecY;

            // Округляем до целых чисел для Point
            return new Point((int)Math.Round(newX), (int)Math.Round(newY));
        }
    }

    public class LineSegment : Shape // Отрезок
    {
        public Point StartPoint { get; private set; }
        public Point EndPoint { get; private set; }
        public int LineWidth { get; set; } // Толщина линии

        public LineSegment(Point start, Point end, Color color, int lineWidth = 2)
            : base(new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y)), // Position верхний левый угол bounding box
                   new Size(Math.Abs(start.X - end.X), Math.Abs(start.Y - end.Y)), // Size размеры выделения
                   color)
        {
            StartPoint = start;
            EndPoint = end;
            LineWidth = lineWidth;
            // Обновляем Position и Size с учетом толщины линии
            UpdateBoundsFromPoints();
        }

        // Обновляет Position и Size на основе Start/End Point и LineWidth
        private void UpdateBoundsFromPoints()
        {
            int minX = Math.Min(StartPoint.X, EndPoint.X);
            int minY = Math.Min(StartPoint.Y, EndPoint.Y);
            int maxX = Math.Max(StartPoint.X, EndPoint.X);
            int maxY = Math.Max(StartPoint.Y, EndPoint.Y);
            // Добавим половину толщины линии к границам для корректности GetBounds и ContainsPoint
            int halfWidth = (LineWidth + 1) / 2;
            Position = new Point(minX - halfWidth, minY - halfWidth);
            // Размер должен учитывать толщину линии
            ShapeSize = new Size(Math.Max(1, maxX - minX + LineWidth), Math.Max(1, maxY - minY + LineWidth));
        }

        // Метод рисования линии
        public override void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Используем using для автоматического Dispose пера
            using (Pen pen = new Pen(FillColor, LineWidth))
            {
                g.DrawLine(pen, StartPoint, EndPoint);
            }
            DrawSelection(g); // Рамку рисуем вокруг выделения
        }

        // GetBounds нужно переопределять, чтобы вызвать UpdateBoundsFromPoints перед возвратом базового значения
        public override Rectangle GetBounds()
        {
            // Убедимся, что Position/Size актуальны перед возвратом Bounds
            UpdateBoundsFromPoints();
            return base.GetBounds();
        }

        // Перемещение отрезка
        public override void Move(int dx, int dy)
        {
            // Двигаем обе точки
            StartPoint = new Point(StartPoint.X + dx, StartPoint.Y + dy);
            EndPoint = new Point(EndPoint.X + dx, EndPoint.Y + dy);
            UpdateBoundsFromPoints(); // Обновляем Position и Size
        }

        // Изменение размера отрезка
        public override void Resize(int dw, int dh)
        {
            // Используем dw как изменение длины (т.к. +/- дают одинаковые dw, dh)
            int deltaLength = dw;

            // Вычисляем вектор от StartPoint к EndPoint
            float vecX = EndPoint.X - StartPoint.X;
            float vecY = EndPoint.Y - StartPoint.Y;

            // Вычисляем текущую длину линии
            float currentLength = (float)Math.Sqrt(vecX * vecX + vecY * vecY);

            // Определим минимальную длину линии
            const float minLength = 5.0f;

            // Рассчитываем новую длину
            float newLength = currentLength + deltaLength;

            // Не даем длине стать меньше минимальной
            if (newLength < minLength)
            {
                newLength = minLength;
            }

            // Предотвращаем деление на ноль, если текущая длина очень мала
            if (currentLength < 0.01f)
            {
                // Если линия была точкой, просто чуть сдвинем EndPoint при увеличении
                if (deltaLength > 0)
                {
                    EndPoint = new Point(StartPoint.X + (int)Math.Round(minLength), StartPoint.Y); // Например, вправо
                }
                // Иначе ничего не делаем
                UpdateBoundsFromPoints();
                return; // Выходим
            }

            // Рассчитываем коэффициент масштабирования вектора
            float scale = newLength / currentLength;

            // Масштабируем исходный вектор
            float newVecX = vecX * scale;
            float newVecY = vecY * scale;

            // Находим новую конечную точку StartPoint + newVector
            int newEndX = StartPoint.X + (int)Math.Round(newVecX);
            int newEndY = StartPoint.Y + (int)Math.Round(newVecY);

            // Обновляем конечную точку
            EndPoint = new Point(newEndX, newEndY);

            // Обновляем Position и ShapeSize
            UpdateBoundsFromPoints();
        }
    }
}
