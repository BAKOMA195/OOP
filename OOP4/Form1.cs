using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D; // Для SmoothingMode
using System.Windows.Forms;

namespace OOP4 // Убедись, что пространство имен твое
{
    public partial class Form1 : Form
    {
        private ShapeStorage storage; // Наш контейнер фигур
        private ShapeType currentShapeType; // Тип фигуры для создания
        private List<Shape> selectedShapes; // Список выделенных фигур
        private Color currentColor; // Текущий выбранный цвет

        // Для рисования/перетаскивания мышью
        private bool isDrawing = false;
        private bool isDragging = false;
        private Point startPoint;
        private Point lastMousePosition;

        // НОВОЕ ПОЛЕ для временной фигуры
        private Shape temporaryShape = null;

        // Для контроля границ
        private Rectangle drawingAreaBounds;

        public Form1()
        {
            InitializeComponent();

            storage = new ShapeStorage();
            selectedShapes = new List<Shape>();
            currentShapeType = ShapeType.Rectangle; // По умолчанию
            currentColor = Color.Red; // Начальный цвет

            // Включаем двойную буферизацию для формы для уменьшения мерцания
            this.DoubleBuffered = true;
            // Включаем обработку событий клавиатуры на уровне формы
            this.KeyPreview = true;

            // Инициализируем границы рабочей области
            UpdateDrawingAreaBounds();

            // Подписываемся на события PictureBox
            pictureBoxDrawingArea.Paint += PictureBoxDrawingArea_Paint;
            pictureBoxDrawingArea.MouseDown += PictureBoxDrawingArea_MouseDown;
            pictureBoxDrawingArea.MouseMove += PictureBoxDrawingArea_MouseMove;
            pictureBoxDrawingArea.MouseUp += PictureBoxDrawingArea_MouseUp;

            // Подписываемся на событие изменения размера формы/пикчербокса
            pictureBoxDrawingArea.Resize += (s, e) => UpdateDrawingAreaBounds();

            // Подписываемся на событие нажатия клавиш на форме
            this.KeyDown += Form1_KeyDown;
        }

        // Обновление границ рабочей области
        private void UpdateDrawingAreaBounds()
        {
            // Используем ClientRectangle PictureBox'а как границы
            drawingAreaBounds = pictureBoxDrawingArea.ClientRectangle;
            // Уменьшим на 1 пиксель, чтобы граница не перекрывалась
            if (drawingAreaBounds.Width > 0) drawingAreaBounds.Width -= 1;
            if (drawingAreaBounds.Height > 0) drawingAreaBounds.Height -= 1;
            pictureBoxDrawingArea.Invalidate(); // Перерисовать при изменении размера
        }

        // Обработчики событий PictureBox

        private void PictureBoxDrawingArea_Paint(object sender, PaintEventArgs e)
        {
            // Включаем сглаживание
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Рисуем все фигуры из хранилища
            storage.DrawAll(e.Graphics);

            // Рисуем временную фигуру, если мы в процессе рисования
            if (isDrawing && temporaryShape != null)
            {
                temporaryShape.Draw(e.Graphics);
                // Важно: не вызываем DrawSelection для временной фигуры
            }
        }

        private void PictureBoxDrawingArea_MouseDown(object sender, MouseEventArgs e)
        {
            // Проверяем, был ли клик левой кнопкой
            if (e.Button != MouseButtons.Left) return;

            startPoint = e.Location;
            lastMousePosition = e.Location;
            isDrawing = false;
            isDragging = false;
            temporaryShape = null; // Сбрасываем временную фигуру при каждом клике

            Shape clickedShape = storage.GetShapeAt(e.Location);
            bool controlPressed = ModifierKeys.HasFlag(Keys.Control);

            if (clickedShape != null) // Кликнули по какой-то фигуре
            {
                if (controlPressed) // Обработка Ctrl + Клик
                {
                    clickedShape.IsSelected = !clickedShape.IsSelected;
                }
                else // Обработка Простого Клика (без Ctrl)
                {
                    if (!clickedShape.IsSelected)
                    {
                        DeselectAllShapes();
                        clickedShape.IsSelected = true;
                    }
                    // Если кликнули по уже выделенной без Ctrl - ничего не меняем
                }
                UpdateSelectedShapesList();
                if (clickedShape.IsSelected) // Начинаем тащить, только если фигура выделена
                {
                    isDragging = true;
                }
            }
            else // Кликнули на пустом месте
            {
                if (!controlPressed)
                {
                    DeselectAllShapes();
                    UpdateSelectedShapesList();
                }
                isDrawing = true; // Начинаем рисовать новую фигуру
                // Временную фигуру создадим в первом MouseMove
            }

            pictureBoxDrawingArea.Invalidate(); // Запрос на перерисовку
        }

        private void PictureBoxDrawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            // Двигаем выделенные фигуры
            if (isDragging && selectedShapes.Count > 0 && e.Button == MouseButtons.Left)
            {
                int dx = e.X - lastMousePosition.X;
                int dy = e.Y - lastMousePosition.Y;

                bool canMove = true;
                foreach (var shape in selectedShapes)
                {
                    Rectangle potentialBounds = shape.GetBounds();
                    potentialBounds.Offset(dx, dy);
                    if (!drawingAreaBounds.Contains(potentialBounds))
                    {
                        canMove = false;
                        break;
                    }
                }

                if (canMove)
                {
                    foreach (var shape in selectedShapes)
                    {
                        shape.Move(dx, dy);
                    }
                    lastMousePosition = e.Location;
                    pictureBoxDrawingArea.Invalidate();
                }
            }
            // Рисуем новую фигуру
            else if (isDrawing && e.Button == MouseButtons.Left)
            {
                lastMousePosition = e.Location;
                // Создаем или обновляем временную фигуру
                // Используем новый вспомогательный метод
                temporaryShape = CreateShapeObject(startPoint, lastMousePosition, false); // false = не добавлять в хранилище

                pictureBoxDrawingArea.Invalidate(); // Запрос на перерисовку для отображения temporaryShape
            }
        }

        private void PictureBoxDrawingArea_MouseUp(object sender, MouseEventArgs e)
        {
            // Завершаем рисование новой фигуры
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                isDrawing = false;
                Point endPoint = e.Location;

                // Создаем ПОСТОЯННУЮ фигуру, если мышь сдвинулась достаточно
                if (Math.Abs(endPoint.X - startPoint.X) > 3 || Math.Abs(endPoint.Y - startPoint.Y) > 3)
                {
                    // Используем основной метод CreateShape, который добавит фигуру в хранилище
                    CreateAndStoreShape(startPoint, endPoint);
                }

                temporaryShape = null; // Убираем временную фигуру
                pictureBoxDrawingArea.Invalidate(); // Перерисовываем, чтобы убрать временную и показать постоянную
            }

            // Завершаем перетаскивание
            if (isDragging && e.Button == MouseButtons.Left) // Проверяем кнопку, чтобы правый клик не сбросил
            {
                isDragging = false;
                // Перерисовка не обязательна, т.к. фигуры уже на месте, но для надежности можно оставить
                pictureBoxDrawingArea.Invalidate();
            }
        }
        private Shape CreateShapeObject(Point p1, Point p2, bool useFixedStartPoint = true)
        {
            // Определяем topLeft и size по двум точкам
            Point topLeft = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            Size size = new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));

            // Минимальный размер для временной фигуры не так важен, но оставим
            if (size.Width < 1) size.Width = 1;
            if (size.Height < 1) size.Height = 1;

            Shape shape = null;

            switch (currentShapeType)
            {
                case ShapeType.Circle:
                    int radius = Math.Min(size.Width, size.Height) / 2;
                    if (radius < 1) radius = 1;
                    Point center = new Point(topLeft.X + size.Width / 2, topLeft.Y + size.Height / 2);
                    shape = new Circle(center, radius, currentColor);
                    break;
                case ShapeType.Square:
                    int side = Math.Min(size.Width, size.Height);
                    if (side < 1) side = 1;
                    shape = new Square(topLeft, side, currentColor);
                    break;
                case ShapeType.Rectangle:
                    shape = new RectangleShape(topLeft, size, currentColor);
                    break;
                case ShapeType.Ellipse:
                    shape = new Ellipse(topLeft, size, currentColor);
                    break;
                case ShapeType.Triangle:
                    int triSize = Math.Min(size.Width, size.Height);
                    if (triSize < 5) triSize = 5; // Треугольнику нужен размер побольше
                    Point triCenter = new Point(topLeft.X + size.Width / 2, topLeft.Y + size.Height / 2);
                    shape = new Triangle(triCenter, triSize, currentColor);
                    break;
                case ShapeType.Line:
                    // Для линии используем точки напрямую
                    Point lineStart = useFixedStartPoint ? p1 : startPoint; // Используем startPoint формы при рисовании
                    Point lineEnd = p2;
                    shape = new LineSegment(lineStart, lineEnd, currentColor, 2);
                    break;
            }
            return shape;
        }
        private void CreateAndStoreShape(Point p1, Point p2)
        {
            // Создаем объект фигуры с помощью нового метода
            Shape newShape = CreateShapeObject(p1, p2);

            if (newShape != null)
            {
                // Проверяем, не выходит ли новая фигура за границы СРАЗУ
                if (newShape.IsWithinBounds(drawingAreaBounds))
                {
                    storage.Add(newShape);
                    // Сразу выделяем созданную фигуру
                    DeselectAllShapes();
                    newShape.IsSelected = true;
                    UpdateSelectedShapesList();
                    // Перерисовка будет вызвана из MouseUp
                }
                else
                {
                    Console.WriteLine("New shape could not be created: Out of bounds.");
                    MessageBox.Show("Фигура выходит за границы рабочей области!", "Ошибка создания", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // Перерисовка все равно нужна, чтобы убрать временную фигуру (вызовется из MouseUp)
                }
            }
        }


        // Обработчик событий клавиатуры (без изменений)
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (selectedShapes.Count == 0 && e.KeyCode != Keys.Delete) return; // Не обрабатываем +/- если нет выделения

            int dx = 0;
            int dy = 0;
            int dw = 0; // Изменение ширины
            int dh = 0; // Изменение высоты
            bool needsProcessing = false; // Флаг, что клавиша должна быть обработана

            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (selectedShapes.Count > 0) { dx = -5; needsProcessing = true; }
                    break;
                case Keys.Right:
                    if (selectedShapes.Count > 0) { dx = 5; needsProcessing = true; }
                    break;
                case Keys.Up:
                    if (selectedShapes.Count > 0) { dy = -5; needsProcessing = true; }
                    break;
                case Keys.Down:
                    if (selectedShapes.Count > 0) { dy = 5; needsProcessing = true; }
                    break;
                case Keys.Delete:
                    if (selectedShapes.Count > 0) { needsProcessing = true; } // Удаляем, если есть что
                    break;
                case Keys.Add:
                case Keys.Oemplus:
                    if (selectedShapes.Count > 0) { dw = 5; dh = 5; needsProcessing = true; }
                    break;
                case Keys.Subtract:
                case Keys.OemMinus:
                    if (selectedShapes.Count > 0) { dw = -5; dh = -5; needsProcessing = true; }
                    break;
            }

            if (needsProcessing)
            {
                // Применяем изменения (перемещение, ресайз или удаление)
                if (e.KeyCode == Keys.Delete)
                {
                    ApplyDelete();
                }
                else if (dx != 0 || dy != 0)
                {
                    ApplyMovement(dx, dy);
                }
                else if (dw != 0 || dh != 0)
                {
                    ApplyResize(dw, dh);
                }

                // Подавляем стандартную обработку клавиши, если мы ее использовали
                e.Handled = true;
                // SuppressKeyPress нужен, чтобы предотвратить системный звук "бип" для некоторых клавиш
                e.SuppressKeyPress = (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus ||
                                      e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus ||
                                      e.KeyCode == Keys.Delete || e.KeyCode == Keys.Up ||
                                      e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right);
            }
        }

        // Вспомогательные методы для обработки клавиатуры

        private void ApplyDelete()
        {
            List<Shape> shapesToRemove = new List<Shape>(selectedShapes); // Копия
            foreach (var shape in shapesToRemove)
            {
                storage.Remove(shape);
            }
            selectedShapes.Clear(); // Очищаем список выделенных
            pictureBoxDrawingArea.Invalidate(); // Перерисовать после удаления
        }

        private void ApplyMovement(int dx, int dy)
        {
            bool canMove = true;
            foreach (var shape in selectedShapes)
            {
                Rectangle potentialBounds = shape.GetBounds();
                potentialBounds.Offset(dx, dy);
                if (!drawingAreaBounds.Contains(potentialBounds))
                {
                    canMove = false;
                    break;
                }
            }

            if (canMove)
            {
                foreach (var shape in selectedShapes)
                {
                    shape.Move(dx, dy);
                }
                pictureBoxDrawingArea.Invalidate();
            }
            else
            {
                Console.WriteLine("Movement cancelled: Shape would go out of bounds.");
            }
        }

        private void ApplyResize(int dw, int dh)
        {
            bool canResizeAll = true;
            foreach (var shape in selectedShapes)
            {
                Rectangle currentBounds = shape.GetBounds();
                int newWidth = currentBounds.Width + dw;
                int newHeight = currentBounds.Height + dh;

                // Проверка на минимальный размер
                if (newWidth < 5 || newHeight < 5)
                {
                    Console.WriteLine($"Resize cancelled: Shape {shape.GetType().Name} would become too small.");
                    canResizeAll = false;
                    break;
                }

                // Упрощенная проверка на выход за границы при увеличении
                if (dw > 0 || dh > 0)
                {
                    Rectangle potentialBounds = currentBounds;
                    // Примерная оценка новых границ (может быть неточной для фигур с центром)
                    potentialBounds.Width = newWidth;
                    potentialBounds.Height = newHeight;
                    if (shape is Circle || shape is Triangle) // Для фигур с центром, он может сместиться при ресайзе от края
                    {
                        // Более сложная проверка нужна, пока упростим - проверим только правый/нижний край
                    }

                    if (!drawingAreaBounds.Contains(potentialBounds) &&
                       (potentialBounds.Right > drawingAreaBounds.Right || potentialBounds.Bottom > drawingAreaBounds.Bottom))
                    {
                        Console.WriteLine($"Resize cancelled: Shape {shape.GetType().Name} might go out of bounds.");
                        canResizeAll = false;
                        break;
                    }
                }
            }

            if (canResizeAll)
            {
                foreach (var shape in selectedShapes)
                {
                    shape.Resize(dw, dh);
                    // Пост-проверка (если Resize некорректно обработал границы)
                    if (!shape.IsWithinBounds(drawingAreaBounds))
                    {
                        Console.WriteLine($"Warning: Shape {shape.GetType().Name} went out of bounds AFTER resize. Reverting is complex.");
                    }
                }
                pictureBoxDrawingArea.Invalidate();
            }
            else
            {
                Console.WriteLine("Resize operation cancelled for the group.");
            }
        }


        // Прочие Вспомогательные методы

        private void DeselectAllShapes()
        {
            foreach (var shape in storage)
            {
                shape.IsSelected = false;
            }
            selectedShapes.Clear();
        }

        private void UpdateSelectedShapesList()
        {
            selectedShapes.Clear();
            foreach (var shape in storage)
            {
                if (shape.IsSelected)
                {
                    selectedShapes.Add(shape);
                }
            }
        }
        // Обработчики событий Меню
        private void SetCurrentShapeType(ShapeType type)
        {
            currentShapeType = type;
            this.Text = $"Визуальный редактор - Инструмент: {type}";
        }

        private void квадратToolStripMenuItem_Click(object sender, EventArgs e)
        { SetCurrentShapeType(ShapeType.Square); }

        private void круToolStripMenuItem_Click(object sender, EventArgs e)
        { SetCurrentShapeType(ShapeType.Circle); }

        private void прямоугольникToolStripMenuItem_Click(object sender, EventArgs e)
        { SetCurrentShapeType(ShapeType.Rectangle); }

        private void эллипсToolStripMenuItem_Click(object sender, EventArgs e)
        { SetCurrentShapeType(ShapeType.Ellipse); }

        private void отрезокToolStripMenuItem_Click(object sender, EventArgs e)
        { SetCurrentShapeType(ShapeType.Line); }

        private void треугольникToolStripMenuItem_Click(object sender, EventArgs e)
        { SetCurrentShapeType(ShapeType.Triangle); }

        private void заливкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = currentColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    currentColor = colorDialog.Color;
                    if (selectedShapes.Count > 0)
                    {
                        foreach (var shape in selectedShapes)
                        {
                            shape.FillColor = currentColor;
                        }
                        pictureBoxDrawingArea.Invalidate();
                    }
                }
            }
        }
    }
}
