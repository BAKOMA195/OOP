#include <iostream>

using namespace std;

// Базовый класс Point
class Point {

protected:

    int x, y; // Защищенные поля для доступа из классов-наследников

public:

    // Конструктор по умолчанию
    Point() {

        this->x = 0;
        this->y = 0;

        cout << "Конструктор Point()" << endl; // Вывод сообщения о создании объекта

    }

    // Конструктор с параметрами
    Point(int x, int y) {

        this->x = x;
        this->y = y;

        cout << "Конструктор Point(" << x << ", " << y << ")" << endl; // Вывод сообщения о создании объекта с параметрами

    }

    // Виртуальный деструктор для правильного удаления наследников
    virtual ~Point() {

        cout << "Деструктор ~Point() для точки (" << x << ", " << y << ")" << endl; // Вывод сообщения о разрушении объекта

    }

    // Виртуальный метод для демонстрации полиморфизма
    virtual void print() const {

        cout << "Точка: (" << x << ", " << y << ")" << endl; // Вывод координат точки

    }

    // Геттеры для доступа к защищенным полям

    int getX() const { 

        return x; // Возвращает значение x

    } 

    int getY() const { 

        return y; // Возвращает значение y

    } 

    // Конструктор копирования
    Point(const Point& other) {

        x = other.x;
        y = other.y;

        cout << "Конструктор копирования Point" << endl; // Вывод сообщения о копировании

    }

    // Оператор присваивания
    Point& operator = (const Point& other) {

        if (this == &other) {
        
            return *this;

        }

        // Проверка на самоприсваивание
        x = other.x;
        y = other.y;

        cout << "Оператор присваивания Point" << endl; // Вывод сообщения о присваивании

        return *this; // Возврат ссылки на текущий объект

    }
};

// Наследующий класс Circle
class Circle : public Point {

private:

    double radius; // Радиус круга

public:

    // Конструктор по умолчанию
    Circle() : Point() { 

        this->radius = 1.0;

        cout << "Конструктор Circle(), радиус = " << radius << endl; // Вывод сообщения о создании объекта

    }

    // Конструктор с параметрами
    Circle(int x, int y, double r) : Point(x, y) {

        radius = r;

        cout << "Конструктор Circle(" << x << ", " << y << ", " << r << ")" << endl; // Вывод сообщения о создании объекта с параметрами

    }

    // Деструктор
    ~Circle() override {

        cout << "Деструктор ~Circle(), радиус = " << radius << endl; // Вывод сообщения о разрушении объекта

    }

    // Переопределение метода print для демонстрации полиморфизма
    void print() const override {

        cout << "Круг с центром (" << x << ", " << y << ")" << " и радиусом " << radius << endl; // Вывод информации о круге

    }

    // Пример protected-метода в классе-наследнике
protected:

    void setRadius(double r) { 

        radius = r; // Метод для изменения радиуса

    } 

public:

    // Конструктор копирования
    Circle(const Circle& other) : Point(other) {

        radius = other.radius;

        cout << "Конструктор копирования Circle" << endl; // Вывод сообщения о копировании

    }

    // Оператор присваивания
    Circle& operator=(const Circle& other) {

        if (this == &other) {

            return *this; // Проверка на самоприсваивание

        }
        
        Point::operator=(other); // Вызов оператора присваивания базового класса
        radius = other.radius;

        cout << "Оператор присваивания Circle" << endl; // Вывод сообщения о присваивании

        return *this; // Возврат ссылки на текущий объект

    }
};

// Класс Rectangle
class Rectangle {

private:

    Point topLeft; // Левая верхняя точка
    Point bottomRight; // Правая нижняя точка

public:
    // Конструктор с параметрами
    Rectangle(int x1, int y1, int x2, int y2) {

        topLeft = Point(x1, y1);
        bottomRight = Point(x2, y2);

        cout << "Конструктор Rectangle(" << x1 << ", " << y1 << ", " << x2 << ", " << y2 << ")" << endl; // Вывод сообщения о создании объекта

    }

    // Деструктор
    ~Rectangle() {

        cout << "Деструктор ~Rectangle()" << endl; // Вывод сообщения о разрушении объекта

    }

    // Метод для вывода информации о прямоугольнике
    void print() const {

        cout << "Прямоугольник:" << endl;
        cout << " Левый верхний ";

        topLeft.print(); // Вывод левой верхней точки

        cout << " Правый нижний ";

        bottomRight.print(); // Вывод правой нижней точки

    }

    // Конструктор копирования
    Rectangle(const Rectangle& other) {

        topLeft = other.topLeft;
        bottomRight = other.bottomRight;

        cout << "Конструктор копирования Rectangle" << endl; // Вывод сообщения о копировании

    }

    // Оператор присваивания
    Rectangle& operator=(const Rectangle& other) {

        if (this == &other) {

            return *this; // Проверка на самоприсваивание

        }
        
        topLeft = other.topLeft;
        bottomRight = other.bottomRight;

        cout << "Оператор присваивания Rectangle" << endl; // Вывод сообщения о присваивании

        return *this; // Возврат ссылки на текущий объект

    }
};

// Класс RectanglePtr с использованием указателей
class RectanglePtr {

private:

    Point* topLeft; // Указатель на левую верхнюю точку
    Point* bottomRight; // Указатель на правую нижнюю точку

public:

    // Конструктор с параметрами
    RectanglePtr(int x1, int y1, int x2, int y2) {

        topLeft = new Point(x1, y1); // Выделение памяти для левой верхней точки
        bottomRight = new Point(x2, y2); // Выделение памяти для правой нижней точки

        cout << "Конструктор RectanglePtr(" << x1 << ", " << y1 << ", " << x2 << ", " << y2 << ")" << endl; // Вывод сообщения о создании объекта

    }

    // Деструктор
    ~RectanglePtr() {

        cout << "Деструктор ~RectanglePtr()" << endl; // Вывод сообщения о разрушении объекта

        delete topLeft; // Освобождение памяти
        delete bottomRight; // Освобождение памяти

    }

    // Метод для вывода информации о прямоугольнике
    void print() const {

        cout << "Прямоугольник (указатели):" << endl;
        cout << " Левый верхний ";

        topLeft->print(); // Вывод левой верхней точки

        cout << " Правый нижний ";

        bottomRight->print(); // Вывод правой нижней точки

    }

    // Конструктор копирования с глубоким копированием
    RectanglePtr(const RectanglePtr& other) {

        topLeft = new Point(*other.topLeft); // Создание новой точки для левой верхней
        bottomRight = new Point(*other.bottomRight); // Создание новой точки для правой нижней

        cout << "Конструктор копирования RectanglePtr" << endl; // Вывод сообщения о копировании

    }

    // Оператор присваивания с глубоким копированием
    RectanglePtr& operator=(const RectanglePtr& other) {

        if (this == &other) { 

            return *this; // Проверка на самоприсваивание

        }

        delete topLeft; // Освобождение старой памяти
        delete bottomRight; // Освобождение старой памяти

        topLeft = new Point(*other.topLeft); // Создание новой точки для левой верхней
        bottomRight = new Point(*other.bottomRight); // Создание новой точки для правой нижней

        cout << "Оператор присваивания RectanglePtr" << endl; // Вывод сообщения о присваивании

        return *this; // Возврат ссылки на текущий объект

    }
};

int main() {

    setlocale(LC_ALL, "RU"); // Установка локали для корректного вывода русских символов

    {

        cout << "--- Статическое создание объектов ---" << endl;

        Point p1; // Создание объекта Point
        Point p2(2, 3); // Создание объекта Point с параметрами

        Circle c1; // Создание объекта Circle
        Circle c2(5, 5, 10.0); // Создание объекта Circle с параметрами

        Rectangle r(0, 0, 4, 4); // Создание объекта Rectangle

        RectanglePtr rp(1, 1, 3, 3); // Создание объекта RectanglePtr

    }

    {

        cout << "--- Динамическое создание объектов ---" << endl;

        Point* p = new Point(10, 20); // Динамическое создание объекта Point

        Circle* c = new Circle(1, 2, 3.5); // Динамическое создание объекта Circle

        delete p; // Освобождение памяти
        delete c; // Освобождение памяти

    }

    {

        cout << "--- Наследование и полиморфизм ---" << endl;

        Point* ptr = new Circle(7, 8, 9.9); // Указатель на базовый класс указывает на объект производного класса

        ptr->print(); // Вызов метода print() производного класса благодаря полиморфизму

        delete ptr;   // Вызов деструктора производного класса благодаря виртуальному деструктору

    }

    {

        cout << "--- Конструкторы копирования ---" << endl;

        Circle c1(1, 2, 3.0); // Создание объекта Circle
        Circle c2 = c1; // Использование конструктора копирования

        Rectangle r1(0, 0, 5, 5); // Создание объекта Rectangle
        Rectangle r2 = r1; // Использование конструктора копирования

        RectanglePtr rp1(1, 2, 3, 4); // Создание объекта RectanglePtr
        RectanglePtr rp2 = rp1; // Использование конструктора копирования

    }

    {

        cout << "--- Операторы присваивания ---" << endl;

        Point p1, p2; // Создание двух объектов Point
        p2 = p1; // Использование оператора присваивания

        Circle c1, c2; // Создание двух объектов Circle
        c2 = c1; // Использование оператора присваивания

        Rectangle r1(0, 0, 1, 1), r2(2, 2, 3, 3); // Создание двух объектов Rectangle
        r2 = r1; // Использование оператора присваивания

        RectanglePtr rp1(5, 5, 6, 6), rp2(7, 7, 8, 8); // Создание двух объектов RectanglePtr
        rp2 = rp1; // Использование оператора присваивания

    }

    {

        cout << "--- Пример protected-метода в классе-наследнике ---" << endl;

        Circle c(0, 0, 5.0); // Создание объекта Circle
        // c.setRadius(10.0); // Раскомментировать, если нужно использовать protected-метод

    }

    cout << "Программа завершена, вызываются деструкторы." << endl;

    return 0; // Завершение программы

}
