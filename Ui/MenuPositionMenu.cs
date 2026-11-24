using RestaurantOrders.Data.Repository;

namespace RestaurantOrders.Ui;

public class MenuPositionMenu(IMenuRepository repo)
{
    public async Task RunAsync()
    {
        while (true)
        {
            ConsoleIO.Header("МЕНЮ РЕСТОРАНУ");
            Console.WriteLine("1. Показати всі позиції");
            Console.WriteLine("2. Додати позицію");
            Console.WriteLine("3. Змінити позицію");
            Console.WriteLine("4. Видалити позицію");
            Console.WriteLine("0. Назад");

            var choice = ConsoleIO.ReadString("Ваш вибір: ");
            if (choice == "0") return;

            try
            {
                switch (choice)
                {
                    case "1":
                        await ListAsync();
                        break;
                    case "2":
                        var name = ConsoleIO.ReadString("Назва: ");
                        var amount = ConsoleIO.ReadDouble("Вага/об'єм порції (г/мл): ");
                        var price = ConsoleIO.ReadDecimal("Ціна: ");
                        var created = await repo.CreateAsync(name, amount, price);
                        Console.WriteLine($"  Додано позицію з ID {created.PositionId}.");
                        break;
                    case "3":
                        await ListAsync();
                        var id = ConsoleIO.ReadInt("ID позиції: ");
                        var n = ConsoleIO.ReadString("Нова назва: ");
                        var a = ConsoleIO.ReadDouble("Нова вага/об'єм: ");
                        var p = ConsoleIO.ReadDecimal("Нова ціна: ");
                        var updated = await repo.UpdateAsync(id, n, a, p);
                        Console.WriteLine(updated is null ? "  Позицію не знайдено." : "  Позицію оновлено.");
                        break;
                    case "4":
                        await ListAsync();
                        var delId = ConsoleIO.ReadInt("ID позиції для видалення: ");
                        Console.WriteLine(await repo.DeleteAsync(delId) ? "  Видалено." : "  Позицію не знайдено.");
                        break;
                    default:
                        Console.WriteLine("  Невідомий пункт меню.");
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleIO.Error(ex);
            }

            ConsoleIO.Pause();
        }
    }

    private async Task ListAsync()
    {
        var items = await repo.GetAllAsync();
        Console.WriteLine();
        Console.WriteLine($"{"ID",-5}{"Назва",-28}{"Порція",-10}{"Ціна",10}");
        Console.WriteLine(new string('-', 53));
        foreach (var m in items)
            Console.WriteLine($"{m.PositionId,-5}{m.PositionName,-28}{m.PositionAmount,-10}{ConsoleIO.Money(m.PositionPrice),10}");
        if (items.Count == 0) Console.WriteLine("  (порожньо)");
    }
}
