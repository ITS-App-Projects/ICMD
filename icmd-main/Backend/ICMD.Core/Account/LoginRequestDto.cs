using System.ComponentModel.DataAnnotations;

namespace ICMD.Core.Account
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ResponseModel: BaseResponse
    {
        public string? Token { get; set; }
        public int ErrorType { get; set; }
    }

}
