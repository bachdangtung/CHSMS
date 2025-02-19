using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class User
    {
        public User()
        {
            Appointments = new HashSet<Appointment>();
            Prescriptions = new HashSet<Prescription>();
            SupplyInventories = new HashSet<SupplyInventory>();
        }

        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public int? DepartmentId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<SupplyInventory> SupplyInventories { get; set; }
    }
}
