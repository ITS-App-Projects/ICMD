using ICMD.Core.Constants;
using ICMD.Core.Dtos.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.NatureOfSignal
{
    public class CreateOrEditNatureOfSignalDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string NatureOfSignalName { get; set; } = string.Empty;

        public List<AttributesDto>? Attributes { get; set; }
    }
}
