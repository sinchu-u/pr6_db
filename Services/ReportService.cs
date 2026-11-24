using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Data;
using RestaurantOrders.Models;

namespace RestaurantOrders.Services;

public record ServerStat(string ServerName, int OrdersCount, decimal Revenue);
public record PositionStat(string PositionName, int TotalQuantity);
public class ReportService(AppDbContext db)
{
    public async Task<List<ServerStat>> GetServerStatsAsync()
    {
        var counts = await db.Servers
            .AsNoTracking()
            .Select(s => new { s.ServerId, s.ServerName, OrdersCount = s.Orders.Count })
            .ToListAsync();

        var revenue = await db.OrderItems
            .AsNoTracking()
            .Where(i => i.Order!.ServerId != null)
            .GroupBy(i => i.Order!.ServerId!.Value)
            .Select(g => new
            {
                ServerId = g.Key,
                Revenue = g.Sum(i => (decimal?)(i.Quantity * i.Position!.PositionPrice)) ?? 0m
            })
            .ToDictionaryAsync(x => x.ServerId, x => x.Revenue);

        return counts
            .Select(c => new ServerStat(c.ServerName, c.OrdersCount, revenue.GetValueOrDefault(c.ServerId)))
            .OrderByDescending(s => s.Revenue)
            .ToList();
    }
    public async Task<List<PositionStat>> GetTopPositionsAsync(int top = 5)
    {
        var data = await db.OrderItems
            .AsNoTracking()
            .Where(i => i.Position != null)
            .GroupBy(i => i.Position!.PositionName)
            .Select(g => new { Name = g.Key, Qty = g.Sum(i => i.Quantity) })
            .OrderByDescending(x => x.Qty)
            .Take(top)
            .ToListAsync();

        return data.Select(x => new PositionStat(x.Name, x.Qty)).ToList();
    }

    public Task<List<MenuPosition>> GetMenuByPriceRangeAsync(decimal min, decimal max) =>
        db.MenuPositions
            .AsNoTracking()
            .Where(m => m.PositionPrice >= min && m.PositionPrice <= max)
            .OrderBy(m => m.PositionPrice)
            .ToListAsync();

    public Task<List<OrderTotalDto>> GetOrderTotalsRawAsync(decimal minTotal) =>
        db.Database.SqlQuery<OrderTotalDto>($"""
            SELECT o.order_id AS "OrderId",
                   o.table_number AS "TableNumber",
                   COALESCE(SUM(oi.quantity * mp.position_price), 0) AS "Total"
            FROM orders o
            LEFT JOIN order_items oi ON oi.order_id = o.order_id
            LEFT JOIN menu_position mp ON mp.position_id = oi.position_id
            GROUP BY o.order_id, o.table_number
            HAVING COALESCE(SUM(oi.quantity * mp.position_price), 0) >= {minTotal}
            ORDER BY o.order_id
            """).ToListAsync();
}
