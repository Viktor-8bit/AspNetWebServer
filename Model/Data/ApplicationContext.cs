using AspNetWebServer.Controllers;
using Microsoft.EntityFrameworkCore;
using AspNetWebServer.Model.Data.ProcessMonitoring;

namespace AspNetWebServer.Model.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<LoginHistory> LoginHistoryLog { get; set; }
        public DbSet<infoSecuritySpecialist> infoSecuritySpecialists { get; set; }
        public DbSet<Pc> Pcs { get; set; }
        public DbSet<MountedProcess> MountedProcesses { get; set; }
        public DbSet<ProcessAction> ProcessActions { get; set; }
        public DbSet<Utilization> Utilizations { get; set; }
        
        public string connectionString;

        public ApplicationContext()
        {
            this.connectionString = "Host=localhost;port=4356;Database=spyapp;Username=postgres;Password=5IODgzvwHK8zwXt1KKlzHJPxIluK1CPI;";   // получаем извне строку подключения
            // Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.HasPostgresExtension("btree_gin");
            
            
            modelBuilder.Entity<ProcessAction>()
                .HasOne(pa => pa.MountedProces)
                .WithMany(mp => mp.ProcessActions)
                .HasForeignKey(pa => pa.MountedProcessId);
            
            modelBuilder.Entity<ProcessAction>()
                .HasIndex(pa => new { pa.Id, pa.Date, pa.MountedProcessId })
                .HasDatabaseName("ProcessAction_Id_Date_MountedProcessId_Index")
                .HasMethod("btree");
            
            modelBuilder.Entity<MountedProcess>()
                .HasIndex(Mp => new { Mp.Id, Mp.MonutedIndex })
                .HasDatabaseName("MountedProcess_Id_MonutedIndex_Index")
                .HasMethod("btree");
            
            modelBuilder.Entity<Pc>()
                .HasIndex(Pc => new { Pc.Id, Pc.hostname })
                .HasDatabaseName("Pc_Id_Hostname_Index")
                .HasMethod("btree");
            

        }
    }
}
