using System;
using System.Collections; // Для IEnumerable
using System.Collections.Generic;
using System.Drawing; // Понадобится для Graphics при рисовании

namespace OOP4
{
    // Собственный контейнер для хранения фигур
    public class ShapeStorage : IEnumerable<Shape> // Реализуем IEnumerable для foreach
    {
        // Внутреннее хранилище List<Shape> 
        private List<Shape> shapes;

        // Конструктор
        public ShapeStorage()
        {
            shapes = new List<Shape>();
        }

        // Добавление фигуры
        public void Add(Shape shape)
        {
            // Проверка на null остается полезной
            if (shape != null)
            {
                shapes.Add(shape);
            }
        }

        // Удаление фигуры по ссылке
        public bool Remove(Shape shape)
        {
            // Прямой вызов List<T>.Remove
            return shapes.Remove(shape);
        }

        // Удаление фигуры по индексу
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < shapes.Count)
            {
                shapes.RemoveAt(index);
            }
            else
            {
                // Можно выбросить исключение ArgumentOutOfRangeException
                Console.WriteLine($"Error: Index {index} is out of range.");
            }
        }

        // Получение фигуры по индексу
        public Shape Get(int index)
        {
            if (index >= 0 && index < shapes.Count)
            {
                return shapes[index];
            }
            else
            {
                // Можно выбросить исключение или вернуть null
                Console.WriteLine($"Error: Index {index} is out of range.");
                return null;
            }
        }

        // Индексатор для доступа как к массиву: storage[i]
        public Shape this[int index]
        {
            get { return shapes[index]; }
        }


        // Количество фигур в контейнере
        public int Count
        {
            // Прямой вызов List<T>.Count
            get { return shapes.Count; }
        }

        // Очистка контейнера
        public void Clear()
        {
            // Прямой вызов List<T>.Clear
            shapes.Clear();
        }

        // Метод для отрисовки всех фигур в контейнере
        public void DrawAll(Graphics g)
        {
            // Рисуем в порядке добавления. Комментарий про альтернативный порядок сохранен.
            foreach (var shape in shapes)
            {
                shape.Draw(g);
            }

            // Альтернатива: рисовать сначала невыделенные, потом выделенные
            // foreach (var shape in shapes) { if (!shape.IsSelected) shape.Draw(g); }
            // foreach (var shape in shapes) { if (shape.IsSelected) shape.Draw(g); } // Выделенные рисуются поверх
        }

        // Метод для получения фигуры по точке
        // Возвращает последнюю (верхнюю) фигуру, содержащую точку
        public Shape GetShapeAt(Point point)
        {
            // Ищем с конца списка, чтобы найти "верхнюю" фигуру
            for (int i = shapes.Count - 1; i >= 0; i--)
            {
                if (shapes[i].ContainsPoint(point))
                {
                    return shapes[i];
                }
            }
            return null; // Ни одна фигура не содержит точку
        }

        // Реализация IEnumerable<Shape> 
        public IEnumerator<Shape> GetEnumerator()
        {
            // Прямой вызов List<T>.GetEnumerator
            return shapes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
