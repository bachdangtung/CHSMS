using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Appointment
    {
        public int AppointmentId { get; set; }
        public int? PatientId { get; set; }
        public int? UserId { get; set; }
        public DateTime? Date { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }

        public virtual Patient? Patient { get; set; }
        public virtual User? User { get; set; }
    }
}
