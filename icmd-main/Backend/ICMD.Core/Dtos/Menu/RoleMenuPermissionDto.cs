using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Menu
{
    public class RoleMenuPermissionDto
    {
        public Guid MenuId { get; set; }
        public Guid RoleId { get; set; }
        public bool IsGranted { get; set; }
    }
}
