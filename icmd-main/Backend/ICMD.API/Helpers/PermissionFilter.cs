using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Menu;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text.Json;

namespace ICMD.API.Helpers
{
    public class PermissionFilter : Attribute, IAuthorizationFilter
    {
        /*private readonly IAuthorizationService _authService*/
        private readonly PermissionRequirement _requirement;
        private readonly IMemoryCache _memoryCache;
        private readonly IProjectUserService _projectUserService;

        public PermissionFilter(PermissionRequirement requirement, IMemoryCache memoryCache, IProjectUserService projectUserService)
        {
            /*_authService = authService*/
            ;
            _requirement = requirement;
            _memoryCache = memoryCache;
            _projectUserService = projectUserService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //AuthorizationResult ok = await _authService.AuthorizeAsync(context.HttpContext.User, null, _requirement);
            //if (!ok.Succeeded) context.Result = new ChallengeResult();

            string controllerName = context.RouteData.Values["controller"]?.ToString() ?? string.Empty;

            var claims = context.HttpContext.User.Claims.ToList();
            string? currentUserRoleName = claims.FirstOrDefault(x => x.Type == IdentityClaimNames.RoleName)?.Value;
            bool isSystem = currentUserRoleName == RoleConstants.Administrator;

            bool hasPermission = false;
            if (!isSystem)
            {
                MenuAndPermissionListDto menus = new MenuAndPermissionListDto();
                string? userId = claims.FirstOrDefault(x => x.Type == IdentityClaimNames.UserId)?.Value ?? "";
                if (_memoryCache.TryGetValue(userId + "_" + IdentityClaimNames.MenuAndPermission, out string cacheValue))
                {
                    menus = JsonSerializer.Deserialize<MenuAndPermissionListDto>(cacheValue);
                }

                if (menus != null)
                {

                    List<MenuItemListDto> menuItems = menus.MenuItems ?? new();
                    List<UserPermissionDto> menuAndPermission = menus != null && menus.Permissions != null ? menus.Permissions.Where(s => s.ControllerName == controllerName).ToList() :
                        new List<UserPermissionDto>();

                    bool isCheck = menuItems.Any(a => a.ControllerName == controllerName || (a.SubMenu != null && a.SubMenu.Any(q => q.ControllerName == controllerName)));

                    if (_requirement.Operation != null && _requirement.Operation.Length != 0)
                    {
                        hasPermission = _requirement.Operation.Any(x =>
                        {
                            return (menuAndPermission != null && menuAndPermission.Any(s => s.PermissionName
                            .Any(s => s == controllerName + "_" + x)));
                        });

                        //Project Permission
                        #region Check User Project Permission                        

                        if (_memoryCache.TryGetValue(userId + "_" + IdentityClaimNames.CurrentUserProject, out string? currentUserProject) && !string.IsNullOrEmpty(currentUserProject))
                        {
                            ProjectUser? projectUser = _projectUserService.GetSingle(x => x.IsActive && !x.IsDeleted && x.ProjectId == new Guid(currentUserProject) && x.UserId == new Guid(userId)
                                    && x.Project != null && !x.Project.IsDeleted && x.IsActive);

                            if (projectUser != null)
                            {
                                var adminTypeName = GetDisplayName(AuthorizationTypes.Administrator);
                                var readOnlyTypeName = GetDisplayName(AuthorizationTypes.ReadOnly);

                                bool hasAdminType = (projectUser.Authorization == adminTypeName);
                                if (hasAdminType || (projectUser.Authorization == readOnlyTypeName))
                                    hasPermission = hasAdminType;
                            }
                        }
                        #endregion

                    }
                    else if (_requirement.Operation != null && _requirement.Operation.Length == 0)
                    {
                        hasPermission = isCheck;
                    }

                }
            }

            if ((_requirement == null || !hasPermission) && !isSystem)
            {
                context.Result = new ChallengeResult();
            }

        }

        private string? GetDisplayName(Enum value)
        {
            return value.GetType()?.GetMember(value.ToString())?.FirstOrDefault()
                                ?.GetCustomAttribute<DisplayAttribute>()?.GetName();
        }
    }
}
