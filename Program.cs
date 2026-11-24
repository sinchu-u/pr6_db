using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantOrders.Data;
using RestaurantOrders.Data.Repository;
using RestaurantOrders.Services;
using RestaurantOrders.Ui;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var services = new ServiceCollection();
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("postgres")));
services.AddScoped<IServerRepository, ServerRepository>();
services.AddScoped<IMenuRepository, MenuRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<ReportService>();

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var sp = scope.ServiceProvider;

try
{
    var db = sp.GetRequiredService<AppDbContext>();

    if (db.Database.GetMigrations().Any())
        await db.Database.MigrateAsync();
    else
        await db.Database.EnsureCreatedAsync();

    await DatabaseSeeder.SeedAsync(db);
}
catch (Exception ex)
{
    Console.WriteLine("Не вдалося підключитися до бази даних або ініціалізувати її.");
    Console.WriteLine($"Причина: {ex.InnerException?.Message ?? ex.Message}");
    return 1;
}

var serverMenu = new ServerMenu(sp.GetRequiredService<IServerRepository>());
var positionMenu = new MenuPositionMenu(sp.GetRequiredService<IMenuRepository>());
var orderMenu = new OrderMenu(
    sp.GetRequiredService<IOrderRepository>(),
    sp.GetRequiredService<IServerRepository>(),
    sp.GetRequiredService<IMenuRepository>());
var reportMenu = new ReportMenu(sp.GetRequiredService<ReportService>());

while (true)
{
    ConsoleIO.Header("РЕСТОРАН — ОБЛІК ЗАМОВЛЕНЬ");
    Console.WriteLine("1. Замовлення");
    Console.WriteLine("2. Меню ресторану");
    Console.WriteLine("3. Офіціанти");
    Console.WriteLine("4. Звіти (LINQ)");
    Console.WriteLine("0. Вихід");

    switch (ConsoleIO.ReadString("Ваш вибір: "))
    {
        case "1": await orderMenu.RunAsync(); break;
        case "2": await positionMenu.RunAsync(); break;
        case "3": await serverMenu.RunAsync(); break;
        case "4": await reportMenu.RunAsync(); break;
        case "0":
            Console.WriteLine("До побачення!");
            return 0;
        default:
            Console.WriteLine("Невідомий пункт меню.");
            break;
    }
}
