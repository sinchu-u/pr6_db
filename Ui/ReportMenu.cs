using RestaurantOrders.Services;

namespace RestaurantOrders.Ui;

public class ReportMenu(ReportService reports)
{
    public async Task RunAsync()
    {
        while (true)
        {
            ConsoleIO.Header("ЗВІТИ");
            Console.WriteLine("1. Замовлення та виручка по офіціантах");
            Console.WriteLine("2. Топ-5 найпопулярніших страв");
            Console.WriteLine("3. Меню за діапазоном цін (фільтр + сортування)");
            Console.WriteLine("0. Назад");

            var choice = ConsoleIO.ReadString("Ваш вибір: ");
            if (choice == "0") return;

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.WriteLine();
                        Console.WriteLine($"{"Офіціант",-24}{"Замовлень",10}{"Виручка",12}");
                        Console.WriteLine(new string('-', 46));
                        foreach (var s in await reports.GetServerStatsAsync())
                            Console.WriteLine($"{s.ServerName,-24}{s.OrdersCount,10}{ConsoleIO.Money(s.Revenue),12}");
                        break;
                    case "2":
                        Console.WriteLine();
                        var rank = 1;
                        foreach (var p in await reports.GetTopPositionsAsync(5))
                            Console.WriteLine($"{rank++}. {p.PositionName,-26} {p.TotalQuantity} шт.");
                        break;
                    case "3":
                        var min = ConsoleIO.ReadDecimal("Мінімальна ціна: ");
                        var max = ConsoleIO.ReadDecimal("Максимальна ціна: ", min);
                        Console.WriteLine();
                        foreach (var m in await reports.GetMenuByPriceRangeAsync(min, max))
                            Console.WriteLine($"{m.PositionName,-28}{ConsoleIO.Money(m.PositionPrice),10}");
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
}
