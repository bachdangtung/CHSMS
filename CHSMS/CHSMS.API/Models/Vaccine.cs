using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Vaccine
    {
        public Vaccine()
        {
            VaccinationRecords = new HashSet<VaccinationRecord>();
            VaccineInventories = new HashSet<VaccineInventory>();
        }

        public int VaccineId { get; set; }
        public string? VaccineName { get; set; }
        public string? DosageForm { get; set; }
        public int? Quantity { get; set; }
        public string? BatchNumber { get; set; }
        public string? BidNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? VaccinationRate { get; set; }

        public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; }
        public virtual ICollection<VaccineInventory> VaccineInventories { get; set; }
    }
}
