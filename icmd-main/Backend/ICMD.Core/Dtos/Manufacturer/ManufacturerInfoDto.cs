using ICMD.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Manufacturer
{
    public class ManufacturerInfoDto: ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;

        public string Manufacturer { get; set; }
    }
}
