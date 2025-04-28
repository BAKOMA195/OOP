#include <iostream>
#include <string>
#include <clocale> // Для setlocale

using namespace std;

// Базовый класс: Еда
class Food {
public:
    string name; // Поле класса

    // Конструктор Food: Инициализация члена 'name' через присваивание в теле
    Food(string n = "Какая-то еда") {
        this->name = n;
        cout << "Конструктор Food: Создан объект '" << name << "'" << endl; 
    }

    // Невиртуальный метод (перекрываемый)
    void chop() {
        cout << "Food::chop(): Нарезаем '" << name << "' базовым способом." << endl; 
    }

    // Виртуальный метод (переопределяемый)
    virtual void taste() {
        cout << "Food::taste(): Пробуем '" << name << "'. Вкус неопределенный." << endl; 
    }

    // Метод, вызывающий другие методы этого класса
    void prepareAndTaste() {
        cout << endl << "Вызов методов из Food::prepareAndTaste() для '" << name << "':" << endl; 
        cout << "  Вызов chop(): ";
        chop(); // chop() сам выводит endl
        cout << "  Вызов taste(): ";
        taste(); // taste() сам выводит endl
        cout << "Завершение Food::prepareAndTaste()" << endl; 
    }

    // Виртуальный деструктор
    virtual ~Food() {
        cout << "Деструктор Food: Уничтожен объект '" << name << "'" << endl; 
    }
};

// Класс-потомок: Фрукт
class Fruit : public Food {

public:

    bool peeled = false; // Поле класса Fruit

    // Конструктор Fruit: Инициализация базового класса ДОЛЖНА остаться в списке
    Fruit(string n = "Какой-то фрукт") : Food(n) { // <-- Вызов конструктора БАЗОВОГО КЛАССА остается здесь!
        cout << "Конструктор Fruit: Создан объект '" << name << "'" << endl; 
    }

    // Перекрытие невиртуального метода
    void chop() {
        cout << "Fruit::chop(): Нарезали '" << name << "'." << endl; 
    }

    // Переопределение виртуального метода
    void taste() override {
        cout << "Fruit::taste(): Пробуем фрукт " << name << endl; 
    }

    // Специфичный метод Fruit
    void peel() {
        peeled = true;
        cout << "Fruit::peel(): Чистим фрукт '" << name << endl; 
    }

    // Деструктор Fruit
    ~Fruit() override {
        cout << "Деструктор Fruit: Выбросили '" << name << "'" << endl; 
    }
};

int main() {

    setlocale(LC_ALL, "RU");

    cout << "Создаем объект Fruit напрямую" << endl; 
    Fruit apple("Яблоко"); // Сначала Food("Яблоко"), потом Fruit("Яблоко")

    cout << endl << "Вызываем методы объекта Fruit напрямую" << endl; 
    apple.chop();   // Вызывается Fruit::chop()
    apple.taste();  // Вызывается Fruit::taste()
    apple.prepareAndTaste(); // Вызывается Food::prepareAndTaste()

    cout << endl << "Создаем объект Fruit через указатель Food*" << endl; 
    Food* foodPtr = new Fruit("Банан"); // Вызываются конструкторы Food, затем Fruit

    cout << endl << "Вызываем методы через указатель Food*" << endl; 
    foodPtr->chop(); // Вызывается Food::chop() !!! (невиртуальный)
    foodPtr->taste();    // Вызывается Fruit::taste() (виртуальный)
    foodPtr->prepareAndTaste(); // Вызывается Food::prepareAndTaste()

    cout << endl << "Вызываем методы через указатель Fruit* (после приведения)" << endl; 
    Fruit* fruitPtr = static_cast<Fruit*>(foodPtr);
    fruitPtr->chop();    // Вызывается Fruit::chop()
    fruitPtr->taste();   // Вызывается Fruit::taste()
    fruitPtr->peel();    // Вызвается метод Fruit

    cout << endl << "Удаляем объект через указатель Food*" << endl; 
    delete foodPtr; // Вызывается ~Fruit(), потом ~Food() (из-за виртуального деструктора)

    // apple выходит из области видимости, вызываются ~Fruit(), потом ~Food()

    return 0;
}
