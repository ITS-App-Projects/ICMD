using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Project;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Linq.Dynamic.Core;
using ICMD.Core.Dtos;
using ICMD.API.Helpers;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProjectController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly IProjectUserService _projectUserService;
        private readonly ITagFieldService _tagFieldService;
        private readonly CommonMethods _commonMethods;
        private readonly IMapper _mapper;
        private static string[] defaultTagFieldNames = { "P&ID Type", "Asset Number", "Tag Type", "Descriptor", "Not Used", "Not Used" };
        private static TagFieldSource[] defaultTagFieldSource = { TagFieldSource.Process, TagFieldSource.HandTyped, TagFieldSource.TagTypeId, TagFieldSource.Descriptor };

        public ProjectController(IMapper mapper, IProjectService projectService, IProjectUserService projectUserService, ITagFieldService tagFieldService, CommonMethods commonMethods)
        {
            _mapper = mapper;
            _projectService = projectService;
            _projectUserService = projectUserService;
            _tagFieldService = tagFieldService;
            _commonMethods = commonMethods;
        }

        #region Project
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ProjectListDto>> GetAllProjects(PagedAndSortedResultRequestDto input)
        {
            IQueryable<ProjectListDto> allProjects = _projectService.GetAll(s => !s.IsDeleted).Select(s => new ProjectListDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                IsActive = s.IsActive
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allProjects = allProjects.Where(s => (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "type".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        int value = Convert.ToInt16(item.FieldValue);
                        if (value == (int)RecordType.Active)
                        {
                            allProjects = allProjects.Where(x => x.IsActive);
                        }
                        else if (value == (int)RecordType.Inactive)
                        {
                            allProjects = allProjects.Where(x => !x.IsActive);
                        }
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allProjects = allProjects.Where(input.SearchColumnFilterQuery);

            allProjects = allProjects.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<ProjectListDto> paginatedData = !isExport ? allProjects.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allProjects;


            return new PagedResultDto<ProjectListDto>(
               allProjects.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<ProjectInfoDto?> GetProjectInfo(Guid id)
        {
            Project projectDetail = await _projectService.GetSingleAsync(s => !s.IsDeleted && s.Id == id);
            if (projectDetail != null)
            {
                ProjectInfoDto projectInfo = _mapper.Map<ProjectInfoDto>(projectDetail);
                projectInfo.UserAuthorizations = _mapper.Map<List<UserAuthorizationDto>>(await _projectUserService.GetAll(s => !s.IsDeleted && s.ProjectId == id).ToListAsync());
                return projectInfo;
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditProject(CreateOrEditProjectDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateProject(info);
            }
            else
            {
                return await UpdateProject(info);
            }
        }

        private async Task<BaseResponse> CreateProject(CreateOrEditProjectDto info)
        {
            if (ModelState.IsValid)
            {
                Project existingProject = await _projectService.GetSingleAsync(x => x.Name.ToLower().Trim() == info.Name.ToLower().Trim() && !x.IsDeleted);
                if (existingProject != null)
                    return new BaseResponse(false, ResponseMessages.ProjectNameAlreadyTaken, HttpStatusCode.Conflict);

                Guid userId = User.GetUserId();
                Project projectInfo = _mapper.Map<Project>(info);
                projectInfo.IsActive = true;
                var response = await _projectService.AddAsync(projectInfo, userId);

                if (info.UserAuthorizations != null && info.UserAuthorizations.Any())
                {
                    foreach (var item in info.UserAuthorizations)
                    {
                        ProjectUser projectUserInfo = _mapper.Map<ProjectUser>(item);
                        projectUserInfo.ProjectId = projectInfo.Id;
                        projectUserInfo.IsActive = true;
                        await _projectUserService.AddAsync(projectUserInfo, userId);
                    }
                }

                for (int i = 0; i < defaultTagFieldNames.Count(); i++)
                {
                    TagField tagField = new TagField()
                    {
                        Name = defaultTagFieldNames[i],
                        Position = i,
                        Source = i < defaultTagFieldSource.Count() ? defaultTagFieldSource[i].ToString() : "",
                        ProjectId = projectInfo.Id,
                        IsActive = true
                    };
                    await _tagFieldService.AddAsync(tagField, userId);
                }

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ProjectNotCreated, HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ProjectCreated, HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateProject(CreateOrEditProjectDto info)
        {
            if (ModelState.IsValid)
            {
                Guid userId = User.GetUserId();
                Project projectDetail = await _projectService.GetSingleAsync(s => s.Id == info.Id && !s.IsDeleted);
                if (projectDetail == null)
                    return new BaseResponse(false, ResponseMessages.ProjectNotExist, HttpStatusCode.BadRequest);

                Project existingProject = await _projectService.GetSingleAsync(x => x.Id != info.Id && x.Name.ToLower().Trim() == info.Name.ToLower().Trim() && !x.IsDeleted);
                if (existingProject != null)
                    return new BaseResponse(false, ResponseMessages.ProjectNameAlreadyTaken, HttpStatusCode.Conflict);

                Project projectInfo = _mapper.Map<Project>(info);
                projectInfo.CreatedBy = projectDetail.CreatedBy;
                projectInfo.CreatedDate = projectDetail.CreatedDate;
                projectInfo.IsActive = projectDetail.IsActive;
                var response = _projectService.Update(projectInfo, projectDetail, userId);
                List<ProjectUser> removePorjectUserDetails = await _projectUserService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == info.Id &&
               !info.UserAuthorizations.Select(s => s.UserId).Contains(s.UserId)).ToListAsync();
                if (removePorjectUserDetails.Count() != 0)
                {
                    foreach (var item in removePorjectUserDetails)
                    {
                        item.IsDeleted = true;
                        _projectUserService.Update(item, item, userId, true, true);
                    }
                }

                if (info.UserAuthorizations != null && info.UserAuthorizations.Any())
                {
                    foreach (var item in info.UserAuthorizations)
                    {
                        ProjectUser chkExistProject = await _projectUserService.GetAll(s => s.Id == item.Id).FirstOrDefaultAsync();
                        if (chkExistProject == null)
                        {
                            ProjectUser projectUserInfo = _mapper.Map<ProjectUser>(item);
                            projectUserInfo.ProjectId = projectInfo.Id;
                            projectUserInfo.IsActive = true;
                            await _projectUserService.AddAsync(projectUserInfo, userId);
                        }
                        else
                        {
                            ProjectUser oldprojectUserInfo = chkExistProject;
                            chkExistProject.Authorization = item.Authorization;
                            _projectUserService.Update(chkExistProject, oldprojectUserInfo, userId);
                        }
                    }
                }

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ProjectNotUpdated, HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ProjectUpdated, HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteProject(Guid id)
        {
            Guid userId = User.GetUserId();
            Project projectDetail = await _projectService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (projectDetail != null)
            {
                //project user
                List<ProjectUser> projectUsers = await _projectUserService.GetAll(s => !s.IsDeleted && s.ProjectId == id).ToListAsync();
                if (projectUsers.Any())
                {
                    foreach (var item in projectUsers)
                    {
                        item.IsDeleted = true;
                        _projectUserService.Update(item, item, userId, true, true);
                    }
                }

                //tag field
                List<TagField> projectTagField = await _tagFieldService.GetAll(s => !s.IsDeleted && s.ProjectId == id).ToListAsync();
                if (projectTagField.Any())
                {
                    foreach (var item in projectTagField)
                    {
                        item.IsDeleted = true;
                        _tagFieldService.Update(item, item, userId, true, true);
                    }
                }
                projectDetail.IsDeleted = true;
                var response = _projectService.Update(projectDetail, projectDetail, userId, true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ProjectNotDeleted, HttpStatusCode.InternalServerError);

                return new BaseResponse(true, ResponseMessages.ProjectDeleted, HttpStatusCode.OK);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ProjectNotExist, HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        [AuthorizePermission(Operations.ActiveInActive)]
        public async Task<BaseResponse> ActiveInActiveProject(ActiveInActiveDto info)
        {
            Guid userId = User.GetUserId();
            Project projectDetail = await _projectService.GetSingleAsync(s => s.Id == info.Id);
            if (projectDetail != null)
            {
                Project oldProjectDetails = projectDetail;
                projectDetail.IsActive = info.IsActive;
                var response = _projectService.Update(projectDetail, oldProjectDetails, userId);

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ProjectNotUpdated, HttpStatusCode.InternalServerError);

                return new BaseResponse(true, info.IsActive ? ResponseMessages.ProjectActivate : ResponseMessages.ProjectDeactivate, HttpStatusCode.OK);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ProjectNotExist, HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public async Task<List<DropdownInfoDto>> GetAllProjectsInfo()
        {
            List<Project> projects = await _projectService.GetAll(s => !s.IsDeleted && s.IsActive).OrderByDescending(s => s.CreatedDate).ToListAsync();
            List<DropdownInfoDto> projectList = _mapper.Map<List<DropdownInfoDto>>(projects);
            var projectIds = projectList.Select(p => p.Id).ToList();
            var userId = User.GetUserId();

            if (User.IsInRole(RoleConstants.SuperUser) || User.IsInRole(RoleConstants.User))
            {
                var userProjectAuthorizations = await _projectUserService.GetAll(u => !u.IsDeleted && projectIds.Contains(u.ProjectId) && u.UserId == userId).ToListAsync();
                var authorizationDictionary = userProjectAuthorizations.ToDictionary(u => u.ProjectId, u => u.Authorization);

                var tempProjectList = projectList.Select(p => new DropdownInfoDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Authorization = authorizationDictionary.ContainsKey(p.Id) ? authorizationDictionary[p.Id] : null
                }).ToList();
                projectList = tempProjectList.Where(p => authorizationDictionary.ContainsKey(p.Id)).ToList();
            }

            return projectList;
        }

        [HttpGet]
        public async Task<List<string>> GetProjectTagFieldNames(Guid id)
        {
            return await _commonMethods.GetProjectTagFieldTableName(id);
        }

        [HttpGet]
        public async Task<List<ProjectTagFieldInfoDto>> GetProjectTagFieldSourcesDataInfo(Guid id)
        {
            return await _commonMethods.GetProjectTagFieldDataInfo(id);
        }
        #endregion
    }
}
