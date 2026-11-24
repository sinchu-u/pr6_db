using RestaurantOrders.Data.Repository;
using RestaurantOrders.Models;

namespace RestaurantOrders.Ui;

public class OrderMenu(IOrderRepository orders, IServerRepository servers, IMenuRepository menu)
{
    public async Task RunAsync()
    {
        while (true)
        {
            ConsoleIO.Header("ЗАМОВЛЕННЯ");
            Console.WriteLine("1. Список замовлень (коротко)");
            Console.WriteLine("2. Деталі замовлення (чек)");
            Console.WriteLine("3. Створити замовлення");
            Console.WriteLine("4. Додати страву до замовлення");
            Console.WriteLine("5. Змінити кількість страви");
            Console.WriteLine("6. Видалити страву із замовлення");
            Console.WriteLine("7. Змінити стіл / офіціанта");
            Console.WriteLine("8. Видалити замовлення");
            Console.WriteLine("0. Назад");

            var choice = ConsoleIO.ReadString("Ваш вибір: ");
            if (choice == "0") return;

            try
            {
                switch (choice)
                {
                    case "1": await ListAsync(); break;
                    case "2": await ShowDetailsAsync(); break;
                    case "3": await CreateAsync(); break;
                    case "4": await AddItemAsync(); break;
                    case "5": await ChangeQuantityAsync(); break;
                    case "6": await RemoveItemAsync(); break;
                    case "7": await UpdateAsync(); break;
                    case "8": await DeleteAsync(); break;
                    default: Console.WriteLine("  Невідомий пункт меню."); break;
                }
            }
            catch (Exception ex)
            {
                ConsoleIO.Error(ex);
            }

            ConsoleIO.Pause();
        }
    }

    private static decimal Total(Order o) =>
        o.Items.Sum(i => i.Quantity * (i.Position?.PositionPrice ?? 0m));

    private async Task ListAsync()
    {
        var all = await orders.GetAllAsync();
        Console.WriteLine();
        Console.WriteLine($"{"№",-5}{"Стіл",-7}{"Офіціант",-22}{"Створено",-20}{"Сума",10}");
        Console.WriteLine(new string('-', 64));
        foreach (var o in all)
            Console.WriteLine(
                $"{o.OrderId,-5}{o.TableNumber,-7}{o.Server?.ServerName ?? "-",-22}{o.CreatedAt,-20:yyyy-MM-dd HH:mm}{ConsoleIO.Money(Total(o)),10}");
        if (all.Count == 0) Console.WriteLine("  (порожньо)");
    }

    private async Task ShowDetailsAsync()
    {
        await ListAsync();
        var id = ConsoleIO.ReadInt("ID замовлення: ");
        var order = await orders.GetByIdAsync(id);
        if (order is null)
        {
            Console.WriteLine("  Замовлення не знайдено.");
            return;
        }
        PrintReceipt(order);
    }

    private static void PrintReceipt(Order o)
    {
        Console.WriteLine();
        Console.WriteLine($"Замовлення №{o.OrderId} | Стіл {o.TableNumber} | Офіціант: {o.Server?.ServerName ?? "-"}");
        Console.WriteLine($"Створено: {o.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"{"ID",-5}{"Страва",-26}{"К-сть",6}{"Ціна",10}{"Сума",12}");
        foreach (var i in o.Items.OrderBy(x => x.ItemId))
        {
            var price = i.Position?.PositionPrice ?? 0m;
            Console.WriteLine(
                $"{i.ItemId,-5}{i.Position?.PositionName ?? "?",-26}{i.Quantity,6}{ConsoleIO.Money(price),10}{ConsoleIO.Money(price * i.Quantity),12}");
        }
        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"{"РАЗОМ:",-47}{ConsoleIO.Money(Total(o)),13}");
    }

    private async Task CreateAsync()
    {
        var table = ConsoleIO.ReadInt("Номер столу: ", 1);

        var allServers = await servers.GetAllAsync();
        if (allServers.Count == 0)
        {
            Console.WriteLine("  Немає жодного офіціанта. Спочатку додайте офіціанта.");
            return;
        }
        foreach (var s in allServers) Console.WriteLine($"  {s.ServerId}. {s.ServerName}");
        var serverId = ConsoleIO.ReadInt("ID офіціанта: ");

        var order = await orders.CreateAsync(table, serverId);
        Console.WriteLine($"  Створено замовлення №{order.OrderId}. Додайте страви (порожній ID — завершити).");

        await AddItemsLoopAsync(order.OrderId);
        var full = await orders.GetByIdAsync(order.OrderId);
        if (full is not null) PrintReceipt(full);
    }

    private async Task AddItemsLoopAsync(int orderId)
    {
        var items = await menu.GetAllAsync();
        foreach (var m in items)
            Console.WriteLine($"  {m.PositionId,-4}{m.PositionName,-26}{ConsoleIO.Money(m.PositionPrice),8}");

        while (true)
        {
            var s = ConsoleIO.ReadString("ID страви (Enter — завершити): ", allowEmpty: true);
            if (s.Length == 0) break;
            if (!int.TryParse(s, out var positionId))
            {
                Console.WriteLine("  Введіть число.");
                continue;
            }
            var qty = ConsoleIO.ReadInt("Кількість: ", 1, 1000);
            try
            {
                await orders.AddItemAsync(orderId, positionId, qty);
                Console.WriteLine("  Додано.");
            }
            catch (Exception ex)
            {
                ConsoleIO.Error(ex);
            }
        }
    }

    private async Task AddItemAsync()
    {
        await ListAsync();
        var orderId = ConsoleIO.ReadInt("ID замовлення: ");
        await AddItemsLoopAsync(orderId);
        var full = await orders.GetByIdAsync(orderId);
        if (full is not null) PrintReceipt(full);
    }

    private async Task ChangeQuantityAsync()
    {
        await ShowDetailsAsync();
        var itemId = ConsoleIO.ReadInt("ID рядка (позиції замовлення): ");
        var qty = ConsoleIO.ReadInt("Нова кількість: ", 1, 1000);
        Console.WriteLine(await orders.UpdateItemQuantityAsync(itemId, qty) ? "  Оновлено." : "  Рядок не знайдено.");
    }

    private async Task RemoveItemAsync()
    {
        await ShowDetailsAsync();
        var itemId = ConsoleIO.ReadInt("ID рядка для видалення: ");
        Console.WriteLine(await orders.RemoveItemAsync(itemId) ? "  Видалено." : "  Рядок не знайдено.");
    }

    private async Task UpdateAsync()
    {
        await ListAsync();
        var id = ConsoleIO.ReadInt("ID замовлення: ");
        var table = ConsoleIO.ReadInt("Новий номер столу: ", 1);
        foreach (var s in await servers.GetAllAsync()) Console.WriteLine($"  {s.ServerId}. {s.ServerName}");
        var serverId = ConsoleIO.ReadInt("ID нового офіціанта: ");
        var updated = await orders.UpdateAsync(id, table, serverId);
        Console.WriteLine(updated is null ? "  Замовлення не знайдено." : "  Оновлено.");
    }

    private async Task DeleteAsync()
    {
        await ListAsync();
        var id = ConsoleIO.ReadInt("ID замовлення для видалення: ");
        if (!ConsoleIO.Confirm("Видалити замовлення разом з усіма його позиціями?")) return;
        Console.WriteLine(await orders.DeleteAsync(id) ? "  Видалено." : "  Замовлення не знайдено.");
    }
}
