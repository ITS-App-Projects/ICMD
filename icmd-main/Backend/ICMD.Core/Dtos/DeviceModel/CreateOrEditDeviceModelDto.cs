using ICMD.Core.Constants;
using ICMD.Core.Dtos.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.DeviceModel
{
    public class CreateOrEditDeviceModelDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Model { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid? ManufacturerId { get; set; }

        public List<AttributesDto>? Attributes { get; set; }
    }
}
