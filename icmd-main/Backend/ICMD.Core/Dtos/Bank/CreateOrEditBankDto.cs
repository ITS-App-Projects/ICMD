using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Bank
{
    public class CreateOrEditBankDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? ProjectId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string Bank { get; set; } = string.Empty;
    }
}
