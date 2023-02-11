using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AntalyaTaksiAccount.Models
{
    public class ATAccountContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly EnvironmentDetermination _environmentDetermination;

        public ATAccountContext(IConfiguration configuration,EnvironmentDetermination environmentDetermination)
        {
            _configuration = configuration;
            _environmentDetermination = environmentDetermination;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!_environmentDetermination.IsDevelopment)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DevCon"));
            }
            else
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ProdCon"));

            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Driver>().Property(c => c.DriverID).IsRequired();
            modelBuilder.Entity<Driver>().HasKey(c => c.DriverID).IsClustered();
           
           
          
            //modelBuilder.Entity<Driver>().Property(c => c.).IsRequired().HasDefaultValue(0);
            //modelBuilder.Entity<Driver>().Property(c => c.InsertedDate).HasDefaultValue(DateTime.Now).IsRequired();
            //modelBuilder.Entity<Driver>().Property(c => c.PasswordChangeDate).HasDefaultValue(DateTime.Now).IsRequired();
            //modelBuilder.Entity<Driver>().Property(c => c.PasswordExpiration).HasDefaultValue(90).IsRequired();
            //modelBuilder.Entity<Driver>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();

            modelBuilder.Entity<Passenger>().Property(c => c.PassengerID).IsRequired();
            modelBuilder.Entity<Passenger>().HasKey(c => c.PassengerID).IsClustered();
           
           
            //modelBuilder.Entity<Passenger>().Property(c => c.Password).IsRequired();
            //modelBuilder.Entity<Passenger>().Property(c => c.MailVerify).IsRequired().HasDefaultValue(0);
            //modelBuilder.Entity<Passenger>().Property(c => c.ResetPasswordVerify).IsRequired().HasDefaultValue(0);
            //modelBuilder.Entity<Passenger>().Property(c => c.InsertedDate).HasDefaultValue(DateTime.Now).IsRequired();
            //modelBuilder.Entity<Passenger>().Property(c => c.PasswordChangeDate).HasDefaultValue(DateTime.Now).IsRequired();
            //modelBuilder.Entity<Passenger>().Property(c => c.PasswordExpiration).HasDefaultValue(90).IsRequired();
            //modelBuilder.Entity<Passenger>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();
          
            modelBuilder.Entity<Station>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();
            modelBuilder.Entity<Station>().HasKey(c => c.StationID).IsClustered();

            modelBuilder.Entity<Vehicle>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();
            modelBuilder.Entity<Vehicle>().HasKey(c => c.VehicleID).IsClustered();

            modelBuilder.Entity<Role>().Property(c => c.RoleID).IsRequired();
            modelBuilder.Entity<Role>().HasKey(c => c.RoleID).IsClustered();
            modelBuilder.Entity<Role>().Property(c => c.RoleName).HasMaxLength(100);
            modelBuilder.Entity<Role>().Property(c => c.Activity).HasDefaultValue(1).IsRequired();

            modelBuilder.Entity<AllUser>().HasKey(c => c.AllUserID).IsClustered();
            modelBuilder.Entity<AllUser>().Property(c => c.Name).HasMaxLength(100);
            modelBuilder.Entity<AllUser>().Property(c => c.Surname).HasMaxLength(100);
            modelBuilder.Entity<AllUser>().Property(c => c.MailAdress).IsRequired();
            modelBuilder.Entity<AllUser>().Property(c => c.Phone).IsRequired().HasMaxLength(11);
            modelBuilder.Entity<AllUser>().Property(c => c.Password).IsRequired();
            modelBuilder.Entity<AllUser>().Property(c => c.MailVerify).IsRequired().HasDefaultValue(0);


        }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverVehicle> DriverVehicles { get; set; }
        public DbSet<VehicleOwner> VehicleOwners { get; set; }
        public DbSet<AllUser> AllUsers { get; set; }
      
    }
}
