using ICMD.Core.Constants;
using ICMD.Core.Dtos.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.DeviceType
{
    public class CreateOrEditDeviceTypeDto
    {

        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Type { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;

        public List<AttributesDto>? Attributes { get; set; }
    }
}
