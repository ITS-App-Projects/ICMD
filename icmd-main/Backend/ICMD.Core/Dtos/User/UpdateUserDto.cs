using ICMD.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace ICMD.Core.Dtos.User
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 2)]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string RoleName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
