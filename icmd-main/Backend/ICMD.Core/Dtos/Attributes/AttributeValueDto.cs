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
    public class AttributeValueDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? ValueType { get; set; }
        public bool Required { get; set; }
        [AttributeValueTypeValidationAttribute]
        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Value { get; set; }
    }
}
