using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.User
{
    public class UserDropdownDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
    }
}
