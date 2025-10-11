using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class RegistrationRequestDto : IValidatableObject
    {

        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [MaxLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [MaxLength(200)]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [MaxLength(20)]
        public string Phone { get; set; } = default!;

        [Required(ErrorMessage = "Nội dung khám là bắt buộc.")]
        [MaxLength(1000, ErrorMessage = "Nội dung không vượt quá 1000 ký tự.")]
        public string Content { get; set; } = default!;

        [Required(ErrorMessage = "Ngày khám là bắt buộc.")]
        public DateTime StartDate { get; set; }

     
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var minDate = DateTime.UtcNow.Date.AddDays(2);
            if (StartDate.Date < minDate)
            {
                yield return new ValidationResult(
                    $"Ngày khám phải sau ít nhất 2 ngày kể từ hôm nay ({minDate:dd/MM/yyyy}).",
                    new[] { nameof(StartDate) }
                );
            }
        }
    }
}
