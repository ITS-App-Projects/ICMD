using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Tag
{
    public class CreateOrEditTagDto
    {
        public Guid Id { get; set; }
        public Guid? Field1Id { get; set; }
        public Guid? Field2Id { get; set; }
        public Guid? Field3Id { get; set; }
        public Guid? Field4Id { get; set; }
        public Guid? Field5Id { get; set; }
        public Guid? Field6Id { get; set; }

        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Field1String { get; set; }

        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Field2String { get; set; }

        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Field3String { get; set; }

        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Field4String { get; set; }

        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Field5String { get; set; }

        [StringLength(10, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Field6String { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string TagName { get; set; } = string.Empty;

        public Guid ProjectId { get; set; }
    }
}
