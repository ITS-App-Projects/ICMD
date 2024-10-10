using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.MemoryCache
{
    public class AddMemoryCacheDto
    {
        public string Key { get; set; }
        public string[] Value { get; set; }
    }
}
