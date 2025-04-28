// program5_food_smartptr_no_init_list_endl.cpp
#include <iostream>
#include <string>
#include <memory> // Для умных указателей (unique_ptr, shared_ptr, make_unique, make_shared)
#include <vector>
#include <utility> // для move
#include <clocale> // для setlocale

using namespace std;

// --- Класс для демонстрации: Ингредиент ---
class Ingredient {
private:
    string name; // Поле класса
public:
    // Конструктор: инициализация name через присваивание в теле
    Ingredient(const string& n) {
        this->name = n; // Присваивание вместо инициализации в списке
        cout << "  Ингредиент '" << name << "' получен (Конструктор)." << endl; // Используем endl
    }

    // Деструктор
    ~Ingredient() {
        cout << "  Ингредиент '" << name << "' использован/выброшен (Деструктор)." << endl; // Используем endl
    }

    // Метод use
    void use() const {
        cout << "  Используем ингредиент '" << name << "'." << endl; // Используем endl
    }

    // Метод getName
    string getName() const { return name; }
};

// --- Функции, работающие с умными указателями ---

// Принимает unique_ptr по значению (требует move) - ЗАБИРАЕТ владение
void process_unique(unique_ptr<Ingredient> unique_ing) {
    cout << "--- Внутри process_unique ---" << endl; // Используем endl
    if (unique_ing) {
        cout << "  Получен unique_ptr (владение передано)." << endl; // Используем endl
        unique_ing->use(); // use() использует endl
    }
    else {
        cout << "  Получен пустой unique_ptr." << endl; // Используем endl
    }
    cout << "--- Выход из process_unique ---" << endl; // Используем endl
} // unique_ing выходит из области видимости, Ингредиент удаляется (деструктор Ingredient использует endl)

// Принимает shared_ptr по значению (копированием) - увеличивает счетчик ссылок
void share_ingredient(shared_ptr<Ingredient> shared_ing) {
    cout << "--- Внутри share_ingredient ---" << endl; // Используем endl
    if (shared_ing) {
        cout << "  Получен shared_ptr. Счетчик ссылок: " << shared_ing.use_count() << endl; // Используем endl
        shared_ing->use(); // use() использует endl
    }
    else {
        cout << "  Получен пустой shared_ptr." << endl; // Используем endl
    }
    cout << "--- Выход из share_ingredient ---" << endl; // Используем endl
} // shared_ing (копия) выходит из области видимости, счетчик ссылок уменьшается

// Принимает shared_ptr по КОНСТАНТНОЙ ссылке - счетчик не меняется, эффективно
void observe_shared(const shared_ptr<Ingredient>& shared_ing_ref) {
    cout << "--- Внутри observe_shared ---" << endl; // Используем endl
    if (shared_ing_ref) {
        cout << "  Получена ссылка на shared_ptr. Счетчик ссылок: " << shared_ing_ref.use_count() << endl; // Используем endl
        shared_ing_ref->use(); // use() использует endl
    }
    else {
        cout << "  Получена пустая ссылка на shared_ptr." << endl; // Используем endl
    }
    cout << "--- Выход из observe_shared ---" << endl; // Используем endl
} // Ссылка перестает существовать

// Фабричная функция, возвращающая unique_ptr
unique_ptr<Ingredient> buy_unique_ingredient(const string& name) {
    cout << "--- Внутри buy_unique_ingredient ---" << endl; // Используем endl
    auto ptr = make_unique<Ingredient>(name); // make_unique вызовет конструктор, который использует endl
    cout << "--- Выход из buy_unique_ingredient ---" << endl; // Используем endl
    return ptr;
}

// Фабричная функция, возвращающая shared_ptr
shared_ptr<Ingredient> buy_shared_ingredient(const string& name) {
    cout << "--- Внутри buy_shared_ingredient ---" << endl; // Используем endl
    auto ptr = make_shared<Ingredient>(name); // make_shared вызовет конструктор, который использует endl
    cout << "  Начальный счетчик ссылок внутри buy_shared_ingredient: " << ptr.use_count() << endl; // Используем endl
    cout << "--- Выход из buy_shared_ingredient ---" << endl; // Используем endl
    return ptr;
}


int main() {
    // Установка русской локали
    setlocale(LC_ALL, "RU");

    cout << "===== Демонстрация unique_ptr =====" << endl << endl; // Используем endl

    cout << "--- Создание unique_ptr --- " << endl; // Используем endl
    auto uptr_carrot = make_unique<Ingredient>("Морковь"); // Конструктор использует endl
    unique_ptr<Ingredient> uptr_onion(new Ingredient("Лук")); // Конструктор использует endl

    if (uptr_carrot) uptr_carrot->use(); // use() использует endl
    if (uptr_onion) uptr_onion->use();   // use() использует endl

    cout << endl << "--- Перемещение unique_ptr --- " << endl; // Используем endl
    unique_ptr<Ingredient> uptr_moved_to;

    cout << " До перемещения: uptr_carrot " << (uptr_carrot ? "владеет ресурсом" : "пустой")
        << ", uptr_moved_to " << (uptr_moved_to ? "владеет ресурсом" : "пустой") << endl; // Используем endl

    uptr_moved_to = move(uptr_carrot);

    cout << " После перемещения: uptr_carrot " << (uptr_carrot ? "владеет ресурсом" : "пустой")
        << ", uptr_moved_to " << (uptr_moved_to ? "владеет ресурсом" : "пустой") << endl; // Используем endl

    if (uptr_moved_to) uptr_moved_to->use(); // use() использует endl

    cout << endl << "--- Передача unique_ptr в функцию (process_unique) --- " << endl; // Используем endl
    process_unique(move(uptr_onion)); // Внутри process_unique используются endl

    cout << " После process_unique: uptr_onion " << (uptr_onion ? "владеет ресурсом" : "пустой") << endl; // Используем endl

    cout << endl << "--- Получение unique_ptr из функции --- " << endl; // Используем endl
    auto uptr_potato = buy_unique_ingredient("Картофель"); // Внутри buy_unique_ingredient используются endl
    cout << " В main, после buy_unique_ingredient:" << endl; // Используем endl
    if (uptr_potato) uptr_potato->use(); // use() использует endl

    cout << endl << "--- unique_ptr выходят из области видимости --- " << endl; // Используем endl
    // Деструкторы выведут сообщения с endl

    cout << endl << endl << "===== Демонстрация shared_ptr =====" << endl << endl; // Используем endl

    shared_ptr<Ingredient> sptr_salt;

    cout << "--- Создание shared_ptr --- " << endl; // Используем endl
    sptr_salt = make_shared<Ingredient>("Соль"); // Конструктор использует endl
    cout << " sptr_salt - счетчик ссылок: " << sptr_salt.use_count() << endl; // = 1

    shared_ptr<Ingredient> sptr_pepper(new Ingredient("Перец")); // Конструктор использует endl
    cout << " sptr_pepper - счетчик ссылок: " << sptr_pepper.use_count() << endl; // = 1


    cout << endl << "--- Копирование shared_ptr --- " << endl; // Используем endl
    shared_ptr<Ingredient> sptr_salt_copy = sptr_salt;
    cout << " После копирования:" << endl; // Используем endl
    cout << "   sptr_salt - счетчик ссылок: " << sptr_salt.use_count() << endl;      // = 2
    cout << "   sptr_salt_copy - счетчик ссылок: " << sptr_salt_copy.use_count() << endl; // = 2
    sptr_salt_copy->use(); // use() использует endl


    cout << endl << "--- Передача shared_ptr в функции --- " << endl; // Используем endl
    cout << " Вызов share_ingredient (передача по значению - копия увеличит счетчик):" << endl; // Используем endl
    share_ingredient(sptr_salt); // Внутри используются endl
    cout << " После share_ingredient, sptr_salt счетчик: " << sptr_salt.use_count() << endl; // = 2

    cout << " Вызов observe_shared (передача по ссылке - счетчик не изменится):" << endl; // Используем endl
    observe_shared(sptr_salt); // Внутри используются endl
    cout << " После observe_shared, sptr_salt счетчик: " << sptr_salt.use_count() << endl; // = 2


    cout << endl << "--- Получение shared_ptr из функции --- " << endl; // Используем endl
    auto sptr_sugar = buy_shared_ingredient("Сахар"); // Внутри используются endl
    cout << " В main, после buy_shared_ingredient. Счетчик sptr_sugar: " << sptr_sugar.use_count() << endl; // = 1


    cout << endl << "--- Сброс shared_ptr (reset) --- " << endl; // Используем endl
    cout << " До sptr_salt_copy.reset(), счетчик sptr_salt: " << sptr_salt.use_count() << endl; // = 2
    sptr_salt_copy.reset(); // Уменьшает счетчик ссылок для "Соли".
    cout << " После sptr_salt_copy.reset():" << endl; // Используем endl
    cout << "   sptr_salt - счетчик ссылок: " << sptr_salt.use_count() << endl;      // = 1
    cout << "   sptr_salt_copy " << (sptr_salt_copy ? "владеет ресурсом" : "пустой") << endl; // Используем endl


    cout << endl << "--- shared_ptr выходят из области видимости --- " << endl; // Используем endl
    // Деструкторы выведут сообщения с endl, когда счетчик дойдет до 0

    cout << endl << "===== Конец main =====" << endl; // Используем endl
    return 0;
}
