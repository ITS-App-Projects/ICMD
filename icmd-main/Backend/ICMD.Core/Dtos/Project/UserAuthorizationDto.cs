using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Project
{
    public class UserAuthorizationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Authorization { get; set; }
    }
}
