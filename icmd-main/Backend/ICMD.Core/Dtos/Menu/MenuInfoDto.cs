using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Menu
{
    public class MenuInfoDto
    {
        public Guid Id { get; set; }
        public string MenuName { get; set; }
        public string MenuDescription { get; set; }
        public string ControllerName { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
        public Guid? ParentMenuId { get; set; }
        public bool IsActive { get; set; }
        public List<MenuInfoDto> SubMenus { get; set; }
    }
}
