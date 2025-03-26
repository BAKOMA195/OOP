#include <iostream>

using namespace std;

class Point {
protected:
    int x, y;

public:
    Point() : x(0), y(0) {
        cout << "Конструктор Point()\n";
    }

    Point(int x, int y) : x(x), y(y) {
        cout << "Конструктор Point(" << x << ", " << y << ")\n";
    }

    // Виртуальный деструктор для правильного удаления наследников
    virtual ~Point() {
        cout << "Деструктор ~Point() для точки (" << x << ", " << y << ")\n";
    }

    // Виртуальный метод для демонстрации полиморфизма
    virtual void print() const {
        cout << "Точка: (" << x << ", " << y << ")\n";
    }

    // Геттеры для демонстрации protected
    int getX() const { return x; }
    int getY() const { return y; }

    // Конструктор копирования
    Point(const Point& other) : x(other.x), y(other.y) {
        cout << "Конструктор копирования Point\n";
    }

    // Оператор присваивания
    Point& operator=(const Point& other) {
        if (this == &other) return *this;
        x = other.x;
        y = other.y;
        cout << "Оператор присваивания Point\n";
        return *this;
    }
};

class Circle : public Point {
private:
    double radius;

public:
    Circle() : Point(), radius(1.0) {
        cout << "Конструктор Circle(), радиус = " << radius << "\n";
    }

    Circle(int x, int y, double r) : Point(x, y), radius(r) {
        cout << "Конструктор Circle(" << x << ", " << y << ", " << r << ")\n";
    }

    ~Circle() override {
        cout << "Деструктор ~Circle(), радиус = " << radius << "\n";
    }

    void print() const override {
        cout << "Круг с центром (" << x << ", " << y << ")";
        cout << " и радиусом " << radius << "\n";
    }

    // Конструктор копирования
    Circle(const Circle& other) : Point(other), radius(other.radius) {
        cout << "Конструктор копирования Circle\n";
    }

    // Оператор присваивания
    Circle& operator=(const Circle& other) {
        if (this == &other) return *this;
        Point::operator=(other);
        radius = other.radius;
        cout << "Оператор присваивания Circle\n";
        return *this;
    }
};

class Rectangle {
private:
    Point topLeft;
    Point bottomRight;

public:
    Rectangle(int x1, int y1, int x2, int y2)
        : topLeft(x1, y1), bottomRight(x2, y2) {
        cout << "Конструктор Rectangle(" << x1 << ", " << y1 << ", " << x2 << ", " << y2 << ")\n";
    }

    ~Rectangle() {
        cout << "Деструктор ~Rectangle()\n";
    }

    void print() const {
        cout << "Прямоугольник:\n";
        cout << " Левый верхний ";
        topLeft.print();
        cout << " Правый нижний ";
        bottomRight.print();
    }

    // Конструктор копирования
    Rectangle(const Rectangle& other)
        : topLeft(other.topLeft), bottomRight(other.bottomRight) {
        cout << "Конструктор копирования Rectangle\n";
    }

    // Оператор присваивания
    Rectangle& operator=(const Rectangle& other) {
        if (this == &other) return *this;
        topLeft = other.topLeft;
        bottomRight = other.bottomRight;
        cout << "Оператор присваивания Rectangle\n";
        return *this;
    }
};

class RectanglePtr {
private:
    Point* topLeft;
    Point* bottomRight;

public:
    RectanglePtr(int x1, int y1, int x2, int y2) {
        topLeft = new Point(x1, y1);
        bottomRight = new Point(x2, y2);
        cout << "Конструктор RectanglePtr(" << x1 << ", " << y1 << ", " << x2 << ", " << y2 << ")\n";
    }

    ~RectanglePtr() {
        cout << "Деструктор ~RectanglePtr()\n";
        delete topLeft;
        delete bottomRight;
    }

    void print() const {
        cout << "Прямоугольник (указатели):\n";
        cout << " Левый верхний ";
        topLeft->print();
        cout << " Правый нижний ";
        bottomRight->print();
    }

    // Конструктор копирования с глубоким копированием
    RectanglePtr(const RectanglePtr& other) {
        topLeft = new Point(*other.topLeft);
        bottomRight = new Point(*other.bottomRight);
        cout << "Конструктор копирования RectanglePtr\n";
    }

    // Оператор присваивания с глубоким копированием
    RectanglePtr& operator=(const RectanglePtr& other) {
        if (this == &other) return *this;
        delete topLeft;
        delete bottomRight;
        topLeft = new Point(*other.topLeft);
        bottomRight = new Point(*other.bottomRight);
        cout << "Оператор присваивания RectanglePtr\n";
        return *this;
    }
};

int main() {
    setlocale(LC_ALL, "RU");

    {
        cout << "\n--- Статическое создание объектов ---\n";
        Point p1;
        Point p2(2, 3);
        Circle c1;
        Circle c2(5, 5, 10.0);
        Rectangle r(0, 0, 4, 4);
        RectanglePtr rp(1, 1, 3, 3);
    }

    {
        cout << "\n--- Динамическое создание объектов ---\n";
        Point* p = new Point(10, 20);
        Circle* c = new Circle(1, 2, 3.5);
        delete p;
        delete c;
    }

    {
        cout << "\n--- Наследование и полиморфизм ---\n";
        Point* ptr = new Circle(7, 8, 9.9);
        ptr->print(); // Вызовет Circle::print() благодаря virtual
        delete ptr;   // Вызовет ~Circle() благодаря virtual
    }

    {
        cout << "\n--- Конструкторы копирования ---\n";
        Circle c1(1, 2, 3.0);
        Circle c2 = c1; // Конструктор копирования
        Rectangle r1(0, 0, 5, 5);
        Rectangle r2 = r1; // Конструктор копирования
        RectanglePtr rp1(1, 2, 3, 4);
        RectanglePtr rp2 = rp1; // Конструктор копирования
    }

    {
        cout << "\n--- Операторы присваивания ---\n";
        Point p1, p2;
        p2 = p1;
        Circle c1, c2;
        c2 = c1;
        Rectangle r1(0, 0, 1, 1), r2(2, 2, 3, 3);
        r2 = r1;
        RectanglePtr rp1(5, 5, 6, 6), rp2(7, 7, 8, 8);
        rp2 = rp1;
    }

    cout << "\nПрограмма завершена, вызываются деструкторы.\n";
    return 0;
}