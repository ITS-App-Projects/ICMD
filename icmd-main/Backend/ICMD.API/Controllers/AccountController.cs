using AutoMapper;
using ICMD.API.Auth;
using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Authorization;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Menu;
using ICMD.Core.Dtos.User;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Data;
using System.Net;

namespace ICMD.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        private readonly UserManager<ICMDUser> _userManager;
        private readonly SignInManager<ICMDUser> _signInManager;
        private readonly TokenProvider _tokenProvider;
        private readonly IMapper _mapper;
        private readonly IProjectService _projectService;
        private readonly IMenuPermissionService _menuPermissionService;
        private readonly IPermissionManagementService _permissionManagementService;
        private readonly RoleManager<ICMDRole> _roleManager;
        private readonly CommonMethods _commonMethods;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IProjectUserService _projectUserService;

        public AccountController(UserManager<ICMDUser> userManager, SignInManager<ICMDUser> signInManager, TokenProvider tokenProvider, IMapper mapper,
            IProjectService projectService, IMenuPermissionService menuPermissionService, IPermissionManagementService permissionManagementService, RoleManager<ICMDRole> roleManager,
            CommonMethods commonMethods, IMemoryCache memoryCache, IConfiguration configuration, IProjectUserService projectUserService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenProvider = tokenProvider;
            _mapper = mapper;
            _projectService = projectService;
            _menuPermissionService = menuPermissionService;
            _permissionManagementService = permissionManagementService;
            _roleManager = roleManager;
            _commonMethods = commonMethods;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _projectUserService = projectUserService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseModel> Login(LoginRequestDto requestDto)
        {
            ResponseModel loginResponseModel = new();
            if (ModelState.IsValid)
            {
                ICMDUser? user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName != null && x.UserName.ToLower().Trim() == requestDto.UserName.ToLower().Trim() || (x.Email != null && x.Email.ToLower().Trim() == requestDto.UserName.ToLower().Trim()));

                if (user == null || user.IsDeleted)
                    return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.LoginInvalid };

                if (!user.IsActive)
                    return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.AccountDeactivate };

                List<ProjectUser> assignedProjects = new();
                List<Project> projects = await _projectService.GetAll(s => !s.IsDeleted && s.IsActive).OrderByDescending(s => s.CreatedDate).ToListAsync();
                IList<string> roles = await _userManager.GetRolesAsync(user);
                if (roles[0] != RoleConstants.Administrator)
                {
                    assignedProjects = await _projectUserService.GetAll(s => !s.IsDeleted && s.UserId == user.Id).ToListAsync();
                    if (!assignedProjects.Any())
                        return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.ProjectNotAssigned };
                }

                // reset password
                var tokenReset = await _userManager.GeneratePasswordResetTokenAsync(user);
                var userPassword = await _userManager.ResetPasswordAsync(user, tokenReset, requestDto.Password);

                var loginResponse = await _signInManager.PasswordSignInAsync(user, requestDto.Password, requestDto.RememberMe, false);
                if (loginResponse.Succeeded)
                {
                    List<Guid> rolesInfo = await _roleManager.Roles.Where(s => roles.Contains(s.Name ?? string.Empty)).Select(s => s.Id).ToListAsync();
                    MenuAndPermissionListDto menuAndPermission = await GetAllMenuList(rolesInfo.FirstOrDefault(), roles[0] == RoleConstants.Administrator);
                    JwtToken token = _tokenProvider.GenerateTokenAsync(User, user, roles, roles != null ? roles[0] != RoleConstants.Administrator ? menuAndPermission : null : null);

                    if (roles != null && roles[0] == RoleConstants.Administrator)
                        user.ProjectId = projects.Any() ? projects.Select(a => a.Id).FirstOrDefault() : null;
                    else
                        user.ProjectId = assignedProjects.Any() ? assignedProjects.OrderByDescending(s => s.Project.CreatedDate).Select(s => s.ProjectId).FirstOrDefault() : null;

                    await _userManager.UpdateAsync(user);
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    //.SetSlidingExpiration(TimeSpan.FromSeconds(3));
                    if (user.ProjectId != null)
                        _memoryCache.Set(user.Id + "_" + IdentityClaimNames.CurrentUserProject, user.ProjectId.Value.ToString(), new MemoryCacheEntryOptions());

                    _memoryCache.Set(user.Id + "_" + IdentityClaimNames.MenuAndPermission, menuAndPermission != null ? JsonSerializer.Serialize(menuAndPermission) : "", cacheEntryOptions);

                    loginResponseModel = new ResponseModel
                    {
                        IsSucceeded = true,
                        Token = token.Value,
                        Message = ResponseMessages.LoginSuccess,
                        Data = new
                        {
                            MenuPermission = JsonSerializer.Serialize(menuAndPermission, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            })
                        }
                    };
                }
                else
                {
                    loginResponseModel = new ResponseModel { IsSucceeded = false, ErrorType = 0, Message = ResponseMessages.LoginFail };
                }
            }
            else
                loginResponseModel = new ResponseModel { IsSucceeded = false, ErrorType = 0, Message = ResponseMessages.GlobalModelValidationMessage };

            return loginResponseModel;
        }

        [HttpPost]
        [Authorize]
        public async Task<BaseResponse> Register(CreateOrEditUserDto requestDto)
        {
            if (ModelState.IsValid)
            {
                ICMDUser? existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName != null && x.UserName.ToLower().Trim() == requestDto.UserName.ToLower().Trim());
                if (existingUser != null)
                    return new BaseResponse(false, ResponseMessages.UsernameAlreadyTaken, HttpStatusCode.Conflict);

                ICMDUser icmdUser = _mapper.Map<ICMDUser>(requestDto);
                icmdUser.CreatedBy = User.GetUserId();
                icmdUser.CreatedDate = DateTime.UtcNow;

                var result = await _userManager.CreateAsync(icmdUser, requestDto.Password);
                if (!result.Succeeded)
                    return new BaseResponse(false, ResponseMessages.UserNotCreated, HttpStatusCode.InternalServerError);
                else
                {
                    var roleResult = await _userManager.AddToRoleAsync(icmdUser, requestDto.RoleName);
                    if (!roleResult.Succeeded)
                        return new BaseResponse(false, ResponseMessages.RoleNotCreated, HttpStatusCode.InternalServerError);

                    return new BaseResponse(true, ResponseMessages.UserRegisterSuccess, HttpStatusCode.OK);
                }
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            string userId = User.GetUserId().ToString();
            foreach (var item in MemoryCacheItems.cacheItems)
            {
                string cacheKey = $"{userId}_{item}";
                if (_memoryCache.TryGetValue(cacheKey, out var itemValue))
                    _memoryCache.Remove(cacheKey);
            }
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<string?> RefreshToken(Guid id)
        {
            ICMDUser? user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null && !user.IsDeleted)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                List<Guid> rolesInfo = await _roleManager.Roles.Where(s => roles.Contains(s.Name ?? string.Empty)).Select(s => s.Id).ToListAsync();
                MenuAndPermissionListDto menuAndPermission = await GetAllMenuList(rolesInfo.FirstOrDefault(), roles[0] == RoleConstants.Administrator);
                JwtToken token = _tokenProvider.GenerateTokenAsync(User, user, roles, roles != null ? roles[0] != RoleConstants.Administrator ? menuAndPermission : null : null);

                string? tokenString = JsonSerializer.Serialize(token.Value);
                return tokenString;
            }

            return null;
        }

        //Change user's own password
        [HttpPost]
        [Authorize]
        public async Task<ResponseModel> ChangePassword(ChangePasswordModel model)
        {
            try
            {
                ICMDUser? user = await _userManager.FindByIdAsync(model.UserId.ToString());
                if (user == null) return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.UserNotExist };

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return new ResponseModel { IsSucceeded = true, ErrorType = 0, Message = ResponseMessages.PasswordChanged };
                }
                else
                {
                    if (result.Errors != null && result.Errors.Any(s => s.Code == "PasswordMismatch"))
                    {
                        return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.CurrentPasswordInValid };
                    }
                    else
                    {
                        return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.PasswordNotChanged };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSucceeded = false, ErrorType = 0, Message = ex.Message };
            }
        }

        //Change any user password by admin
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<BaseResponse> ChangeUserPassword(ChangeUserPasswordModel model)
        {
            try
            {
                ICMDUser? user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user == null) return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.UserNotExist };

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
                if (result.Succeeded)
                {
                    return new ResponseModel { IsSucceeded = true, ErrorType = 0, Message = ResponseMessages.PasswordChanged };
                }
                else
                {
                    if (result.Errors != null && result.Errors.Any(s => s.Code == "PasswordMismatch"))
                    {
                        return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.CurrentPasswordInValid };
                    }
                    else
                    {
                        return new ResponseModel { IsSucceeded = false, ErrorType = 2, Message = ResponseMessages.PasswordNotChanged };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSucceeded = false, ErrorType = 0, Message = ex.Message };
            }
        }

        private async Task<MenuAndPermissionListDto> GetAllMenuList(Guid roleId, bool isSystemAdmin)
        {
            return await _commonMethods.GetAllMenuList(roleId, isSystemAdmin);
        }
    }
}
