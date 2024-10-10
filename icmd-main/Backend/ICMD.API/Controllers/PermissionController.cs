using AutoMapper;
using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Menu;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class PermissionController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IPermissionManagementService _permissionManagementService;
        private readonly IMenuPermissionService _menuPermissionService;
        private readonly IMenuService _menuService;


        public PermissionController(IMapper mapper, IPermissionManagementService permissionManagementService, IMenuPermissionService menuPermissionService,
            IMenuService menuService)
        {
            _mapper = mapper;
            _permissionManagementService = permissionManagementService;
            _menuPermissionService = menuPermissionService;
            _menuService = menuService;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<MenuPermissionDto>> GetAllMenuWithPermission(Guid roleId)
        {
            List<MenuPermissionDto> allMenuPermission = new();
            List<Operations> operations = Enum.GetValues(typeof(Operations)).Cast<Operations>().ToList();
            List<OperationsDto> operationsDto = operations.Select(x => new OperationsDto
            {
                Id = (int)x,
                OperationName = x.ToString()//Enum.GetName(typeof(Operations), (int)x.Operation)
            }).ToList();
            if (operationsDto != null && operationsDto.Count > 0)
            {
                //Menu Permission
                List<MenuPermission> getAllMenuPermission = await _menuPermissionService.GetAll(s => s.IsActive && !s.IsDeleted && s.RoleId == roleId && s.IsGranted).ToListAsync();
                allMenuPermission = _mapper.Map<List<MenuPermissionDto>>(getAllMenuPermission.Where(s => s.MenuItems.IsPermission));

                var permissionManagements = await _permissionManagementService.GetAll(s => s.IsActive && !s.IsDeleted).ToListAsync();

                allMenuPermission.ForEach(x =>
                {
                    var permissionManagementInfo = _mapper.Map<List<PermissionByMenuRoleDto>>(permissionManagements.Where(s => s.MenuPermission.RoleId == roleId && !s.MenuPermission.IsDeleted && s.MenuPermission.IsGranted && s.MenuPermission.IsActive
                    && s.MenuPermissionId == x.Id).ToList());

                    var IsMainMenu = getAllMenuPermission.FirstOrDefault(s => s.Id == x.Id)?.MenuItems?.ParentMenuId == null;
                    x.MenuName = getAllMenuPermission.FirstOrDefault(s => s.Id == x.Id)?.MenuItems?.MenuDescription;
                    bool hasSubMenu = getAllMenuPermission.Count(s => s.MenuItems.ParentMenuId == x.MenuId) > 0;

                    if (!(IsMainMenu && hasSubMenu))
                    {
                        string? parentMenuName = _menuService.GetSingle(s => s.Id == x.MenuId)?.ParentMenu?.MenuName ?? null;
                        x.ParentMenuName = !IsMainMenu ? parentMenuName : null;

                        if (!permissionManagementInfo.Any(a => a.MenuPermissionId == x.Id))
                        {
                            x.OperationList = operationsDto.Select(s => new OperationListDto
                            {
                                OperationId = s.Id,
                                OperationName = s.OperationName,
                                IsGranted = false,
                                MenuPermissionId = x.Id
                            }).ToList();
                        }
                        else
                        {
                            var info = (from o in operationsDto
                                        join o1 in permissionManagementInfo on o.Id equals ((int)o1.Operation) into Permission
                                        from o2 in Permission.DefaultIfEmpty()
                                            //where !o2.IsDeleted && o2.IsActive
                                        select new OperationListDto
                                        {
                                            OperationId = o.Id,
                                            OperationName = o.OperationName,
                                            IsGranted = o2 != null ? o2.IsGranted : false,
                                            MenuPermissionId = o2 != null ? o2.MenuPermissionId : x.Id,
                                        }).ToList();

                            x.OperationList = info.Where(s => s.MenuPermissionId == x.Id || s.MenuPermissionId == null).ToList();
                        }
                    }
                });

                allMenuPermission = allMenuPermission.Where(x => x.OperationList != null).ToList();
            }

            return allMenuPermission;
        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<BaseResponse> SetPermissionByRole(List<PermissionByMenuRoleDto> menuPermissionDto)
        {
            if (menuPermissionDto == null || menuPermissionDto.Count == 0)
                return new BaseResponse(false, ResponseMessages.NoMenuPermission, HttpStatusCode.NoContent);

            IQueryable<PermissionManagement> permissionManagements = _permissionManagementService.GetAll(s => s.IsActive && !s.IsDeleted);

            foreach (var item in menuPermissionDto)
            {
                PermissionManagement? getPermissionInfo =
                    permissionManagements.FirstOrDefault(s => s.MenuPermissionId == item.MenuPermissionId && s.Operation == item.Operation);
                //permissionManagements.FirstOrDefault(s => s.MenuPermissionId == item.MenuPermissionId && s.PermissionId == item.PermissionId);
                if (getPermissionInfo == null)
                {
                    PermissionManagement permissionInfo = _mapper.Map<PermissionManagement>(item);
                    await _permissionManagementService.AddAsync(permissionInfo, User.GetUserId());
                }
                else
                {
                    PermissionManagement permissionInfo = _mapper.Map<PermissionManagement>(getPermissionInfo);
                    PermissionManagement oldPermissionInfo = permissionInfo;
                    permissionInfo = _mapper.Map<PermissionByMenuRoleDto, PermissionManagement>(item, permissionInfo);
                    _permissionManagementService.Update(permissionInfo, oldPermissionInfo, User.GetUserId());
                }
            }

            MenuPermission menuPermission = await _menuPermissionService.GetSingleAsync(x => x.Id == menuPermissionDto.First().MenuPermissionId);
            return new BaseResponse(true, ResponseMessages.PermissionUpdate, HttpStatusCode.OK);
        }
    }
}
