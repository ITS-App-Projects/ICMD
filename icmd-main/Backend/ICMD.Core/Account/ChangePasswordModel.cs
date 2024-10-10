using System.ComponentModel.DataAnnotations;

namespace ICMD.Core.Account
{
    public class ChangePasswordModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class ChangeUserPasswordModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
