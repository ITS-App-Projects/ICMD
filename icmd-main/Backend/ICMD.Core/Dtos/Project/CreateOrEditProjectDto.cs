using ICMD.Core.Constants;
using ICMD.Core.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Project
{
    public class CreateOrEditProjectDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? Description { get; set; }

        
        public List<UserAuthorizationDto>? UserAuthorizations { get; set; }
    }
}
