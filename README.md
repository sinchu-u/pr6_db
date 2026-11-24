# Практична робота №6 — ORM (Entity Framework Core + PostgreSQL)

Консольний застосунок «Облік замовлень у ресторані» на C# (.NET 8).

## Вимоги
- .NET SDK 8.0+
- Docker Desktop (для PostgreSQL)
- Інструмент міграцій: `dotnet tool install --global dotnet-ef --version 8.*`

## Запуск
```bash
# 1. База даних (з теки проєкту)
docker compose up -d

# 2. Міграція (створює таблиці з C#-моделей)
dotnet ef migrations add InitialCreate
dotnet ef database update

# 3. Запуск застосунку (сідінг тестових даних виконується автоматично)
dotnet run
```
Якщо міграцій немає, застосунок сам створить схему через `EnsureCreated`,
але для звіту краще виконати кроки з міграціями.

Скинути базу: `docker compose down -v`, потім заново пункти 1–3.

## Структура
| Папка / файл | Призначення |
|---|---|
| `Models/` | Сутності: `Server`, `Order`, `OrderItem`, `MenuPosition` (+ `OrderTotalDto` для сирого SQL) |
| `Data/AppDbContext.cs` | `DbContext`, `DbSet`, `OnModelCreating` (ключі, типи, зв'язки) |
| `Data/AppDbContextFactory.cs` | Потрібна лише для `dotnet ef` |
| `Data/Repository/` | Репозиторії (інтерфейс + реалізація): CRUD через ORM, сирі SQL |
| `Data/DatabaseSeeder.cs` | Початкові дані |
| `Services/ReportService.cs` | LINQ-звіти та сирий SQL (`SqlQuery`) |
| `Ui/` | Консольні меню |
| `Program.cs` | DI-контейнер, міграція, сідінг, головне меню |

## Що демонструється
- **CRUD через ORM** для всіх таблиць (меню, офіціанти, замовлення та їх позиції).
- **LINQ**: фільтрація, сортування, групування, `Include/ThenInclude`.
- **Сирий SQL**: `FromSql`, `SqlQuery<T>` (JOIN + GROUP BY).
- **SQL-ін'єкції**: пункт меню 5 → 3 (вразливо) і 4/5 (безпечно), пункт 6 — порівняння.
  Введіть `' OR '1'='1` — вразливий запит поверне всю таблицю, а безпечні — 0 рядків.
- **Міграції та сідінг**, **Dependency Injection** (`AddScoped`).
- **Цілісність даних**: не можна видалити офіціанта із замовленнями або страву, що є у замовленні.

## Відповідність SQL-схемі
Таблиці й назви колонок збігаються з наданим SQL (`SERIAL`, `varchar`, `numeric`,
`timestamp DEFAULT now()`, `NOT NULL`). Зв'язки — без `ON DELETE CASCADE`,
тому позиції замовлення видаляються явно в `OrderRepository.DeleteAsync`.
