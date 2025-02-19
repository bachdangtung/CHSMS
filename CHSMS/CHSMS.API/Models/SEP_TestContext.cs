using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CHSMS.API.Models
{
    public partial class SEP_TestContext : DbContext
    {
        public SEP_TestContext()
        {
        }

        public SEP_TestContext(DbContextOptions<SEP_TestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<MedicalInventory> MedicalInventories { get; set; } = null!;
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public virtual DbSet<MedicalSupply> MedicalSupplies { get; set; } = null!;
        public virtual DbSet<MedicalUsage> MedicalUsages { get; set; } = null!;
        public virtual DbSet<Medicine> Medicines { get; set; } = null!;
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public virtual DbSet<Prescription> Prescriptions { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<SupplyConsumptionDetail> SupplyConsumptionDetails { get; set; } = null!;
        public virtual DbSet<SupplyConsumptionReport> SupplyConsumptionReports { get; set; } = null!;
        public virtual DbSet<SupplyInventory> SupplyInventories { get; set; } = null!;
        public virtual DbSet<SupplySettlementReport> SupplySettlementReports { get; set; } = null!;
        public virtual DbSet<TransferedPatient> TransferedPatients { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<VaccinationRecord> VaccinationRecords { get; set; } = null!;
        public virtual DbSet<Vaccine> Vaccines { get; set; } = null!;
        public virtual DbSet<VaccineInventory> VaccineInventories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Console.WriteLine(Directory.GetCurrentDirectory());
                IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
                var strConn = config["ConnectionStrings:SEP_DB"];
                optionsBuilder.UseSqlServer(strConn);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(e => e.AppointmentId)
                    .ValueGeneratedNever()
                    .HasColumnName("AppointmentID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__Appointme__Patie__48CFD27E");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Appointme__UserI__49C3F6B7");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.DepartmentId)
                    .ValueGeneratedNever()
                    .HasColumnName("DepartmentID");

                entity.Property(e => e.DepartmentName).HasMaxLength(255);
            });

            modelBuilder.Entity<MedicalInventory>(entity =>
            {
                entity.ToTable("MedicalInventory");

                entity.Property(e => e.MedicalInventoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("MedicalInventoryID");

                entity.Property(e => e.DecisionLetter).HasMaxLength(255);

                entity.Property(e => e.MedicineId).HasColumnName("MedicineID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.HasOne(d => d.Medicine)
                    .WithMany(p => p.MedicalInventories)
                    .HasForeignKey(d => d.MedicineId)
                    .HasConstraintName("FK__MedicalIn__Medic__4AB81AF0");
            });

            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.ToTable("MedicalRecord");

                entity.Property(e => e.MedicalRecordId)
                    .ValueGeneratedNever()
                    .HasColumnName("MedicalRecordID");

                entity.Property(e => e.Condition).HasMaxLength(255);

                entity.Property(e => e.Diagnosis).HasMaxLength(255);

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Symptoms).HasMaxLength(255);

                entity.Property(e => e.TreatmentMethod).HasMaxLength(255);

                entity.Property(e => e.VisitDate).HasColumnType("date");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__MedicalRe__Patie__4BAC3F29");
            });

            modelBuilder.Entity<MedicalSupply>(entity =>
            {
                entity.Property(e => e.MedicalSupplyId)
                    .ValueGeneratedNever()
                    .HasColumnName("MedicalSupplyID");

                entity.Property(e => e.BatchNumber).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.SupplyType).HasMaxLength(255);

                entity.Property(e => e.UnitOfMeasure).HasMaxLength(255);

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.MedicalSupplies)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__MedicalSu__Suppl__4CA06362");
            });

            modelBuilder.Entity<MedicalUsage>(entity =>
            {
                entity.HasKey(e => e.UsageId)
                    .HasName("PK__MedicalU__29B197C0E3E8CD92");

                entity.ToTable("MedicalUsage");

                entity.Property(e => e.UsageId)
                    .ValueGeneratedNever()
                    .HasColumnName("UsageID");

                entity.Property(e => e.MedicineId).HasColumnName("MedicineID");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");

                entity.Property(e => e.ReturnDate).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.TransferedDate).HasColumnType("date");

                entity.HasOne(d => d.Medicine)
                    .WithMany(p => p.MedicalUsages)
                    .HasForeignKey(d => d.MedicineId)
                    .HasConstraintName("FK__MedicalUs__Medic__4E88ABD4");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.MedicalUsages)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__MedicalUs__Patie__4F7CD00D");

                entity.HasOne(d => d.Prescription)
                    .WithMany(p => p.MedicalUsages)
                    .HasForeignKey(d => d.PrescriptionId)
                    .HasConstraintName("FK__MedicalUs__Presc__5070F446");
            });

            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.ToTable("Medicine");

                entity.Property(e => e.MedicineId)
                    .ValueGeneratedNever()
                    .HasColumnName("MedicineID");

                entity.Property(e => e.ActiveIngredient).HasMaxLength(255);

                entity.Property(e => e.BatchNumber).HasMaxLength(255);

                entity.Property(e => e.BidNumber).HasMaxLength(255);

                entity.Property(e => e.Dosage).HasMaxLength(255);

                entity.Property(e => e.DosageForm).HasMaxLength(255);

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.TreatmentType).HasMaxLength(255);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(e => e.PatientId)
                    .ValueGeneratedNever()
                    .HasColumnName("PatientID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.EducationalLevel).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.EthnicGroup).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.HealthInsurance).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.Property(e => e.PrescriptionId)
                    .ValueGeneratedNever()
                    .HasColumnName("PrescriptionID");

                entity.Property(e => e.Diagnosis).HasMaxLength(255);

                entity.Property(e => e.IssueDate).HasColumnType("date");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.PaymentStatus).HasMaxLength(50);

                entity.Property(e => e.ReExamination).HasColumnType("date");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__Prescript__Patie__5165187F");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Prescript__UserI__52593CB8");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("RoleID");

                entity.Property(e => e.RoleName).HasMaxLength(255);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.SupplierId)
                    .ValueGeneratedNever()
                    .HasColumnName("SupplierID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.ContactInfo).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<SupplyConsumptionDetail>(entity =>
            {
                entity.Property(e => e.SupplyConsumptionDetailId)
                    .ValueGeneratedNever()
                    .HasColumnName("SupplyConsumptionDetailID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.SupplyName).HasMaxLength(50);

                entity.Property(e => e.SupplySettlementReportId).HasColumnName("SupplySettlementReportID");

                entity.Property(e => e.UnitOfMeasure).HasMaxLength(50);

                entity.HasOne(d => d.SupplySettlementReport)
                    .WithMany(p => p.SupplyConsumptionDetails)
                    .HasForeignKey(d => d.SupplySettlementReportId)
                    .HasConstraintName("FK__SupplyCon__Suppl__534D60F1");
            });

            modelBuilder.Entity<SupplyConsumptionReport>(entity =>
            {
                entity.ToTable("SupplyConsumptionReport");

                entity.Property(e => e.SupplyConsumptionReportId)
                    .ValueGeneratedNever()
                    .HasColumnName("SupplyConsumptionReportID");

                entity.Property(e => e.MedicalSupplyId).HasColumnName("MedicalSupplyID");

                entity.Property(e => e.ReportDate).HasColumnType("date");

                entity.Property(e => e.UnitOfMeasure).HasMaxLength(255);

                entity.HasOne(d => d.MedicalSupply)
                    .WithMany(p => p.SupplyConsumptionReports)
                    .HasForeignKey(d => d.MedicalSupplyId)
                    .HasConstraintName("FK__SupplyCon__Medic__5441852A");
            });

            modelBuilder.Entity<SupplyInventory>(entity =>
            {
                entity.ToTable("SupplyInventory");

                entity.Property(e => e.SupplyInventoryId).HasColumnName("SupplyInventoryID");

                entity.Property(e => e.CertificateNumber).HasMaxLength(255);

                entity.Property(e => e.ExpirationDate).HasColumnType("date");

                entity.Property(e => e.MedicalSupplyId).HasColumnName("MedicalSupplyID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.HasOne(d => d.MedicalSupply)
                    .WithMany(p => p.SupplyInventories)
                    .HasForeignKey(d => d.MedicalSupplyId)
                    .HasConstraintName("FK_SupplyInventory_MedicalSupplies");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.SupplyInventories)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK_SupplyInventory_Users");
            });

            modelBuilder.Entity<SupplySettlementReport>(entity =>
            {
                entity.ToTable("SupplySettlementReport");

                entity.Property(e => e.SupplySettlementReportId)
                    .ValueGeneratedNever()
                    .HasColumnName("SupplySettlementReportID");

                entity.Property(e => e.ServiceName).HasMaxLength(255);

                entity.Property(e => e.ServiceType).HasMaxLength(255);

                entity.Property(e => e.Unit).HasMaxLength(255);
            });

            modelBuilder.Entity<TransferedPatient>(entity =>
            {
                entity.Property(e => e.TransferedPatientId)
                    .ValueGeneratedNever()
                    .HasColumnName("TransferedPatientID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.TransferedDate).HasColumnType("date");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.TransferedPatients)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__Transfere__Patie__5629CD9C");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("UserID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.ResetToken).HasMaxLength(255);

                entity.Property(e => e.ResetTokenExpiry).HasColumnType("datetime");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK__Users__Departmen__571DF1D5");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleID__5812160E");
            });

            modelBuilder.Entity<VaccinationRecord>(entity =>
            {
                entity.ToTable("VaccinationRecord");

                entity.Property(e => e.VaccinationRecordId)
                    .ValueGeneratedNever()
                    .HasColumnName("VaccinationRecordID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.VaccinationDate).HasColumnType("date");

                entity.Property(e => e.VaccineId).HasColumnName("VaccineID");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.VaccinationRecords)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__Vaccinati__Patie__59063A47");

                entity.HasOne(d => d.Vaccine)
                    .WithMany(p => p.VaccinationRecords)
                    .HasForeignKey(d => d.VaccineId)
                    .HasConstraintName("FK__Vaccinati__Vacci__59FA5E80");
            });

            modelBuilder.Entity<Vaccine>(entity =>
            {
                entity.Property(e => e.VaccineId)
                    .ValueGeneratedNever()
                    .HasColumnName("VaccineID");

                entity.Property(e => e.BatchNumber).HasMaxLength(255);

                entity.Property(e => e.BidNumber).HasMaxLength(255);

                entity.Property(e => e.DosageForm).HasMaxLength(255);

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VaccineInventory>(entity =>
            {
                entity.ToTable("VaccineInventory");

                entity.Property(e => e.VaccineInventoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("VaccineInventoryID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.Property(e => e.VaccineId).HasColumnName("VaccineID");

                entity.HasOne(d => d.Vaccine)
                    .WithMany(p => p.VaccineInventories)
                    .HasForeignKey(d => d.VaccineId)
                    .HasConstraintName("FK__VaccineIn__Vacci__5AEE82B9");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
