using ICMD.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.Dtos.User
{
    public class CreateOrEditUserDto
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string RoleName { get; set; }
    }
}
