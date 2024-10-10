using ICMD.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace ICMD.Core.Dtos.SubSystem
{
    public class CreateOrEditSubSystemDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Number { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid? SystemId { get; set; }
    }
}
