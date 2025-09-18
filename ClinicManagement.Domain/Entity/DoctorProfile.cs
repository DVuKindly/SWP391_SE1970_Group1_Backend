using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class DoctorProfile : BaseEntity
    {
        [Key]
        public int DoctorProfileId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;


        [MaxLength(100)]
        public string? Degree { get; set; }         

        [MaxLength(200)]
        public string? Title { get; set; }          

        public int? ExperienceYears { get; set; }  

        [MaxLength(300)]
        public string? Education { get; set; }       

        [MaxLength(500)]
        public string? Certifications { get; set; }   

        [MaxLength(1000)]
        public string? Biography { get; set; }      

        [MaxLength(300)]
        public string? Workplace { get; set; }        

        [MaxLength(300)]
        public string? Image { get; set; }           
    }
}
