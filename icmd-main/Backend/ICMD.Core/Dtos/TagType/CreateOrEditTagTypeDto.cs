using System.ComponentModel.DataAnnotations;

using ICMD.Core.Constants;

namespace ICMD.Core.Dtos.TagType
{
    public class CreateOrEditTagTypeDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(30, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Name { get; set; } = string.Empty;

        [StringLength(80, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;
    }
}
