using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Menu
{
    public class PermissionByMenuRoleDto
    {
        public Operations Operation { get; set; }
        public Guid MenuPermissionId { get; set; }
        public bool IsGranted { get; set; }
    }
}
