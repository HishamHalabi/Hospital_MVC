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
      
        public DbSet<Prescription> prescriptions { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Payment> Payments { get; set; }
    
        public DbSet<PreMed> PreMeds { get; set; }

        public DbSet<NurseSurgery>NurseSurgeries { get; set; }

        public DbSet<DoctorSurgery> DoctorSurgeries { get; set; }

        public DbSet<Scan> Scans { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for PreMed
            modelBuilder.Entity<PreMed>()
                .HasKey(pr => new { pr.MedID, pr.PreID });

            modelBuilder.Entity<NurseSurgery>()
             .HasKey(pr => new { pr.NurseId, pr.SurgeryId });

            modelBuilder.Entity<DoctorSurgery>()
            .HasKey(pr => new { pr.DoctorId, pr.SurgeryId });

            // One-to-One: Appointment <--> Prescription
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Appointment)
                .WithOne(a => a.Prescription)
                .HasForeignKey<Prescription>(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-One: Bill <--> Payment
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Bill)
                .HasForeignKey<Payment>(p => p.BillId)
                .OnDelete(DeleteBehavior.Restrict); // Or .Cascade

            base.OnModelCreating(modelBuilder);
        }

    }
}
