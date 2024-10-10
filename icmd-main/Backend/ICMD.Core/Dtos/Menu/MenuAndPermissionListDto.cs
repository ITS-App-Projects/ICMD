using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Menu
{
    public class MenuAndPermissionListDto
    {
        public List<UserPermissionDto> Permissions { get; set; }
        public List<MenuItemListDto> MenuItems { get; set; }
    }

    public class UserPermissionDto
    {
        //public string MenuName { get; set; }
        public string ControllerName { get; set; }
        public string URL { get; set; }
        public List<string> PermissionName { get; set; }
    }

    public class MenuItemDto
    {
        //public Guid Id { get; set; }
        //public string MenuName { get; set; }
        public string MenuDescription { get; set; }
        public string ControllerName { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
        //public bool IsPermission { get; set; }
        //public List<string> Permissions { get; set; }
    }

    public class MenuItemListDto : MenuItemDto
    {
        public List<MenuItemDto> SubMenu { get; set; }
    }
}
