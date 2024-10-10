using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.TagType
{
    public class CreateOrEditTagTypeDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Name { get; set; } = string.Empty;

        [StringLength(80, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;
    }
}
