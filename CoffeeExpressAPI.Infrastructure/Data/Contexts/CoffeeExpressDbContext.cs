using Microsoft.EntityFrameworkCore;

namespace CoffeeExpressAPI.Infrastructure.Data.Contexts
{
    public class CoffeeExpressDbContext : DbContext
    {
        public CoffeeExpressDbContext(DbContextOptions<CoffeeExpressDbContext> options) : base(options) { }

        // TODO: DbSets se agregarán en Sprint 1.2 cuando creemos las entidades
        // public DbSet<Coffee> Coffees { get; set; }
        // public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: Configuraciones de entidades se agregarán en Sprint 1.2
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoffeeExpressDbContext).Assembly);
        }
    }
}
