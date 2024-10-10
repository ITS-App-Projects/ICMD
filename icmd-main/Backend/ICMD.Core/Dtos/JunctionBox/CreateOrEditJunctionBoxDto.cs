using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.JunctionBox
{
    public class CreateOrEditJunctionBoxDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? TagId { get; set; }

        [StringLength(100, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Type { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;

        public Guid? ReferenceDocumentTypeId { get; set; }

        public Guid? ReferenceDocumentId { get; set; }

    }
}
