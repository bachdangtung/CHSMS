using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class User
    {
        public User()
        {
            ExternalPrescriptions = new HashSet<ExternalPrescription>();
            MedicalRecordHistories = new HashSet<MedicalRecordHistory>();
            MedicalSupplyInventories = new HashSet<MedicalSupplyInventory>();
            MedicineInventories = new HashSet<MedicineInventory>();
            Prescriptions = new HashSet<Prescription>();
            UseMedicalSupplies = new HashSet<UseMedicalSupply>();
        }

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public string? Specialization { get; set; }
        public bool? Status { get; set; }
        public string? Fullname { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<ExternalPrescription> ExternalPrescriptions { get; set; }
        public virtual ICollection<MedicalRecordHistory> MedicalRecordHistories { get; set; }
        public virtual ICollection<MedicalSupplyInventory> MedicalSupplyInventories { get; set; }
        public virtual ICollection<MedicineInventory> MedicineInventories { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<UseMedicalSupply> UseMedicalSupplies { get; set; }
    }
}
