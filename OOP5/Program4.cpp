// program4_food_return_no_init_list_no_addr.cpp
#include <iostream>
#include <string>
#include <utility> // для :move
#include <clocale> // для setlocale

using namespace std;

// --- Класс для демонстрации: Блюдо ---
class Dish {
public:
    string name;

    // Конструктор по умолчанию: Инициализация присваиванием
    Dish(string n = "Безымянное блюдо") {
        this->name = n;
        cout << "  Конструктор Dish: Приготовлено [" << name << "]" << endl; // Убран адрес, endl
    }

    // Конструктор копирования: Инициализация присваиванием
    Dish(const Dish& other) {
        this->name = other.name + "_копия";
        cout << "  КОНСТРУКТОР КОПИРОВАНИЯ Dish: с [" << other.name << "] на [" << name << "]" << endl; // Убраны адреса, endl
    }

    // Конструктор перемещения: Инициализация присваиванием (через move)
    Dish(Dish&& other) noexcept {
        // Используем присваивание вместо инициализации в списке
        this->name = move(other.name) + "_перемещено";
        other.name = "Блюдо_перемещено_из_" + this->name; // Используем this->name, т.к. оно уже содержит новое значение
        cout << "  КОНСТРУКТОР ПЕРЕМЕЩЕНИЯ Dish: с [" << other.name << "] на [" << name << "]" << endl; // Убраны адреса, endl
    }

    // Оператор присваивания перемещением
    Dish& operator=(Dish&& other) noexcept {
        if (this != &other) { // Проверка на самоприсваивание
            this->name = move(other.name) + "_присвоено_перемещ";
            other.name = "Блюдо_присвоено_из_" + this->name;
            cout << "  ОПЕРАТОР ПРИСВАИВАНИЯ ПЕРЕМЕЩЕНИЕМ Dish: с [" << other.name << "] на [" << name << "]" << endl; // Убраны адреса, endl
        }
        return *this;
    }

    // Оператор присваивания копированием
    Dish& operator=(const Dish& other) {
        if (this != &other) { // Проверка на самоприсваивание
            this->name = other.name + "_присвоено_копир";
            cout << "  ОПЕРАТОР ПРИСВАИВАНИЯ КОПИРОВАНИЕМ Dish: с [" << other.name << "] на [" << name << "]" << endl; // Убраны адреса, endl
        }
        return *this;
    }


    // Деструктор
    ~Dish() {
        cout << "  Деструктор Dish: Блюдо [" << name << "] съедено (уничтожено)" << endl; // Убран адрес, endl
    }

    // Метод serve
    void serve() const {
        cout << "  Подача блюда: [" << name << "]" << endl; // Убран адрес, endl
    }
};

// === Возврат ЛОКАЛЬНЫХ (автоматических, на стеке) объектов ===

// 1. Возврат по значению (локальный объект)
Dish makeDish_local_val() {
    cout << "-- Вход в makeDish_local_val --" << endl; // endl
    Dish local_dish("Суп");
    local_dish.serve();
    cout << "-- Выход из makeDish_local_val --" << endl; // endl
    return local_dish;
}

// 2. Возврат УКАЗАТЕЛЯ на локальный объект (ОПАСНО!)
Dish* makeDish_local_ptr() {
    cout << "-- Вход в makeDish_local_ptr --" << endl; // endl
    Dish local_dish("Салат");
    local_dish.serve();
    Dish* ptr = &local_dish;
    cout << "-- Выход из makeDish_local_ptr (возвращаем ВИСЯЧИЙ указатель!) --" << endl; // endl
    return ptr;
} // local_dish УНИЧТОЖАЕТСЯ ЗДЕСЬ!

// 3. Возврат ССЫЛКИ на локальный объект (ОПАСНО!)
Dish& makeDish_local_ref() {
    cout << "-- Вход в makeDish_local_ref --" << endl; // endl
    Dish local_dish("Компот");
    local_dish.serve();
    cout << "-- Выход из makeDish_local_ref (возвращаем ВИСЯЧУЮ ссылку!) --" << endl; // endl
    return local_dish;
} // local_dish УНИЧТОЖАЕТСЯ ЗДЕСЬ!

// === Возврат ДИНАМИЧЕСКИХ (в куче, heap) объектов ===

// 4. Возврат по значению (динамический объект -> возврат КОПИИ, УТЕЧКА!)
Dish makeDish_dynamic_val() {
    cout << "-- Вход в makeDish_dynamic_val --" << endl; // endl
    Dish* dynamic_dish = new Dish("Жаркое_динам");
    dynamic_dish->serve();
    Dish copy_to_return = *dynamic_dish; // Вызов конструктора копирования
    cout << "!!! ЗАБЫЛИ УДАЛИТЬ dynamic_dish -> УТЕЧКА ПАМЯТИ !!!" << endl; // endl
    // delete dynamic_dish; // <- Вот что нужно было сделать!
    cout << "-- Выход из makeDish_dynamic_val --" << endl; // endl
    return copy_to_return;
}

// 5. Возврат УКАЗАТЕЛЯ на динамический объект (КОРРЕКТНО, но ручное управление)
Dish* makeDish_dynamic_ptr() {
    cout << "-- Вход в makeDish_dynamic_ptr --" << endl; // endl
    Dish* dynamic_dish = new Dish("Рыба_динам");
    dynamic_dish->serve();
    cout << "-- Выход из makeDish_dynamic_ptr (возвращаем указатель на кучу) --" << endl; // endl
    return dynamic_dish; // Вызывающий код отвечает за delete
}

// 6. Возврат ССЫЛКИ на динамический объект (ПЛОХОЙ ДИЗАЙН!)
Dish& makeDish_dynamic_ref() {
    cout << "-- Вход в makeDish_dynamic_ref --" << endl; // endl
    Dish* dynamic_dish = new Dish("Десерт_динам");
    dynamic_dish->serve();
    cout << "-- Выход из makeDish_dynamic_ref (возвращаем ссылку на кучу - ПЛОХОЙ ДИЗАЙН!) --" << endl; // endl
    return *dynamic_dish; // Кто удалит? Неясно.
}

int main() {
    // Установка русской локали
    setlocale(LC_ALL, "RU");

    cout << endl << "======= Функция 1: Возврат по значению (локальный) =======" << endl; // endl
    Dish result1 = makeDish_local_val();
    cout << "  main: Получено блюдо result1 "; result1.serve();
    cout << "--------------------------------------------------" << endl; // endl

    cout << endl << "======= Функция 2: Возврат указателя (локальный) =======" << endl; // endl
    Dish* result2_ptr = makeDish_local_ptr();
    cout << "  main: Получен result2_ptr (указывает на освобожденную память стека!)" << endl; // Убрано значение указателя
    if (result2_ptr) { // Сам указатель может быть не nullptr, но память уже невалидна
        cout << "  main: !!! РАЗЫМЕНОВАНИЕ ВИСЯЧЕГО УКАЗАТЕЛЯ -> НЕОПРЕДЕЛЕННОЕ ПОВЕДЕНИЕ (ПАДЕНИЕ/МУСОР) !!!" << endl; // endl
        // result2_ptr->serve(); // <-- НЕ РАСКОММЕНТИРОВЫВАТЬ!
    }
    cout << "  main: Нельзя делать 'delete result2_ptr', это не куча." << endl; // endl
    cout << "--------------------------------------------------" << endl; // endl

    cout << endl << "======= Функция 3: Возврат ссылки (локальный) =======" << endl; // endl
    cout << "  main: Попытка использовать результат func3 (висячая ссылка)..." << endl; // endl
    try {
        cout << "  main: !!! ИСПОЛЬЗОВАНИЕ ВИСЯЧЕЙ ССЫЛКИ -> НЕОПРЕДЕЛЕННОЕ ПОВЕДЕНИЕ !!!" << endl; // endl
        Dish& ref_from_func3 = makeDish_local_ref();
        // ref_from_func3.serve(); // <-- НЕ РАСКОММЕНТИРОВЫВАТЬ!
        cout << "  main: Ссылка получена (но она уже невалидна)" << endl; // endl

    }
    catch (...) {
        cout << "Перехвачено исключение (маловероятно, скорее всего UB)" << endl; // endl
    }
    cout << "--------------------------------------------------" << endl; // endl

    cout << endl << "======= Функция 4: Возврат по значению (динамический - УТЕЧКА!) =======" << endl; // endl
    Dish result4 = makeDish_dynamic_val();
    cout << "  main: Получено блюдо result4 "; result4.serve();
    cout << "  main: !!! ПАМЯТЬ УТЕКЛА внутри makeDish_dynamic_val !!!" << endl; // endl
    cout << "--------------------------------------------------" << endl; // endl

    cout << endl << "======= Функция 5: Возврат указателя (динамический) =======" << endl; // endl
    Dish* result5_ptr = makeDish_dynamic_ptr();
    cout << "  main: Получен result5_ptr (указывает на объект в куче)" << endl; // Убрано значение указателя
    if (result5_ptr) {
        cout << "  main: Используем result5_ptr "; result5_ptr->serve();
        cout << "  main: !!! ВЫЗЫВАЮЩИЙ КОД ОБЯЗАН сделать delete result5_ptr !!!" << endl; // endl
        delete result5_ptr;
        result5_ptr = nullptr;
        cout << "  main: Память освобождена (delete выполнен)." << endl; // endl
    }
    cout << "--------------------------------------------------" << endl; // endl

    cout << endl << "======= Функция 6: Возврат ссылки (динамический - ПЛОХО!) =======" << endl; // endl
    Dish* leaked_dish_ptr = nullptr; // Используем только для возможности delete
    try {
        cout << "  main: Вызов makeDish_dynamic_ref для получения ссылки..." << endl; // endl
        Dish& result6_ref = makeDish_dynamic_ref();
        cout << "  main: Получена ссылка result6_ref. Подача: "; result6_ref.serve();
        cout << "  main: !!! ПРОБЛЕМА: Кто владеет объектом? Кто должен вызвать delete? !!!" << endl; // endl
        cout << "  main: Невозможно надежно удалить объект по ссылке. Вероятна УТЕЧКА!" << endl; // endl
        leaked_dish_ptr = &result6_ref; // Получаем адрес через ссылку ТОЛЬКО для демо-очистки
    }
    catch (...) {
        cout << "Перехвачено исключение (маловероятно)." << endl; // endl
    }

    if (leaked_dish_ptr) {
        cout << "  main: (Демо-очистка) Удаляем объект вручную..." << endl; // Убрано значение указателя
        delete leaked_dish_ptr;
    }

    cout << "--------------------------------------------------" << endl; // endl


    cout << endl << "======= Конец main =======" << endl; // endl
    return 0;
}
