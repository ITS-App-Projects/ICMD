using AutoMapper;
using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Authorization;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Menu;
using ICMD.Core.Dtos.Role;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly IMenuPermissionService _menuPermissionService;
        private readonly IMapper _mapper;
        private readonly RoleManager<ICMDRole> _roleManager;
        private static string ModuleName = "Menu";
        private readonly IPermissionManagementService _permissionManagementService;

        public MenuController(IMenuService menuService, IMenuPermissionService menuPermissionService, IMapper mapper, RoleManager<ICMDRole> roleManager, IPermissionManagementService permissionManagementService)
        {
            _menuService = menuService;
            _menuPermissionService = menuPermissionService;
            _mapper = mapper;
            _roleManager = roleManager;
            _permissionManagementService = permissionManagementService;
        }
        #region Menu
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<MenuInfoDto>> GetAllMenu()
        {
            IQueryable<MenuItems> allmenuInfo = _menuService.GetAll(x => !x.IsDeleted).OrderBy(s => s.SortOrder);
            List<MenuInfoDto> mainMenuInfo = _mapper.Map<List<MenuInfoDto>>(await allmenuInfo.Where(s => s.ParentMenuId == null).ToListAsync());
            mainMenuInfo.ForEach(s =>
            {
                List<MenuInfoDto> allSubMenus = _mapper.Map<List<MenuInfoDto>>(allmenuInfo.Where(a => a.ParentMenuId == s.Id));
                s.SubMenus = allSubMenus.OrderBy(x => x.SortOrder).ToList();
            });
            return mainMenuInfo.ToList();
        }

        [HttpGet]
        public async Task<MenuForViewDto> GetMenuById(Guid menuId)
        {
            MenuItems menuItems = await _menuService.GetSingleAsync(x => x.Id == menuId && !x.IsDeleted && x.IsActive);
            return _mapper.Map<MenuForViewDto>(menuItems);
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditMenu(CreateOrEditMenuDto createOrEditMenuDto)
        {
            if (createOrEditMenuDto.Id == Guid.Empty)
            {
                return await CreateMenu(createOrEditMenuDto);
            }
            else
            {
                return await UpdateMenu(createOrEditMenuDto);
            }
        }

        private async Task<BaseResponse> CreateMenu(CreateOrEditMenuDto menuDto)
        {
            Guid userId = User.GetUserId();
            MenuItems menu = _mapper.Map<MenuItems>(menuDto);
            MenuItems isExist = await _menuService.GetSingleAsync(x => x.MenuName == menu.MenuName
            && x.ParentMenuId == (menu.ParentMenuId != null && menu.ParentMenuId != Guid.Empty ? menu.ParentMenuId : null)
            && !x.IsDeleted && x.IsActive);
            if (isExist != null)
                return new BaseResponse(false, ResponseMessages.MenuAlreadyExist, HttpStatusCode.Conflict);

            //create menu
            var menuResponse = await _menuService.AddAsync(menu, User.GetUserId());
            if (menuResponse == null)
                return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.Conflict);

            //Assign System-Admin Role
            var systemAdminRole = await _roleManager.Roles.FirstOrDefaultAsync(s => s.Name == RoleConstants.Administrator);
            Guid menuPermissionId = Guid.Empty;
            if (systemAdminRole != null)
            {
                List<RoleMenuPermissionDto> rolemenuInfo = new()
                {
                    new RoleMenuPermissionDto()
                    {
                        IsGranted = true,
                        MenuId = menuResponse.Id,
                        RoleId = systemAdminRole.Id
                    }
                };
                if (rolemenuInfo != null)
                {
                    BaseResponse baseResponse = await SetRoleWiseMenuPermission(rolemenuInfo);
                    menuPermissionId = (baseResponse.Data as MenuPermission)?.Id ?? Guid.Empty;
                }
            }

            //Assign Permission to System-Admin
            if (menuPermissionId != Guid.Empty && menuDto.IsPermission)
            {
                await SetPermissionOfMenu(menuPermissionId);
            }

            return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, menuResponse);
        }

        private async Task<BaseResponse> UpdateMenu(CreateOrEditMenuDto menuDto)
        {
            MenuItems menuItems = await _menuService.GetSingleAsync(s => s.Id == menuDto.Id && !s.IsDeleted && s.IsActive);
            if (menuItems == null)
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

            MenuItems menu = _mapper.Map<MenuItems>(menuDto);
            //update menu
            menu.CreatedBy = menuItems.CreatedBy;
            menu.CreatedDate = menuItems.CreatedDate;
            var menuResponse = _menuService.Update(menu, menuItems, User.GetUserId());
            if (menuResponse == null)
                return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

            return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, menuResponse);
        }

        private async Task SetPermissionOfMenu(Guid menuPermissionId)
        {
            List<PermissionManagement> permissionManagement = await _permissionManagementService.GetAll(s => s.IsActive && !s.IsDeleted && s.MenuPermissionId == menuPermissionId).ToListAsync();
            List<Operations> OperationEnums = Enum.GetValues(typeof(Operations)).Cast<Operations>().ToList();
            foreach (var operations in OperationEnums)
            {
                PermissionManagement? permissionInfo = permissionManagement?.FirstOrDefault(s => s.Operation == operations) ?? null;
                if (permissionInfo != null)
                {
                    permissionInfo.IsGranted = true;
                    _permissionManagementService.Update(permissionInfo, permissionInfo, User.GetUserId());
                }
                else
                {
                    PermissionByMenuRoleDto permissionByMenuRoleDto = new()
                    {
                        Operation = operations,
                        IsGranted = true,
                        MenuPermissionId = menuPermissionId
                    };
                    PermissionManagement model = _mapper.Map<PermissionManagement>(permissionByMenuRoleDto);
                    await _permissionManagementService.AddAsync(model, User.GetUserId());
                }
            }
        }
        #endregion


        #region Menu With Permission
        [HttpGet]
        [AuthorizePermission()]
        public async Task<MenuListWithRoleDto> GetAllMenuWithPermission()
        {
            IQueryable<MenuItems> allmenuInfo = _menuService.GetAll(x => !x.IsDeleted && x.IsActive).OrderBy(s => s.SortOrder);
            IQueryable<MenuPermission> getAllMenuPermission = _menuPermissionService.GetAll(s => s.IsActive && !s.IsDeleted);

            List<RoleDropDownDto> rolesList = _mapper.Map<List<RoleDropDownDto>>((await _roleManager.Roles.ToListAsync()).OrderBy(x => RoleOrder.Roles[x.Name]));

            List<MenuPermission> allMenuPermission = await getAllMenuPermission.ToListAsync();
            List<MenuItems> mainMenus = await allmenuInfo.ToListAsync();

            MenuListWithRoleDto menuList = new()
            {
                RoleList = rolesList,
                MenuList = (_mapper.Map<List<MainMenuDto>>(mainMenus.Where(s => s.ParentMenuId == null))).OrderBy(x => x.SortOrder).ToList(),
            };

            menuList.MenuList.ForEach(x =>
            {
                x.RoleIds = allMenuPermission.Where(s => s.IsGranted && s.IsActive && !s.IsDeleted && s.MenuId == x.Id).Select(x => x.RoleId).ToList();
                var subMenus = _mapper.Map<List<SubMenuDto>>(mainMenus.Where(s => s.ParentMenuId == x.Id));
                subMenus.ForEach(y =>
                {
                    y.RoleIds = allMenuPermission.Where(s => s.IsGranted && s.IsActive && !s.IsDeleted && s.MenuId == y.Id).Select(x => x.RoleId).ToList();
                });

                x.SubMenuList = subMenus.OrderBy(y => y.SortOrder).ToList();
            });
            return menuList;
        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<BaseResponse> SetRoleWiseMenuPermission(List<RoleMenuPermissionDto> rolemenuInfo)
        {
            if (rolemenuInfo == null || rolemenuInfo.Count == 0)
                return new BaseResponse(false, ResponseMessages.NoMenuPermission, HttpStatusCode.NoContent);

            IQueryable<MenuPermission> getAllMenuPermission = _menuPermissionService.GetAll(s => s.IsActive && !s.IsDeleted);

            MenuPermission menuPermissionResponse = new();

            List<Guid> roleIds = new();
            foreach (var item in rolemenuInfo)
            {
                MenuPermission? getPermissionInfo = getAllMenuPermission.FirstOrDefault(s => s.RoleId == item.RoleId && s.MenuId == item.MenuId);
                if (getPermissionInfo == null)
                {
                    MenuPermission permissionInfo = _mapper.Map<MenuPermission>(item);
                    menuPermissionResponse = await _menuPermissionService.AddAsync(permissionInfo, User.GetUserId());
                }
                else
                {
                    MenuPermission permissionInfo = _mapper.Map<MenuPermission>(getPermissionInfo);
                    MenuPermission oldPermissionInfo = permissionInfo;
                    permissionInfo = _mapper.Map<RoleMenuPermissionDto, MenuPermission>(item, permissionInfo);
                    menuPermissionResponse = _menuPermissionService.Update(permissionInfo, oldPermissionInfo, User.GetUserId());
                }

                if (!roleIds.Any(x => x == item.RoleId))
                    roleIds.Add(item.RoleId);
            }



            return new BaseResponse(true, ResponseMessages.PermissionUpdate, HttpStatusCode.OK, menuPermissionResponse);
        }
        #endregion
    }
}
