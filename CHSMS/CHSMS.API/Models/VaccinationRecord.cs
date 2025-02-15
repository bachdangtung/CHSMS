using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class VaccinationRecord
    {
        public int VaccinationRecordId { get; set; }
        public int? VaccineId { get; set; }
        public int? PatientId { get; set; }
        public int? Dose { get; set; }
        public DateTime? VaccinationDate { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; }

        public virtual Patient? Patient { get; set; }
        public virtual Vaccine? Vaccine { get; set; }
    }
}
