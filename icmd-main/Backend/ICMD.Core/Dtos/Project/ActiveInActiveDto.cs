using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Project
{
    public class ActiveInActiveDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
