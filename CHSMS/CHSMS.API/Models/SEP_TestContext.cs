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

        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public virtual DbSet<MedicalRecordHistory> MedicalRecordHistories { get; set; } = null!;
        public virtual DbSet<MedicalSupply> MedicalSupplies { get; set; } = null!;
        public virtual DbSet<MedicalSupplyConsumption> MedicalSupplyConsumptions { get; set; } = null!;
        public virtual DbSet<MedicalSupplyInventory> MedicalSupplyInventories { get; set; } = null!;
        public virtual DbSet<Medicine> Medicines { get; set; } = null!;
        public virtual DbSet<MedicineConsumption> MedicineConsumptions { get; set; } = null!;
        public virtual DbSet<MedicineInventory> MedicineInventories { get; set; } = null!;
        public virtual DbSet<MedicinePrescription> MedicinePrescriptions { get; set; } = null!;
        public virtual DbSet<Prescription> Prescriptions { get; set; } = null!;
        public virtual DbSet<PrescriptionMedicineConsumption> PrescriptionMedicineConsumptions { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=localhost; database=SEP_Test7;user=sa;password=123;Integrated Security=true;TrustServerCertificate=Yes");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.ToTable("MedicalRecord");

                entity.Property(e => e.MedicalRecordId).HasColumnName("MedicalRecordID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.EducationLevel).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.EthnicGroup).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.HealthInsurance).HasMaxLength(255);

                entity.Property(e => e.Job).HasMaxLength(50);

                entity.Property(e => e.Note).HasMaxLength(50);

                entity.Property(e => e.PatientName).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<MedicalRecordHistory>(entity =>
            {
                entity.ToTable("MedicalRecordHistory");

                entity.Property(e => e.MedicalRecordHistoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("MedicalRecordHistoryID");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.BloodPressure)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Icd).HasColumnName("ICD");

                entity.Property(e => e.MedicalRecordHistoryCode).HasMaxLength(50);

                entity.Property(e => e.MedicalRecordId).HasColumnName("MedicalRecordID");

                entity.Property(e => e.PatientCategory).HasMaxLength(50);

                entity.Property(e => e.TreatmentBed).HasMaxLength(25);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.MedicalRecord)
                    .WithMany(p => p.MedicalRecordHistories)
                    .HasForeignKey(d => d.MedicalRecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MedicalRecordHistory_MedicalRecord");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MedicalRecordHistories)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_MedicalRecordHistory_Users");
            });

            modelBuilder.Entity<MedicalSupply>(entity =>
            {
                entity.Property(e => e.MedicalSupplyId).HasColumnName("MedicalSupplyID");

                entity.Property(e => e.MedicalSupplyCode).HasMaxLength(50);

                entity.Property(e => e.MedicalSupplyName).HasMaxLength(255);

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.SupplyType).HasMaxLength(255);

                entity.Property(e => e.UnitOfMeasure).HasMaxLength(255);

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.MedicalSupplies)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__MedicalSu__Suppl__49C3F6B7");
            });

            modelBuilder.Entity<MedicalSupplyConsumption>(entity =>
            {
                entity.HasKey(e => e.MsconsumptionId);

                entity.ToTable("MedicalSupplyConsumption");

                entity.Property(e => e.MsconsumptionId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MSConsumptionId");

                entity.Property(e => e.ConsumptionDate).HasColumnType("date");

                entity.Property(e => e.MedicalSupplyInventoryId).HasColumnName("MedicalSupplyInventoryID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.HasOne(d => d.Msconsumption)
                    .WithOne(p => p.MedicalSupplyConsumption)
                    .HasForeignKey<MedicalSupplyConsumption>(d => d.MsconsumptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MedicalSupplyConsumption_MedicalSupplyInventory");
            });

            modelBuilder.Entity<MedicalSupplyInventory>(entity =>
            {
                entity.HasKey(e => e.SupplyInventoryId)
                    .HasName("PK__MedicalS__8C3DCF4A17054FB0");

                entity.ToTable("MedicalSupplyInventory");

                entity.Property(e => e.SupplyInventoryId).HasColumnName("SupplyInventoryID");

                entity.Property(e => e.BatchNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CertificateNumber).HasMaxLength(255);

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.Property(e => e.ManufactureDate).HasColumnType("date");

                entity.Property(e => e.MedicalSupplyId).HasColumnName("MedicalSupplyID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.HasOne(d => d.MedicalSupply)
                    .WithMany(p => p.MedicalSupplyInventories)
                    .HasForeignKey(d => d.MedicalSupplyId)
                    .HasConstraintName("FK__MedicalSu__Medic__4CA06362");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.MedicalSupplyInventories)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK_MedicalSupplyInventory_Users");
            });

            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.ToTable("Medicine");

                entity.Property(e => e.MedicineId).HasColumnName("MedicineID");

                entity.Property(e => e.ActiveIngredient).HasMaxLength(255);

                entity.Property(e => e.BidNumber).HasMaxLength(255);

                entity.Property(e => e.Dosage).HasMaxLength(255);

                entity.Property(e => e.DosageForm).HasMaxLength(255);

                entity.Property(e => e.IsBhyt).HasColumnName("IsBHYT");

                entity.Property(e => e.MedicineCode).HasMaxLength(255);

                entity.Property(e => e.MedicineName).HasMaxLength(255);
            });

            modelBuilder.Entity<MedicineConsumption>(entity =>
            {
                entity.ToTable("MedicineConsumption");

                entity.Property(e => e.MedicineConsumptionId).HasColumnName("MedicineConsumptionID");

                entity.Property(e => e.ConsumptionDate).HasColumnType("date");

                entity.Property(e => e.MedicineInventoryId).HasColumnName("MedicineInventoryID");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .IsFixedLength();

                entity.HasOne(d => d.MedicineInventory)
                    .WithMany(p => p.MedicineConsumptions)
                    .HasForeignKey(d => d.MedicineInventoryId)
                    .HasConstraintName("FK_MedicineConsumption_MedicineInventory");
            });

            modelBuilder.Entity<MedicineInventory>(entity =>
            {
                entity.ToTable("MedicineInventory");

                entity.Property(e => e.MedicineInventoryId).HasColumnName("MedicineInventoryID");

                entity.Property(e => e.BatchNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CertificateNumber).HasMaxLength(255);

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.ManufacturingDate).HasColumnType("datetime");

                entity.Property(e => e.MedicineId).HasColumnName("MedicineID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.HasOne(d => d.Medicine)
                    .WithMany(p => p.MedicineInventories)
                    .HasForeignKey(d => d.MedicineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MedicalIn__Medic__5DCAEF64");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.MedicineInventories)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK_MedicineInventory_Users");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.MedicineInventories)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_MedicineInventory_Suppliers");
            });

            modelBuilder.Entity<MedicinePrescription>(entity =>
            {
                entity.HasKey(e => new { e.PrescriptionId, e.MedicineId });

                entity.ToTable("Medicine_Prescription");

                entity.Property(e => e.MedicineId).HasColumnName("MedicineID");

                entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");

                entity.HasOne(d => d.Medicine)
                    .WithMany()
                    .HasForeignKey(d => d.MedicineId)
                    .HasConstraintName("FK_Medicine_Prescription_Medicine");

                entity.HasOne(d => d.Prescription)
                    .WithMany()
                    .HasForeignKey(d => d.PrescriptionId)
                    .HasConstraintName("FK_Medicine_Prescription_Prescriptions");
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");

                entity.Property(e => e.IsBhyt).HasColumnName("IsBHYT");

                entity.Property(e => e.IssueDate).HasColumnType("date");

                entity.Property(e => e.MedicalRecordHistoryId).HasColumnName("MedicalRecordHistoryID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.MedicalRecordHistory)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.MedicalRecordHistoryId)
                    .HasConstraintName("FK_Prescriptions_MedicalRecordHistory");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Prescript__UserI__5535A963");
            });

            modelBuilder.Entity<PrescriptionMedicineConsumption>(entity =>
            {
                entity.HasKey(e => new { e.PrescriptionId, e.MedicineConsumtionId });

                entity.ToTable("Prescription_MedicineConsumption");

                entity.HasIndex(e => e.MedicineConsumtionId, "IX_Prescription_MedicineConsumption")
                    .IsUnique();

                entity.Property(e => e.MedicineConsumtionId).HasColumnName("MedicineConsumtionID");

                entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");

                entity.HasOne(d => d.MedicineConsumtion)
                    .WithOne()
                    .HasForeignKey<PrescriptionMedicineConsumption>(d => d.MedicineConsumtionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prescription_MedicineConsumption_MedicineConsumption");

                entity.HasOne(d => d.Prescription)
                    .WithMany()
                    .HasForeignKey(d => d.PrescriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prescription_MedicineConsumption_Prescriptions");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName).HasMaxLength(255);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.ContactInfo).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Fullname).HasMaxLength(50);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.ResetToken).HasMaxLength(255);

                entity.Property(e => e.ResetTokenExpiry).HasColumnType("datetime");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Specialization).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(255);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleID__5812160E");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
