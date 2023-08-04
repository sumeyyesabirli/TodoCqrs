using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Dal
{
    public class CalendarContext : DbContext
    {
        public CalendarContext(DbContextOptions<CalendarContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp").Entity<Event>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
        }
        public DbSet<Event> Events { get; set; }

    }
}
