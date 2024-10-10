using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Authorization;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.Dtos.Role;
using ICMD.Core.Dtos.User;
using ICMD.Core.Shared.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Linq.Dynamic.Core;
using ICMD.EntityFrameworkCore.Database;
using ICMD.Core.Shared.Interface;
using ICMD.API.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly UserManager<ICMDUser> _userManager;
        private readonly RoleManager<ICMDRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IProjectUserService _projectUserService;
        private readonly ICMDDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        public UserController(UserManager<ICMDUser> userManager, IMapper mapper, RoleManager<ICMDRole> roleManager, ICMDDbContext dbContext, IProjectUserService projectUserService, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _projectUserService = projectUserService;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<UserListDto>> GetAllUser(PagedAndSortedResultRequestDto input)
        {
            ICMDUser? getHostUser = await _userManager.Users.FirstOrDefaultAsync(s => s.Email == "admin@gmail.com");
            IQueryable<UserListDto> allUsers = (from um in _userManager.Users
                                                join ur in _dbContext.ICMDUserRole on um.Id equals ur.UserId
                                                join rm in _dbContext.ICMDRole on ur.RoleId equals rm.Id
                                                where (getHostUser != null && um.Id != getHostUser.Id)
                                                select new UserListDto
                                                {
                                                    Id = um.Id,
                                                    FullName = um.FirstName + " " + um.LastName,
                                                    UserName = um.UserName ?? string.Empty,
                                                    RoleName = rm.DisplayName ?? "",
                                                    Email = um.Email ?? string.Empty,
                                                    PhoneNumber = um.PhoneNumber ?? string.Empty,
                                                    IsDeleted = um.IsDeleted,
                                                });


            if (!string.IsNullOrEmpty(input.Search))
            {
                allUsers = allUsers.Where(s => s.FullName.ToLower().Contains(input.Search.ToLower()) ||
                s.UserName.ToLower().Contains(input.Search.ToLower()) ||
                s.Email.ToLower().Contains(input.Search.ToLower()) || s.PhoneNumber.ToLower().Contains(input.Search.ToLower()));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "roles".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        List<string> roles = item.FieldValue.Split(',').ToList();
                        allUsers = allUsers.Where(x => roles.Contains(x.RoleName));
                    }


                    if (item.FieldName.ToLower() == "type".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        int value = Convert.ToInt16(item.FieldValue);
                        if (value == (int)RecordType.Active)
                        {
                            allUsers = allUsers.Where(x => !x.IsDeleted);
                        }
                        else if (value == (int)RecordType.Inactive)
                        {
                            allUsers = allUsers.Where(x => x.IsDeleted);
                        }
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allUsers = allUsers.Where(input.SearchColumnFilterQuery);


            allUsers = allUsers.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<UserListDto> paginatedData = !isExport ? allUsers.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allUsers;
            List<UserListDto> usersLists = await paginatedData.ToListAsync();


            return new PagedResultDto<UserListDto>(
               allUsers.Count(),
                usersLists
           );
        }

        [HttpGet]
        public async Task<UserInfoDto?> GetUserInfo(Guid userId)
        {
            ICMDUser? userDetails = await _userManager.Users.SingleOrDefaultAsync(s => s.Id == userId && !s.IsDeleted && s.IsActive);
            if (userDetails != null)
            {
                UserInfoDto userInfo = _mapper.Map<UserInfoDto>(userDetails);
                IList<string> roles = await _userManager.GetRolesAsync(userDetails);
                userInfo.RoleName = string.Join(",", roles.ToList());
                return userInfo;
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<BaseResponse> CreateUser(CreateOrEditUserDto info)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(info.Email.Trim()))
                {
                    ICMDUser? checkEmailExist = await _userManager.Users.FirstOrDefaultAsync(s => s.Email != null && s.Email.ToLower().Trim() == info.Email.ToLower().Trim() && !s.IsDeleted && s.IsActive);
                    if (checkEmailExist != null)
                        return new BaseResponse(false, ResponseMessages.EmailAlreadyTaken, HttpStatusCode.Conflict);
                }

                if (!string.IsNullOrEmpty(info.PhoneNumber?.Trim()))
                {
                    ICMDUser? checkEmailExist = await _userManager.Users.FirstOrDefaultAsync(s => s.PhoneNumber != null && s.PhoneNumber.ToLower().Trim() == info.PhoneNumber.ToLower().Trim() && !s.IsDeleted && s.IsActive);
                    if (checkEmailExist != null)
                        return new BaseResponse(false, ResponseMessages.PhoneNoAlreadyTaken, HttpStatusCode.Conflict);
                }

                ICMDUser? existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName != null && x.UserName.ToLower().Trim() == info.UserName.ToLower().Trim());
                if (existingUser != null)
                    return new BaseResponse(false, ResponseMessages.UsernameAlreadyTaken, HttpStatusCode.Conflict);


                ICMDUser icmdUser = _mapper.Map<ICMDUser>(info);
                icmdUser.CreatedBy = User.GetUserId();
                icmdUser.CreatedDate = DateTime.UtcNow;
                var result = await _userManager.CreateAsync(icmdUser, info.Password);
                if (!result.Succeeded)
                    return new BaseResponse(false, ResponseMessages.UserNotCreated, HttpStatusCode.InternalServerError);

                var roleResult = await _userManager.AddToRoleAsync(icmdUser, info.RoleName);
                if (!roleResult.Succeeded)
                    return new BaseResponse(false, ResponseMessages.RoleNotCreated, HttpStatusCode.InternalServerError);

                return new BaseResponse(true, ResponseMessages.UserCreated, HttpStatusCode.OK);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [AuthorizePermission(Operations.Edit)]
        public async Task<BaseResponse> UpdateUser(UpdateUserDto info)
        {
            if (ModelState.IsValid)
            {
                ICMDUser? userDetails = await _userManager.Users.SingleOrDefaultAsync(s => s.Id == info.Id && !s.IsDeleted && s.IsActive);
                if (userDetails == null)
                    return new BaseResponse(false, ResponseMessages.UserNotExist, HttpStatusCode.BadRequest);

                if (!string.IsNullOrEmpty(info.Email.Trim()))
                {
                    ICMDUser? checkEmailExist = await _userManager.Users.FirstOrDefaultAsync(s => s.Id != info.Id && s.Email != null && s.Email.ToLower().Trim() == info.Email.ToLower().Trim() && !s.IsDeleted && s.IsActive);
                    if (checkEmailExist != null)
                        return new BaseResponse(false, ResponseMessages.EmailAlreadyTaken, HttpStatusCode.Conflict);
                }

                if (!string.IsNullOrEmpty(info.PhoneNumber?.Trim()))
                {
                    ICMDUser? checkEmailExist = await _userManager.Users.FirstOrDefaultAsync(s => s.Id != info.Id && s.PhoneNumber != null && s.PhoneNumber.ToLower().Trim() == info.PhoneNumber.ToLower().Trim() && !s.IsDeleted && s.IsActive);
                    if (checkEmailExist != null)
                        return new BaseResponse(false, ResponseMessages.PhoneNoAlreadyTaken, HttpStatusCode.Conflict);
                }

                ICMDUser icmdUser = _mapper.Map<ICMDUser>(info);
                icmdUser = _mapper.Map<UpdateUserDto, ICMDUser>(info, userDetails);

                var result = await _userManager.UpdateAsync(icmdUser);
                if (!result.Succeeded)
                    return new BaseResponse(false, ResponseMessages.UserNotUpdated, HttpStatusCode.InternalServerError);
                else
                {
                    IList<string> currentUserRole = await _userManager.GetRolesAsync(icmdUser);
                    if (!string.IsNullOrEmpty(info.RoleName) && info.RoleName != currentUserRole[0])
                    {
                        await _userManager.RemoveFromRolesAsync(icmdUser, currentUserRole);
                        await _userManager.AddToRoleAsync(icmdUser, info.RoleName);
                    }
                }

                return new BaseResponse(true, ResponseMessages.UserUpdated, HttpStatusCode.OK);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);

        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteUser(Guid userId)
        {
            ICMDUser? userDetails = await _userManager.Users.SingleOrDefaultAsync(s => s.Id == userId && !s.IsDeleted && s.IsActive);
            if (userDetails == null)
                return new BaseResponse(false, ResponseMessages.UserNotExist, HttpStatusCode.BadRequest);

            bool isChkExist = _projectUserService.GetAll(s => (s.Project != null && !s.Project.IsDeleted) && s.IsActive && !s.IsDeleted && s.UserId == userId).Any();
            if (isChkExist)
                return new BaseResponse(false, ResponseMessages.UserNotDeleteAlreadyAssigned, HttpStatusCode.InternalServerError);

            userDetails.IsDeleted = true;
            var result = await _userManager.UpdateAsync(userDetails);
            if (!result.Succeeded)
                return new BaseResponse(false, ResponseMessages.UserNotDeleted, HttpStatusCode.InternalServerError);

            return new BaseResponse(true, ResponseMessages.UserDeleted, HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<List<RoleDropDownDto>> GetAllRolesInfo()
        {
            List<ICMDRole> allRoles = await _roleManager.Roles.ToListAsync();
            return _mapper.Map<List<RoleDropDownDto>>(allRoles.OrderBy(x => RoleOrder.Roles[x.Name]));
        }

        [HttpGet]
        public async Task<List<UserDropdownDto>> GetAllUserInfo()
        {
            List<ICMDUser> users = await _userManager.Users.Where(s => s.Email != "admin@gmail.com" && !s.IsDeleted && s.IsActive).ToListAsync();
            return _mapper.Map<List<UserDropdownDto>>(users);
        }

        [HttpGet]
        public async Task<BaseResponse> UpdateUserProject(Guid projectId)
        {
            Guid userId = User.GetUserId();
            ICMDUser? userDetails = await _userManager.Users.SingleOrDefaultAsync(s => s.Id == userId && !s.IsDeleted && s.IsActive);
            if (userDetails == null)
                return new BaseResponse(false, ResponseMessages.UserNotExist, HttpStatusCode.BadRequest);

            _memoryCache.Set(userId + "_" + IdentityClaimNames.CurrentUserProject, projectId, new MemoryCacheEntryOptions());

            userDetails.ProjectId = projectId;
            await _userManager.UpdateAsync(userDetails);
            return new BaseResponse(true, ResponseMessages.UserUpdated, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<BaseResponse> UpdateMyProfile(UpdateUserDto info)
        {
            return await UpdateUser(info);
        }

    }
}
