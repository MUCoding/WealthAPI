using Microsoft.EntityFrameworkCore;

namespace WealthAPI.Data
{
    public class WealthContext : DbContext
    {
        public WealthContext(DbContextOptions<WealthContext> options) : base(options)
        {
        }

        public DbSet<WealthObject> WealthObjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
