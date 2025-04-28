#include <iostream>
#include <string>
#include <stdexcept> // Для bad_cast
#include <clocale>   // Для setlocale

using namespace std;

// --- Базовый класс: Еда ---
class Food {
protected:
    string id;

public:
    // Конструктор по умолчанию: присваивание id в теле
    Food(string name = "Еда") {
        this->id = name;
        cout << "  Конструктор Food (умолч.): [" << id << "]" << endl;
    }

    // Конструктор копирования (стандартный): присваивание id в теле
    Food(const Food& other) {
        this->id = other.id + "_копия"; // Добавим суффикс для ясности
        cout << "  Конструктор Food (КОПИР. const&): с [" << other.id << "] на [" << id << "]" << endl;
    }

    // Конструктор из УКАЗАТЕЛЯ (нестандартный, по требованию ЛР)
    Food(Food* obj) {
        if (obj) {
            this->id = obj->id + "_из_указателя"; // Добавим суффикс
            cout << "  Конструктор Food (ИЗ УКАЗАТЕЛЯ *): с [" << obj->id << "] на [" << id << "]" << endl;
        }
        else {
            this->id = "Еда_из_null_указателя";
            cout << "  Конструктор Food (ИЗ УКАЗАТЕЛЯ *): Получен nullptr, создан [" << id << "]" << endl;
        }
    }

    // Деструктор
    virtual ~Food() {
        cout << "  Деструктор Food: [" << id << "]" << endl;
    }

    // Виртуальный метод
    virtual void consume() const {
        cout << "  Потребляем: [" << id << "]" << endl;
    }

    // Геттер
    string getID() const { return id; }
};

// --- Класс-потомок: Напиток ---
class Drink : public Food {
public:
    // Конструктор по умолчанию: БАЗОВЫЙ КЛАСС инициализируется в списке
    Drink(string name = "Напиток") : Food(name) {
        cout << "  Конструктор Drink (умолч.): [" << id << "]" << endl;
    }

    // Конструктор копирования (стандартный): БАЗОВЫЙ КЛАСС инициализируется в списке
    Drink(const Drink& other) : Food(other) { // Вызывает Food(const Food&)
        cout << "  Конструктор Drink (КОПИР. const&): с [" << other.id << "] на [" << id << "]" << endl;
    }

    // Конструктор из УКАЗАТЕЛЯ (нестандартный, по требованию ЛР)
    // Инициализируем базовую часть Food, используя конструктор Food(Food*)
    Drink(Drink* obj) : Food(obj) { // Вызывает Food(Food*), т.к. Drink* -> Food*
        // id уже инициализирован базовым конструктором Food(Food*)
        if (obj) {
            cout << "  Конструктор Drink (ИЗ УКАЗАТЕЛЯ *): с [" << obj->id << "] (базовый установил [" << id << "])" << endl;
        }
        else {
            cout << "  Конструктор Drink (ИЗ УКАЗАТЕЛЯ *): Получен nullptr (базовый установил [" << id << "])" << endl;
        }
    }


    // Деструктор
    ~Drink() override {
        cout << "  Деструктор Drink: [" << id << "]" << endl;
    }

    // Переопределение виртуального метода
    void consume() const override {
        cout << "  Выпиваем: [" << id << "]" << endl;
    }

    // Специфичный метод потомка
    void pour() const {
        cout << "  Наливаем напиток: [" << id << "]" << endl;
    }
};

// --- Функции для демонстрации ---

// 1. Передача по значению (вызывает КОНСТРУКТОР КОПИРОВАНИЯ const&)
void func1_value(Food food_copy) {
    cout << "--- Внутри func1_value(Food food_copy) ---" << endl;
    cout << " Параметр food_copy: id=" << food_copy.getID() << endl;
    food_copy.consume(); // Вызовется Food::consume из-за срезки

    Drink* d_ptr = dynamic_cast<Drink*>(&food_copy);
    if (!d_ptr) {
        cout << "  Невозможно привести копию к Drink* внутри func1_value (произошла срезка)." << endl;
    }

    cout << "--- Выход из func1_value(Food food_copy) ---" << endl;
} // Деструктор food_copy выведет свое сообщение

// 2. Передача по указателю (конструкторы НЕ вызываются при передаче)
void func2_pointer(Food* food_ptr) {
    cout << "--- Внутри func2_pointer(Food* food_ptr) ---" << endl;
    if (!food_ptr) {
        cout << " Получен nullptr!" << endl;
        cout << "--- Выход из func2_pointer(Food* food_ptr) ---" << endl;
        return;
    }
    cout << " Параметр food_ptr указывает на: id=" << food_ptr->getID() << endl;
    food_ptr->consume(); // Полиморфный вызов

    Drink* d_ptr = dynamic_cast<Drink*>(food_ptr);
    if (d_ptr) {
        cout << "  dynamic_cast к Drink* успешен внутри func2_pointer." << endl;
        d_ptr->pour();
    }
    else {
        cout << "  dynamic_cast к Drink* не удался внутри func2_pointer." << endl;
    }
    cout << "--- Выход из func2_pointer(Food* food_ptr) ---" << endl;
}

// 3. Передача по ссылке (конструкторы НЕ вызываются при передаче)
void func3_reference(Food& food_ref) {
    cout << "--- Внутри func3_reference(Food& food_ref) ---" << endl;
    cout << " Параметр food_ref ссылается на: id=" << food_ref.getID() << endl;
    food_ref.consume(); // Полиморфный вызов

    try {
        Drink& d_ref = dynamic_cast<Drink&>(food_ref);
        cout << "  dynamic_cast<Drink&> успешен внутри func3_reference." << endl;
        d_ref.pour();
    }
    catch (const bad_cast& e) {
        cout << "  dynamic_cast<Drink&> не удался внутри func3_reference (ссылается не на Drink): " << e.what() << endl;
    }
    cout << "--- Выход из func3_reference(Food& food_ref) ---" << endl;
}


int main() {
    // Установка русской локали
    setlocale(LC_ALL, "RU");

    cout << "=== Создаем объекты ===" << endl;
    Food bread("Хлеб");
    cout << "------" << endl;
    Drink water("Вода");
    cout << "======================" << endl << endl;


    cout << endl << ">>> Вызов func1_value (передача по ЗНАЧЕНИЮ) с объектом Food <<<" << endl;
    // Вызовет Food(const Food&) для создания food_copy
    func1_value(bread);
    cout << "----------------------------------------" << endl;

    cout << endl << ">>> Вызов func1_value (передача по ЗНАЧЕНИЮ) с объектом Drink <<<" << endl;
    // Вызовет Food(const Food&) для создания food_copy (СРЕЗКА!)
    func1_value(water);
    cout << "----------------------------------------" << endl;


    cout << endl << ">>> Вызов func2_pointer (передача по УКАЗАТЕЛЮ) с объектом Food <<<" << endl;
    // Конструкторы не вызываются
    func2_pointer(&bread);
    cout << "----------------------------------------" << endl;

    cout << endl << ">>> Вызов func2_pointer (передача по УКАЗАТЕЛЮ) с объектом Drink <<<" << endl;
    // Конструкторы не вызываются
    func2_pointer(&water);
    cout << "----------------------------------------" << endl;


    cout << endl << ">>> Вызов func3_reference (передача по ССЫЛКЕ) с объектом Food <<<" << endl;
    // Конструкторы не вызываются
    func3_reference(bread);
    cout << "----------------------------------------" << endl;

    cout << endl << ">>> Вызов func3_reference (передача по ССЫЛКЕ) с объектом Drink <<<" << endl;
    // Конструкторы не вызываются
    func3_reference(water);
    cout << "----------------------------------------" << endl;

    // --- Демонстрация ЯВНОГО вызова конструкторов из указателя ---
    cout << endl << ">>> ЯВНЫЙ вызов конструкторов из указателя <<<" << endl;
    cout << "Создаем f_ptr_constructed из &bread:" << endl;
    Food f_ptr_constructed(&bread); // Явный вызов Food(Food*)
    cout << "------" << endl;
    cout << "Создаем d_ptr_constructed из &water:" << endl;
    Drink d_ptr_constructed(&water); // Явный вызов Drink(Drink*) -> Food(Food*)
    cout << "------" << endl;
    cout << "Создаем f_null_constructed из nullptr:" << endl;
    Food f_null_constructed(nullptr); // Явный вызов Food(Food*) с nullptr
    cout << "------" << endl;
    cout << "Создаем d_null_constructed из nullptr:" << endl;
    Drink d_null_constructed(nullptr); // Явный вызов Drink(Drink*) -> Food(Food*) с nullptr
    cout << "------" << endl;


    cout << endl << "=== Конец main() ===" << endl;
    // Деструкторы для bread, water, f_ptr_constructed, d_ptr_constructed, f_null_constructed, d_null_constructed
    return 0;
}
