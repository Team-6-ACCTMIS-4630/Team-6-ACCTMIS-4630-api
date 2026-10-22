using Team6.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Team6.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
    }
}