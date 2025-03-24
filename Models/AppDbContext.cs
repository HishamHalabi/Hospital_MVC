using Microsoft.EntityFrameworkCore;
using Hospital.Models;

namespace Hospital
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Room> Rooms { get; set; }
       
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Surgery> Surgeries { get; set; }
        public DbSet<Description> Descriptions { get; set; }
        public DbSet<Prespection> Prespection { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<NurseRoom> NurseRooms { get; set; } // Added NurseRoom
        public DbSet<PreMed> PreMeds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-Many Relationship: Nurse <-> Room
            modelBuilder.Entity<NurseRoom>()
                .HasKey(nr => new { nr.NurseID, nr.RoomID });

           

            modelBuilder.Entity<NurseRoom>()
                .HasOne(nr => nr.Nurse)
                .WithMany(n => n.NurseRooms)
                .HasForeignKey(nr => nr.NurseID);

            modelBuilder.Entity<NurseRoom>()
                .HasOne(nr => nr.Room)
                .WithMany(r => r.NurseRooms)
                .HasForeignKey(nr => nr.RoomID);

            // Many-to-Many Relationship: Prescription <-> Medicine (Corrected to PreMed)
            modelBuilder.Entity<PreMed>()
                .HasKey(pm => new { pm.MedID, pm.PreID });

            modelBuilder.Entity<PreMed>()
                .HasOne(pm => pm.Medicine)
                .WithMany(m => m.PreMeds)
                .HasForeignKey(pm => pm.MedID);

            modelBuilder.Entity<PreMed>()
                .HasOne(pm => pm.Prespection)
                .WithMany(p => p.PreMeds)
                .HasForeignKey(pm => pm.PreID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
