using ICMD.Core.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Menu
{
    public class MenuListWithRoleDto
    {
        public List<RoleDropDownDto> RoleList { get; set; }
        public List<MainMenuDto> MenuList { get; set; }
    }

    public abstract class MenuDto
    {
        public Guid Id { get; set; }
        public string MenuDescription { get; set; }
        public int SortOrder { get; set; }
        public List<Guid> RoleIds { get; set; }
    }

    public class MainMenuDto : MenuDto
    {
        public List<SubMenuDto> SubMenuList { get; set; }
    }

    public class SubMenuDto : MenuDto
    {
        public Guid? ParentMenuId { get; set; }
    }
}
