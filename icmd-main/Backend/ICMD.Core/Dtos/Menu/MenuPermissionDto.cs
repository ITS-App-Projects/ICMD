using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Menu
{
    public class MenuPermissionDto
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        public string? MenuName { get; set; }
        public string? ParentMenuName { get; set; }
        public List<OperationListDto>? OperationList { get; set; }
    }

    public class OperationListDto
    {
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public bool IsGranted { get; set; }
        public Guid? MenuPermissionId { get; set; }

    }

    public class OperationsDto
    {
        public int Id { get; set; }
        public string OperationName { get; set; }
    }
}
