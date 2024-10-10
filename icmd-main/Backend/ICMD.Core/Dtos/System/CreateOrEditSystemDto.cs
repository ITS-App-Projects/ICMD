using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.System
{
    public class CreateOrEditSystemDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Number { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid? WorkAreaPackId { get; set; }
    }
}
