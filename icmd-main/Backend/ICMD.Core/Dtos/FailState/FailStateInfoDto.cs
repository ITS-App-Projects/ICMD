using ICMD.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.FailState
{
    public class FailStateInfoDto: ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? FailStateName { get; set; }
    }
}
