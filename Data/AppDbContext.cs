using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Models;

namespace RestaurantOrders.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Server> Servers => Set<Server>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<MenuPosition> MenuPositions => Set<MenuPosition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Server>(e =>
        {
            e.ToTable("servers");
            e.HasKey(x => x.ServerId);
            e.Property(x => x.ServerId).HasColumnName("server_id").UseSerialColumn();
            e.Property(x => x.ServerName).HasColumnName("server_name").HasColumnType("varchar").IsRequired();
        });

        modelBuilder.Entity<MenuPosition>(e =>
        {
            e.ToTable("menu_position");
            e.HasKey(x => x.PositionId);
            e.Property(x => x.PositionId).HasColumnName("position_id").UseSerialColumn();
            e.Property(x => x.PositionName).HasColumnName("position_name").HasColumnType("varchar").IsRequired();
            e.Property(x => x.PositionAmount).HasColumnName("position_amount").HasColumnType("double precision");
            e.Property(x => x.PositionPrice).HasColumnName("position_price").HasColumnType("numeric");
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("orders");
            e.HasKey(x => x.OrderId);
            e.Property(x => x.OrderId).HasColumnName("order_id").UseSerialColumn();
            e.Property(x => x.TableNumber).HasColumnName("table_number").IsRequired();
            e.Property(x => x.ServerId).HasColumnName("server_id");
            e.Property(x => x.CreatedAt).HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("now()");

            e.HasOne(x => x.Server)
                .WithMany(s => s.Orders)
                .HasForeignKey(x => x.ServerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("order_items");
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemId).HasColumnName("item_id").UseSerialColumn();
            e.Property(x => x.OrderId).HasColumnName("order_id");
            e.Property(x => x.PositionId).HasColumnName("position_id");
            e.Property(x => x.Quantity).HasColumnName("quantity").IsRequired();

            e.HasOne(x => x.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Position)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(x => x.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
