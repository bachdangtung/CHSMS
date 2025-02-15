using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class TransferedPatient
    {
        public int TransferedPatientId { get; set; }
        public int? PatientId { get; set; }
        public DateTime? TransferedDate { get; set; }
        public string? Note { get; set; }

        public virtual Patient? Patient { get; set; }
    }
}
