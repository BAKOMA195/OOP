#include <iostream>
#include <string>
#include <vector>
#include <memory>   // Для unique_ptr, make_unique
#include <typeinfo> // Для dynamic_cast
#include <clocale>  // Для setlocale
#include <iomanip>  // Для boolalpha

using namespace std;

// Базовый класс: Еда
class Food {

public:

    string name; // Поле класса

    // Конструктор Food: Инициализация 'name' присваиванием в теле
    Food(string n = "Еда") {
        this->name = n;
        cout << "Конструктор Food: '" << name << "'" << endl;
    }

    // Виртуальный деструктор (тело можно оставить пустым)
    virtual ~Food() {
        cout << "Деструктор Food: '" << name << "'" << endl;
    }

    // Метод 1: Возвращает имя класса
    virtual string classname() const {
        return "Food";
    }

    // Метод 2: Проверяет иерархию
    virtual bool isA(const string& classname_to_check) const {
        return classname_to_check == "Food";
    }

    // Виртуальный метод для вывода информации
    virtual void printInfo() const {
        cout << "Это объект Food: " << name << endl;
    }
};

// Класс-потомок 1: Фрукт
class Fruit : public Food {
public:
    // Конструктор Fruit: Инициализация БАЗОВОГО КЛАССА обязательна в списке
    Fruit(string n = "Фрукт") : Food(n) {
        cout << "Конструктор Fruit: '" << name << "'" << endl;
    }

    // Деструктор (тело пустое)
    ~Fruit() override {
        cout << "Деструктор Fruit: '" << name << "'" << endl;
    }

    // Переопределяем методы базового класса
    string classname() const override {
        return "Fruit";
    }

    bool isA(const string& classname_to_check) const override {
        if (classname_to_check == "Fruit") {
            return true;
        }
        return Food::isA(classname_to_check); // Проверка вверх по иерархии
    }

    void printInfo() const override {
        cout << "Это объект Fruit: " << name << endl;
    }

    // Специфичный метод Fruit
    void peel() const {
        cout << "Чистим фрукт '" << name << "'." << endl;
    }
};

// Класс-потомок 2: Овощ
class Vegetable : public Food {

public:

    // Конструктор Vegetable: Инициализация БАЗОВОГО КЛАССА обязательна в списке
    Vegetable(string n = "Овощ") : Food(n) {
        cout << "Конструктор Vegetable: '" << name << "'" << endl;
    }

    // Деструктор
    ~Vegetable() override {
        cout << "Деструктор Vegetable: '" << name << "'" << endl;
    }

    // Переопределяем методы базового класса
    string classname() const override {
        return "Vegetable";
    }

    bool isA(const string& classname_to_check) const override {
        if (classname_to_check == "Vegetable") {
            return true;
        }
        return Food::isA(classname_to_check); // Проверка вверх по иерархии
    }

    void printInfo() const override {
        cout << "Это объект Vegetable: " << name << endl;
    }

    // Специфичный метод Vegetable
    void wash() const {
        cout << "  Моем овощ '" << name << "'." << endl;
    }
};


// Функция для демонстрации опасного приведения типов
void tryUnsafeCastToFruit(Food* ptr) {
    cout << endl << "Попытка НЕБЕЗОПАСНОГО приведения к Fruit* " << endl; 
    Fruit* unsafe_fruit_ptr = static_cast<Fruit*>(ptr);
    cout << "Небезопасное приведение выполнено (static_cast)" << endl; 
    if (ptr->classname() != "Fruit") {
        cout << "Попытка вызова метода Fruit::peel() через неверно приведенный указатель может вызвать краш" << endl; 
    }
    else {
        cout << " Тип оказался верным, вызов метода отработает" << endl; 
        unsafe_fruit_ptr->peel(); // Опасно без гарантии типа! peel() выводит endl
    }
}


int main() {

    setlocale(LC_ALL, "RU");

    // Используем умные указатели для упрощения управления памятью

    vector<unique_ptr<Food>> foods;

    foods.push_back(make_unique<Food>("Хлеб"));
    foods.push_back(make_unique<Fruit>("Апельсин"));
    foods.push_back(make_unique<Vegetable>("Морковь"));

    cout << endl << "Обработка продуктов " << endl; 

    for (const auto& food_uptr : foods) {

        Food* ptr = food_uptr.get(); // Получаем сырой указатель для работы

        cout << endl << "Информация об объекте:" << endl; 
        ptr->printInfo(); // Полиморфный вызов, printInfo() использует endl

        // 1. Использование classname()
        cout << "classname(): " << ptr->classname() << endl; 

        // 2. Использование isA()
        // Используем std::boolalpha для вывода true/false вместо 1/0
        cout << "  isA(\"Food\"):      " << boolalpha << ptr->isA("Food") << endl; 
        cout << "  isA(\"Fruit\"):     " << boolalpha << ptr->isA("Fruit") << endl; 
        cout << "  isA(\"Vegetable\"): " << boolalpha << ptr->isA("Vegetable") << endl; 

        // 3. Опасное приведение типов (демонстрация)
        if (ptr->classname() == "Vegetable") { // Найдем Овощ, чтобы показать проблему
            tryUnsafeCastToFruit(ptr); // Эта функция использует endl
        }

        // 4. Безопасное приведение типов (вручную с isA)
        cout << "Попытка ручного безопасного приведения (с isA) " << endl; 
        if (ptr->isA("Fruit")) {
            Fruit* fruit_ptr_manual = static_cast<Fruit*>(ptr);
            cout << "  Ручное приведение к Fruit* успешно (использовали isA)." << endl; 
            fruit_ptr_manual->peel(); // peel() использует endl
        }
        else {
            cout << "Ручное приведение к Fruit* не удалось (объект не Fruit)." << endl; 
        }

        // 5. Безопасное приведение типов (dynamic_cast)
        cout << "Попытка dynamic_cast " << endl; 
        // Попытка приведения к Fruit*
        Fruit* fruit_ptr_dynamic = dynamic_cast<Fruit*>(ptr);
        if (fruit_ptr_dynamic != nullptr) {
            cout << "  dynamic_cast к Fruit* успешен." << endl; 
            fruit_ptr_dynamic->peel(); // peel() использует endl
        }
        else {
            cout << "dynamic_cast к Fruit* не удался" << endl; 
        }

        // Попытка приведения к Vegetable*
        Vegetable* veg_ptr_dynamic = dynamic_cast<Vegetable*>(ptr);
        if (veg_ptr_dynamic) { // Краткая проверка на nullptr
            cout << "  dynamic_cast к Vegetable* успешен." << endl; 
            veg_ptr_dynamic->wash(); // wash() использует endl
        }
        else {
            cout << "dynamic_cast к Vegetable* не удался." << endl; 
        }
    }

    // Очистка памяти не нужна вручную, т.к. использовали unique_ptr
    cout << endl << "Объекты будут автоматически удалены unique_ptr при выходе из main" << endl; 

    return 0;
}
