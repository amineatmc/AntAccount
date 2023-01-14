using Microsoft.EntityFrameworkCore;

namespace AntalyaTaksiAccount.Models
{
    public class ATAccountContext : DbContext
    {
        private readonly IConfiguration _configuration;
       
        public ATAccountContext(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DevCon"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(c=>c.UserID).IsRequired();
            modelBuilder.Entity<User>().HasKey(c => c.UserID).IsClustered();
        }
    }
}
