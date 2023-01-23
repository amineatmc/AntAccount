using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AntalyaTaksiAccount.Models
{
    public class ATAccountContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ATAccountContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DevCon"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(c => c.UserID).IsRequired();
            modelBuilder.Entity<User>().HasKey(c => c.UserID).IsClustered();
            modelBuilder.Entity<User>().Property(c => c.Name).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(c => c.Surname).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(c => c.MailAdress).IsRequired();
            modelBuilder.Entity<User>().Property(c => c.Phone).IsRequired().HasMaxLength(11);
            modelBuilder.Entity<User>().Property(c => c.Password).IsRequired();
            modelBuilder.Entity<User>().Property(c => c.MailVerify).IsRequired().HasDefaultValue(0);
            modelBuilder.Entity<User>().Property(c => c.ResetPasswordVerify).IsRequired().HasDefaultValue(0);
            modelBuilder.Entity<User>().Property(c => c.InsertedDate).HasDefaultValue(DateTime.Now).IsRequired();
            modelBuilder.Entity<User>().Property(c => c.PasswordChangeDate).HasDefaultValue(DateTime.Now).IsRequired();
            modelBuilder.Entity<User>().Property(c => c.PasswordExpiration).HasDefaultValue(90).IsRequired();
            modelBuilder.Entity<User>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();

            modelBuilder.Entity<Passenger>().Property(c => c.PassengerID).IsRequired();
            modelBuilder.Entity<Passenger>().HasKey(c => c.PassengerID).IsClustered();
            modelBuilder.Entity<Passenger>().Property(c => c.Name).HasMaxLength(100);
            modelBuilder.Entity<Passenger>().Property(c => c.Surname).HasMaxLength(100);
            modelBuilder.Entity<Passenger>().Property(c => c.MailAdress).IsRequired();
            modelBuilder.Entity<Passenger>().Property(c => c.Phone).IsRequired().HasMaxLength(11);
            modelBuilder.Entity<Passenger>().Property(c => c.Password).IsRequired();
            modelBuilder.Entity<Passenger>().Property(c => c.MailVerify).IsRequired().HasDefaultValue(0);
            modelBuilder.Entity<Passenger>().Property(c => c.ResetPasswordVerify).IsRequired().HasDefaultValue(0);
            modelBuilder.Entity<Passenger>().Property(c => c.InsertedDate).HasDefaultValue(DateTime.Now).IsRequired();
            modelBuilder.Entity<Passenger>().Property(c => c.PasswordChangeDate).HasDefaultValue(DateTime.Now).IsRequired();
            modelBuilder.Entity<Passenger>().Property(c => c.PasswordExpiration).HasDefaultValue(90).IsRequired();
            modelBuilder.Entity<Passenger>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();

            modelBuilder.Entity<Department>().Property(c => c.DepartmentID).IsRequired();
            modelBuilder.Entity<Department>().HasKey(c => c.DepartmentID).IsClustered();
            modelBuilder.Entity<Department>().Property(c => c.DepartmentName).HasMaxLength(100);
            modelBuilder.Entity<Department>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();

            modelBuilder.Entity<Company>().Property(c => c.CompanyID).IsRequired();
            modelBuilder.Entity<Company>().HasKey(c => c.CompanyID).IsClustered();
            modelBuilder.Entity<Company>().Property(c => c.CompanyName).HasMaxLength(100);
            modelBuilder.Entity<Company>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();

            modelBuilder.Entity<Gender>().Property(c => c.GenderID).IsRequired();
            modelBuilder.Entity<Gender>().HasKey(c => c.GenderID).IsClustered();
            modelBuilder.Entity<Gender>().Property(c => c.GenderName).HasMaxLength(100);
            modelBuilder.Entity<Gender>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();

            modelBuilder.Entity<Role>().Property(c => c.RoleID).IsRequired();
            modelBuilder.Entity<Role>().HasKey(c => c.RoleID).IsClustered();
            modelBuilder.Entity<Role>().Property(c => c.RoleName).HasMaxLength(100);
            modelBuilder.Entity<Role>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();


        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
