using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Models;

namespace RestaurantOrders.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Servers.AnyAsync())
        {
            db.Servers.AddRange(
                new Server { ServerName = "Іван Петренко" },
                new Server { ServerName = "Марія Іваненко" },
                new Server { ServerName = "Олег Шевченко" });
            await db.SaveChangesAsync();
        }

        if (!await db.MenuPositions.AnyAsync())
        {
            db.MenuPositions.AddRange(
                new MenuPosition { PositionName = "Борщ з пампушками", PositionAmount = 350, PositionPrice = 120m },
                new MenuPosition { PositionName = "Цезар з куркою", PositionAmount = 250, PositionPrice = 165m },
                new MenuPosition { PositionName = "Стейк рибай", PositionAmount = 300, PositionPrice = 520m },
                new MenuPosition { PositionName = "Паста карбонара", PositionAmount = 320, PositionPrice = 210m },
                new MenuPosition { PositionName = "Тірамісу", PositionAmount = 150, PositionPrice = 110m },
                new MenuPosition { PositionName = "Лимонад", PositionAmount = 400, PositionPrice = 70m },
                new MenuPosition { PositionName = "Кава американо", PositionAmount = 200, PositionPrice = 55m });
            await db.SaveChangesAsync();
        }

        if (!await db.Orders.AnyAsync())
        {
            var servers = await db.Servers.OrderBy(s => s.ServerId).ToListAsync();
            var menu = await db.MenuPositions.OrderBy(m => m.PositionId).ToListAsync();

            db.Orders.AddRange(
                new Order
                {
                    TableNumber = 1,
                    ServerId = servers[0].ServerId,
                    Items =
                    {
                        new OrderItem { PositionId = menu[0].PositionId, Quantity = 2 },
                        new OrderItem { PositionId = menu[5].PositionId, Quantity = 2 }
                    }
                },
                new Order
                {
                    TableNumber = 4,
                    ServerId = servers[1].ServerId,
                    Items =
                    {
                        new OrderItem { PositionId = menu[2].PositionId, Quantity = 1 },
                        new OrderItem { PositionId = menu[3].PositionId, Quantity = 1 },
                        new OrderItem { PositionId = menu[4].PositionId, Quantity = 2 }
                    }
                },
                new Order
                {
                    TableNumber = 7,
                    ServerId = servers[0].ServerId,
                    Items =
                    {
                        new OrderItem { PositionId = menu[1].PositionId, Quantity = 3 },
                        new OrderItem { PositionId = menu[6].PositionId, Quantity = 3 }
                    }
                });
            await db.SaveChangesAsync();
        }
    }
}
