using ICMD.Core.Constants;
using ICMD.Core.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Attributes
{
    public class AttributesDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;

        public string? ValueType { get; set; }

        public bool Private { get; set; }

        public bool Inherit { get; set; }

        public bool Required { get; set; }

        [AttributeValueTypeValidationAttribute]
        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Value { get; set; }
    }
}
