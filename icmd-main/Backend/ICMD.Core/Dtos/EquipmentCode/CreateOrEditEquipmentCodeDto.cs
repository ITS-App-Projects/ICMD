using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.EquipmentCode
{
    public class CreateOrEditEquipmentCodeDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Code { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Descriptor { get; set; } = string.Empty;
    }
}
