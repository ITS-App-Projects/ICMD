using ICMD.Core.ViewDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.PnIdTags
{
    public class PnIDTagExceptionInfoDto
    {
        public string? Key { get; set; }
        public List<ViewPnIDTagExceptionDto> Items { get; set; } = new List<ViewPnIDTagExceptionDto>();
    }
}
