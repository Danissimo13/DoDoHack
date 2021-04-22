using DoDoModels;
using Microsoft.EntityFrameworkCore;

namespace DoDoHack.Data
{
    public class DodoBase : DbContext
    {
        public DodoBase(DbContextOptions<DodoBase> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>();
            modelBuilder.Entity<Courier>().HasBaseType<User>();
            modelBuilder.Entity<Admin>().HasBaseType<User>();

            modelBuilder.Entity<Courier>()
                        .HasMany(e => e.WorkZones)
                        .WithMany(e => e.PinnedCouriers)
                        .UsingEntity<CourierWorkZone>(
                            e => e.HasOne(cw => cw.WorkZone)
                                    .WithMany()
                                    .HasForeignKey(cw => cw.WorkZoneId),
                            e => e.HasOne(cw => cw.PinnedCourier)
                                    .WithMany()
                                    .HasForeignKey(cw => cw.PinnedCourierId),
                            e => e.HasKey(cw => cw.Id)
                         );

            modelBuilder.Entity<Courier>()
                        .HasMany(e => e.Orders)
                        .WithOne(e => e.Courier)
                        .HasForeignKey(e => e.CourierId);

            modelBuilder.Entity<Courier>()
                        .HasMany(e => e.CourierActions)
                        .WithOne(e => e.Courier)
                        .HasForeignKey(e => e.CourierId);

            modelBuilder.Entity<Courier>()
                        .HasOne(e => e.Statistic)
                        .WithOne()
                        .HasForeignKey<Courier>(e => e.StatisticId);

            modelBuilder.Entity<Courier>()
                        .HasOne(e => e.OrdersVision)
                        .WithOne()
                        .HasForeignKey<Courier>(e => e.OrdersVisionId);

            modelBuilder.Entity<Courier>()
                        .HasMany(e => e.Tracks)
                        .WithOne(e => e.Courier)
                        .HasForeignKey(e => e.CourierId);

            modelBuilder.Entity<Order>()
                        .HasOne(e => e.WorkZone)
                        .WithMany()
                        .HasForeignKey(e => e.WorkZoneId)
                        .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                        .HasMany(e => e.Products)
                        .WithMany(e => e.Orders)
                        .UsingEntity<OrderProduct>(
                            e => e.HasOne(op => op.Product)
                                  .WithMany()
                                  .HasForeignKey(op => op.ProductId),
                            e => e.HasOne(op => op.Order)
                                  .WithMany()
                                  .HasForeignKey(op => op.OrderId),
                            e => e.HasKey(e => e.Id)
                        );

            modelBuilder.Entity<LineChatMessage>();
            modelBuilder.Entity<CourierChatMessage>();
            modelBuilder.Entity<ChatMessage>();

            modelBuilder.Entity<News>();

            modelBuilder.Entity<Track>().Property(e => e.Latitude).HasColumnType("DECIMAL(38, 20)");
            modelBuilder.Entity<Track>().Property(e => e.Longitude).HasColumnType("DECIMAL(38, 20)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
