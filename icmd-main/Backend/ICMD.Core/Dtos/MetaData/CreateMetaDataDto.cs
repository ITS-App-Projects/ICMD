using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.MetaData
{
    public class CreateMetaDataDto
    {
        [Required]
        public string TemplateName { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
