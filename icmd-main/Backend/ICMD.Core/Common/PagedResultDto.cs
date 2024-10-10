using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Common
{
    public class PagedResultDto<T>
    {
        public int TotalCount { get; set; }
        public IReadOnlyList<T> Items { get; set; }
        public PagedResultDto() { }
        public PagedResultDto(int totalCount, IReadOnlyList<T> items)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }
}
