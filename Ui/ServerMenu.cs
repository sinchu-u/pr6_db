using RestaurantOrders.Data.Repository;

namespace RestaurantOrders.Ui;

public class ServerMenu(IServerRepository repo)
{
    public async Task RunAsync()
    {
        while (true)
        {
            ConsoleIO.Header("ОФІЦІАНТИ");
            Console.WriteLine("1. Показати всіх");
            Console.WriteLine("2. Додати");
            Console.WriteLine("3. Змінити ім'я");
            Console.WriteLine("4. Видалити");
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
                        var created = await repo.CreateAsync(ConsoleIO.ReadString("Ім'я офіціанта: "));
                        Console.WriteLine($"  Додано офіціанта з ID {created.ServerId}.");
                        break;
                    case "3":
                        await ListAsync();
                        var id = ConsoleIO.ReadInt("ID офіціанта: ");
                        var updated = await repo.UpdateNameAsync(id, ConsoleIO.ReadString("Нове ім'я: "));
                        Console.WriteLine(updated is null ? "  Офіціанта не знайдено." : "  Ім'я змінено.");
                        break;
                    case "4":
                        await ListAsync();
                        var delId = ConsoleIO.ReadInt("ID офіціанта для видалення: ");
                        Console.WriteLine(await repo.DeleteAsync(delId) ? "  Видалено." : "  Офіціанта не знайдено.");
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
        var servers = await repo.GetAllAsync();
        Console.WriteLine();
        Console.WriteLine($"{"ID",-5}{"Ім'я"}");
        Console.WriteLine(new string('-', 40));
        foreach (var s in servers)
            Console.WriteLine($"{s.ServerId,-5}{s.ServerName}");
        if (servers.Count == 0) Console.WriteLine("  (порожньо)");
    }
}
