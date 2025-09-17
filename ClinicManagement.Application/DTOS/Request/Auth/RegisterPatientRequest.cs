using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class RegisterPatientRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [MaxLength(254, ErrorMessage = "Email must be at most 254 characters.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [MaxLength(128, ErrorMessage = "Password must be at most 128 characters.")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name must be at most 100 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Phone format is invalid.")]
        [MaxLength(10, ErrorMessage = "Phone must be at most 10 characters.")]
        public string Phone { get; set; } = default!;
    }
}
