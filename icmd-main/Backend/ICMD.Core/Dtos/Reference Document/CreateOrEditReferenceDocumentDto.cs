using ICMD.Core.Constants;
using ICMD.Core.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Reference_Document
{
    public class CreateOrEditReferenceDocumentDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? ProjectId { get; set; }

        [Required]
        public Guid? ReferenceDocumentTypeId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string DocumentNumber { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        [UrlValidation(ErrorMessage = ResponseMessages.URLIsInvalid)]
        public string? URL { get; set; }

        [StringLength(500, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Version { get; set; }

        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Revision { get; set; }

        public string? Date { get; set; }

        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Sheet { get; set; }
    }
}
