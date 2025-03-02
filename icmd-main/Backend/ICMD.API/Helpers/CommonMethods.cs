using AutoMapper;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.Device;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.JunctionBox;
using ICMD.Core.Dtos.Menu;
using ICMD.Core.Dtos.Project;
using ICMD.Core.Dtos.ReferenceDocumentType;
using ICMD.Core.Dtos.Stand;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Text;

namespace ICMD.API.Helpers
{
    public class CommonMethods
    {
        private readonly IProjectService _projectService;
        private readonly ITagFieldService _tagFieldService;
        private readonly IProcessService _processService;
        private readonly ISubProcessService _subProcessService;
        private readonly IStreamService _streamService;
        private readonly IEquipmentCodeService _equipmentCodeService;
        private readonly ITagTypeService _tagTypeService;
        private readonly ITagDescriptorService _tagDescriptorService;
        private readonly IMapper _mapper;
        private readonly ICMDDbContext _dbContext;
        private readonly IProjectUserService _projectUserService;
        private readonly IMenuPermissionService _menuPermissionService;
        private readonly IPermissionManagementService _permissionManagementService;
        private readonly IReferenceDocumentService _referenceDocumentService;
        private readonly IReferenceDocumentTypeService _referenceDocumentTypeService;
        private readonly CSVImport _csvImport;
        private readonly ITagService _tagService;
        private readonly IJunctionBoxService _junctionBoxService;
        private readonly IPanelService _panelService;
        private readonly Lazy<ChangeLogHelper> _changeLogHelper;
        private readonly IDeviceService _deviceService;
        private readonly IStandService _standService;
        private readonly ISkidService _skidService;

        public CommonMethods(IProjectService projectService, ITagFieldService tagFieldService, IMapper mapper, IProcessService processService, ISubProcessService subProcessService, IEquipmentCodeService equipmentCodeService, IStreamService streamService,
            ITagTypeService tagTypeService, ITagDescriptorService tagDescriptorService, ICMDDbContext dbContext, IProjectUserService projectUserService, IMenuPermissionService menuPermissionService, IPermissionManagementService permissionManagementService,
            IReferenceDocumentTypeService referenceDocumentTypeService, IReferenceDocumentService referenceDocumentService, CSVImport csvImport, ITagService tagService,
            IJunctionBoxService junctionBoxService, IPanelService panelService, Lazy<ChangeLogHelper> changeLogHelper, IDeviceService deviceService, IStandService standService, ISkidService skidService)
        {
            _projectService = projectService;
            _tagFieldService = tagFieldService;
            _mapper = mapper;
            _tagTypeService = tagTypeService;
            _tagDescriptorService = tagDescriptorService;
            _processService = processService;
            _subProcessService = subProcessService;
            _streamService = streamService;
            _equipmentCodeService = equipmentCodeService;
            _dbContext = dbContext;
            _projectUserService = projectUserService;
            _menuPermissionService = menuPermissionService;
            _permissionManagementService = permissionManagementService;
            _referenceDocumentService = referenceDocumentService;
            _csvImport = csvImport;
            _referenceDocumentTypeService = referenceDocumentTypeService;
            _tagService = tagService;
            _junctionBoxService = junctionBoxService;
            _panelService = panelService;
            _changeLogHelper = changeLogHelper;
            _deviceService = deviceService;
            _standService = standService;
            _skidService = skidService;
        }

        public async Task<List<string>> GetProjectTagFieldTableName(Guid projectId)
        {
            List<TagField> projectTagFields = await _tagFieldService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId).OrderBy(s => s.Position).ToListAsync();

            List<string> tagTables = new List<string> { "Not Used", "Not Used", "Not Used" };

            foreach (TagField tagField in projectTagFields)
            {
                TagFieldSource sourceEnum;
                if (Enum.TryParse(tagField.Source, out sourceEnum))
                {
                    switch (sourceEnum)
                    {
                        case TagFieldSource.Process:
                            tagTables[0] = tagField.Name ?? "";
                            break;
                        case TagFieldSource.SubProcess:
                            tagTables[1] = tagField.Name ?? "";
                            break;
                        case TagFieldSource.Stream:
                            tagTables[2] = tagField.Name ?? "";
                            break;
                    }
                }
            }

            return tagTables;
        }
        public async Task<List<ProjectTagFieldInfoDto>> GetProjectTagFieldDataInfo(Guid projectId)
        {
            List<ProjectTagFieldInfoDto> tagFieldData = new List<ProjectTagFieldInfoDto>();
            List<TagField> projectTagFields = await _tagFieldService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId).OrderBy(s => s.Position).ToListAsync();
            tagFieldData = _mapper.Map<List<ProjectTagFieldInfoDto>>(projectTagFields);

            List<SourceDataInfoDto> equipmentCodes = await _equipmentCodeService.GetAll(s => s.IsActive && !s.IsDeleted).OrderBy(s => s.Code).Select(s => new SourceDataInfoDto
            {
                Id = s.Id,
                Name = s.Code
            }).ToListAsync();

            List<SourceDataInfoDto> tagTypes = _mapper.Map<List<SourceDataInfoDto>>(await _tagTypeService.GetAll(s => s.IsActive && !s.IsDeleted).OrderBy(s => s.Name).ToListAsync());
            List<SourceDataInfoDto> tagDescriptors = _mapper.Map<List<SourceDataInfoDto>>(await _tagDescriptorService.GetAll(s => s.IsActive && !s.IsDeleted).OrderBy(s => s.Name).ToListAsync());
            foreach (var item in tagFieldData)
            {

                TagFieldSource sourceEnum;
                if (Enum.TryParse(item.Source, out sourceEnum))
                {
                    item.IsUsed = sourceEnum != TagFieldSource.NotUsed;
                    switch (sourceEnum)
                    {
                        case TagFieldSource.Process:
                            item.FieldData = await _processService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId).OrderBy(s => s.ProcessName).Select(s => new SourceDataInfoDto
                            {
                                Id = s.Id,
                                Name = s.ProcessName
                            }).ToListAsync();
                            break;
                        case TagFieldSource.SubProcess:
                            item.FieldData = await _subProcessService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId).OrderBy(s => s.SubProcessName).Select(s => new SourceDataInfoDto
                            {
                                Id = s.Id,
                                Name = s.SubProcessName
                            }).ToListAsync();
                            break;
                        case TagFieldSource.Stream:
                            item.FieldData = await _streamService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId).OrderBy(s => s.StreamName).Select(s => new SourceDataInfoDto
                            {
                                Id = s.Id,
                                Name = s.StreamName
                            }).ToListAsync();
                            break;
                        case TagFieldSource.EquipmentCode:
                            item.FieldData = equipmentCodes;
                            break;
                        case TagFieldSource.TagTypeId:
                            item.FieldData = tagTypes;
                            break;
                        case TagFieldSource.Descriptor:
                            item.FieldData = tagDescriptors;
                            break;
                    }

                }

            }
            return tagFieldData;
        }
        public String GenerateFullReportName(ReferenceDocument document)
        {
            var fullName = new StringBuilder(document.DocumentNumber);

            if (document.IsVDPDocumentNumber)
            {
                fullName.Append((!string.IsNullOrEmpty(document.Revision)) ? "-" + document.Revision : String.Empty);
                fullName.Append((!string.IsNullOrEmpty(document.Version)) ? "-" + document.Version : String.Empty);
                fullName.Append((!string.IsNullOrEmpty(document.Sheet)) ? "(" + document.Sheet + ")" : String.Empty);
            }
            else
            {
                fullName.Append((!string.IsNullOrEmpty(document.Revision)) ? ", rev. " + document.Revision : String.Empty);
                fullName.Append((!string.IsNullOrEmpty(document.Version)) ? ", ver. " + document.Version : String.Empty);
                fullName.Append((!string.IsNullOrEmpty(document.Sheet)) ? ", sheet " + document.Sheet : String.Empty);
            }

            return fullName.ToString();
        }

        public bool TaxFieldEditableOrNot(Guid userId, Guid? projectId, AuthorizationTypes type = AuthorizationTypes.Administrator)
        {
            var userInfo = (from iu in _dbContext.ICMDUser
                            join ur in _dbContext.UserRoles on iu.Id equals ur.UserId
                            join ir in _dbContext.Roles on ur.RoleId equals ir.Id
                            where iu.Id == userId
                            select new
                            {
                                User = iu,
                                Role = ir.Name
                            }).FirstOrDefault();


            if (userInfo != null)
            {
                if (userInfo.Role == RoleConstants.Administrator)
                    return true;

                var authorizations = _projectUserService.GetAll(s => s.IsActive && !s.IsDeleted && s.UserId == userInfo.User.Id).ToList();
                if (authorizations.Count() == 0)
                    return false;

                ProjectUser? projectAuthorization = authorizations.FirstOrDefault(a => a.ProjectId == projectId);
                if (projectAuthorization == null)  // not a member of project
                {
                    return false;
                }

                AuthorizationTypes authorizationEnum;
                if (Enum.TryParse(projectAuthorization.Authorization, out authorizationEnum))
                {
                    return type == AuthorizationTypes.ReadOnly || type == authorizationEnum ||
                        type == AuthorizationTypes.ReadWrite && authorizationEnum == AuthorizationTypes.Administrator;
                }
            }
            return false;
        }


        public List<KeyValueInfoDto> IsInstrumentSelectList()
        {
            List<KeyValueInfoDto> IsInstrumentOptions = new List<KeyValueInfoDto>();
            IsInstrumentOptions.Add(CreateKeyValue("Yes", IsInstrumentOption.Yes));
            IsInstrumentOptions.Add(CreateKeyValue("No", IsInstrumentOption.No));
            IsInstrumentOptions.Add(CreateKeyValue("Both", IsInstrumentOption.Both));
            IsInstrumentOptions.Add(CreateKeyValue("None", IsInstrumentOption.None));

            return IsInstrumentOptions;
        }

        public async Task<MenuAndPermissionListDto> GetAllMenuList(Guid roleId, bool isSystemAdmin)
        {
            //Menu Permission
            List<MenuPermission> MenuPermission = await _menuPermissionService.GetAll(x => x.IsActive && !x.IsDeleted && x.RoleId == roleId && x.IsGranted).Where(s => s.MenuItems.IsActive && !s.IsDeleted).ToListAsync();

            //All Permission Management
            List<Guid> menuPermissionIds = MenuPermission.Select(x => x.Id).ToList();
            List<PermissionManagement> permissionManagements = await _permissionManagementService.GetAll(s => s.IsActive && !s.IsDeleted && s.IsGranted && menuPermissionIds.Contains(s.MenuPermissionId)).ToListAsync();

            //Permissions
            List<UserPermissionDto> userPermissions = permissionManagements
                .GroupBy(pm => new
                {
                    pm.MenuPermissionId,
                    pm.MenuPermission.MenuItems.ControllerName,
                    pm.MenuPermission.MenuItems.MenuName,
                    //pm.MenuPermission.MenuItems.MenuDescription,
                    pm.MenuPermission.MenuItems.Url
                }).Select(group => new UserPermissionDto
                {
                    ControllerName = group.Key.ControllerName,
                    //MenuName = group.Key.MenuName,
                    URL = group.Key.Url,
                    //MenuDescription = group.Key.MenuDescription ?? string.Empty,
                    PermissionName = group.Select(x => group.Key.ControllerName + "_" + Enum.GetName(typeof(Operations), (int)x.Operation)).ToList()
                }).ToList();

            List<MenuItemListDto> menuItemLists = new();
            foreach (MenuPermission menu in MenuPermission.Where(a => a.MenuItems.ParentMenuId == null).OrderBy(a => a.MenuItems.SortOrder))
            {
                List<Guid> ids = MenuPermission.Where(s => s.MenuItems?.ParentMenuId == menu.MenuId).Select(s => s.Id).ToList();
                if (menu.MenuItems.ParentMenuId == null)
                {
                    bool hasListPermission = isSystemAdmin || permissionManagements.Any(s => s.MenuPermissionId == menu.Id);
                    //List<string> MenusWithNoList = _JWTconfig.Value.NoListMenu?.Split(',').ToList() ?? new List<string>();

                    MenuItemListDto menuPermission = new()
                    {
                        //IsPermission = menu?.MenuItems?.IsPermission ?? false,
                        //MenuName = menu?.MenuItems?.MenuName ?? string.Empty,
                        MenuDescription = menu?.MenuItems?.MenuDescription ?? string.Empty,
                        Icon = menu?.MenuItems?.Icon ?? string.Empty,
                        SortOrder = menu?.MenuItems?.SortOrder ?? 0,
                        Url = menu?.MenuItems?.Url ?? string.Empty,
                        ControllerName = menu?.MenuItems?.ControllerName ?? string.Empty,
                    };
                    //var subMenu = MenuPermission.Where(s => s.MenuItems?.ParentMenuId == menu?.MenuId
                    //&& (isSystemAdmin || permissionManagements.Any(x => x.MenuPermissionId == s.Id))
                    //).Select(s => s.MenuItems).ToList();
                    var subMenu = MenuPermission.Where(s => s.MenuItems?.ParentMenuId == menu?.MenuId).Select(s => s.MenuItems).OrderBy(a => a.SortOrder).ToList();

                    List<MenuItemDto> subMenus = _mapper.Map<List<MenuItemDto>>(subMenu);
                    menuPermission.SubMenu = subMenus.ToList();

                    //if (((menuPermission.SubMenu.Count == 0)) || isSystemAdmin)
                    //{
                    menuItemLists.Add(menuPermission);
                    //}
                    //}
                }
            }
            List<string> exceptedMenus = new List<string>();
            MenuAndPermissionListDto menuAndPermissionListDto = new()
            {
                Permissions = userPermissions,
                MenuItems = menuItemLists.Where(s => !exceptedMenus.Any(x => x == s.MenuDescription)).OrderBy(x => x.SortOrder).ToList()
            };

            return menuAndPermissionListDto;
        }

        public async Task<ImportFileResultDto<JunctionBoxListDto>> CommonBulkImport([FromForm] FileUploadModel info, FileType importFileType, Guid userId, string moduleName)
        {
            List<JunctionBoxListDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            try
            {
                if (!(info.File != null && info.File.Length > 0))
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };

                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (!new[] { FileType.JunctionBox, FileType.Panel, FileType.Skid, FileType.Stand }.Contains(fileType) || typeHeaders == null)
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };

                List<string> requiredKeys = [];
                List<string> requiredExportKeys = [];
                if (importFileType == FileType.JunctionBox)
                {
                    requiredKeys = FileHeadingConstants.JunctionBoxHeadings;
                    requiredExportKeys = FileHeadingConstants.JunctionBoxExportHeadings;
                }
                else if (importFileType == FileType.Panel)
                {
                    requiredKeys = FileHeadingConstants.PanelHeadings;
                    requiredExportKeys = FileHeadingConstants.PanelExportHeadings;
                }
                else if (importFileType == FileType.Skid)
                {
                    requiredKeys = FileHeadingConstants.SkidHeadings;
                    requiredExportKeys = FileHeadingConstants.SkidExportHeadings;
                }
                else if (importFileType == FileType.Stand)
                {
                    requiredKeys = FileHeadingConstants.StandHeadings;
                    requiredExportKeys = FileHeadingConstants.StandExportHeadings;
                }

                var isEditImport = false;
                if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                    isEditImport = true;

                foreach (var columns in typeHeaders)
                {
                    var dictionary = new Dictionary<string, string>();
                    var editId = Guid.Empty;

                    foreach (var item in columns)
                    {
                        if (item.Key == FileHeadingConstants.IdHeading)
                        {
                            var isSuccess = Guid.TryParse(item.Value, out editId);
                            if (!isSuccess)
                                editId = Guid.Empty;

                            continue;
                        }

                        dictionary.Add(item.Key, item.Value);
                    }

                    var keys = dictionary.Keys.ToList();
                    if (requiredKeys.All(keys.Contains) || requiredExportKeys.All(keys.Contains))
                    {
                        string? TagName = string.Empty;
                        string? type = string.Empty;
                        string? description = string.Empty;
                        string? ReferenceDocumentTypeName = string.Empty;
                        string? ReferenceDocumentName = string.Empty;
                        string? area = string.Empty;

                        // Use export template to populate the items
                        if (requiredExportKeys.All(keys.Contains))
                        {
                            TagName = dictionary[requiredExportKeys[0]];
                            type = dictionary[requiredExportKeys[7]];
                            description = dictionary[requiredExportKeys[8]];
                            ReferenceDocumentTypeName = dictionary[requiredExportKeys[9]];
                            ReferenceDocumentName = dictionary[requiredExportKeys[10]];

                            if (importFileType == FileType.Stand)
                            {
                                type = dictionary[requiredExportKeys[8]];
                                description = dictionary[requiredExportKeys[7]];
                                area = dictionary[requiredExportKeys[9]];
                                ReferenceDocumentTypeName = dictionary[requiredExportKeys[10]];
                                ReferenceDocumentName = dictionary[requiredExportKeys[11]];
                            }
                        }
                        else
                        {
                            TagName = dictionary[requiredKeys[0]];
                            type = dictionary[requiredKeys[1]];
                            description = dictionary[requiredKeys[2]];
                            ReferenceDocumentTypeName = dictionary[requiredKeys[3]];
                            ReferenceDocumentName = dictionary[requiredKeys[4]];

                            if (importFileType == FileType.Stand)
                            {
                                type = dictionary[requiredKeys[2]];
                                description = dictionary[requiredKeys[1]];
                                area = dictionary[requiredKeys[3]];
                                ReferenceDocumentTypeName = dictionary[requiredKeys[4]];
                                ReferenceDocumentName = dictionary[requiredKeys[5]];
                            }
                        }

                        bool isSuccess = false;
                        List<string> message = [];

                        Guid? tagId = null;

                        JunctionBox? existingJunctionBox = null;
                        Panel? existingPanel = null;
                        Skid? existingSkid = null;
                        Stand? existingStand = null;
                        Guid? recordId = null;

                        var importLog = new ImportLogDto
                        {
                            Operation = OperationType.Insert
                        };

                        if (!string.IsNullOrEmpty(TagName))
                        {
                            if (importFileType == FileType.JunctionBox)
                            {
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    importLog.Operation = OperationType.Edit;
                                    existingJunctionBox = await _junctionBoxService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                        x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingJunctionBox == null)
                                    {
                                        message.Add("Record is not found.");
                                        importLog.Items = GetChanges(new JunctionBox(), new()
                                        {
                                            Type = type,
                                            Description = description,
                                        });
                                    }
                                    else
                                    {
                                        var existingRecordName = await _junctionBoxService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Junction Box Tag is already taken.");
                                            importLog.Items = GetChanges(existingJunctionBox, new()
                                            {
                                                Type = type,
                                                Description = description,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    existingJunctionBox = await _junctionBoxService.GetSingleAsync(x => x.IsActive && !x.IsDeleted && x.Tag != null && x.Tag.ProjectId == info.ProjectId && x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive && !x.Tag.IsDeleted);
                                }
                                recordId = existingJunctionBox?.Id ?? null;
                            }
                            else if (importFileType == FileType.Panel)
                            {
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    importLog.Operation = OperationType.Edit;
                                    existingPanel = await _panelService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                        x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingPanel == null)
                                    {
                                        message.Add("Record is not found.");
                                        importLog.Items = GetChanges(new JunctionBox(), new()
                                        {
                                            Type = type,
                                            Description = description,
                                        });
                                    }
                                    else
                                    {
                                        var existingRecordName = await _panelService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Panel Tag is already taken.");
                                            importLog.Items = GetChanges(existingPanel, new()
                                            {
                                                Type = type,
                                                Description = description,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    existingPanel = await _panelService.GetSingleAsync(x => x.IsActive && !x.IsDeleted && x.Tag != null && x.Tag.ProjectId == info.ProjectId && x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive && !x.Tag.IsDeleted);
                                }
                                recordId = existingPanel?.Id ?? null;
                            }
                            else if (importFileType == FileType.Skid)
                            {
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    importLog.Operation = OperationType.Edit;
                                    existingSkid = await _skidService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                        x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingSkid == null)
                                    {
                                        message.Add("Record is not found.");
                                        importLog.Items = GetChanges(new JunctionBox(), new()
                                        {
                                            Type = type,
                                            Description = description,
                                        });
                                    }
                                    else
                                    {
                                        var existingRecordName = await _skidService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Skid Tag is already taken.");
                                            importLog.Items = GetChanges(existingSkid, new()
                                            {
                                                Type = type,
                                                Description = description,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    existingSkid = await _skidService.GetSingleAsync(x => x.IsActive && !x.IsDeleted && x.Tag != null && x.Tag.ProjectId == info.ProjectId && x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive && !x.Tag.IsDeleted);
                                }
                                recordId = existingSkid?.Id ?? null;
                            }
                            else if (importFileType == FileType.Stand)
                            {
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    importLog.Operation = OperationType.Edit;
                                    existingStand = await _standService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                        x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingStand == null)
                                    {
                                        message.Add("Record is not found.");
                                        importLog.Items = GetChanges(new JunctionBox(), new()
                                        {
                                            Type = type,
                                            Description = description,
                                        });
                                    }
                                    else
                                    {
                                        var existingRecordName = await _standService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Stand Tag is already taken.");
                                            importLog.Items = GetChanges(existingStand, new()
                                            {
                                                Type = type,
                                                Description = description,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    existingStand = await _standService.GetSingleAsync(x => x.IsActive && !x.IsDeleted && x.Tag != null && x.Tag.ProjectId == info.ProjectId && x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive && !x.Tag.IsDeleted);
                                }
                                recordId = existingStand?.Id ?? null;
                            }

                            List<DropdownInfoDto> tagsInfo = await GetProjectWiseTagInfo(info.ProjectId, "", null, true);
                            tagId = tagsInfo.FirstOrDefault(x => x.Name == TagName)?.Id ?? null;
                        }

                        ReferenceDocumentType? ReferenceDocumentType = !string.IsNullOrEmpty(ReferenceDocumentTypeName) ? await _referenceDocumentTypeService.GetSingleAsync(x => x.Type == ReferenceDocumentTypeName && !x.IsDeleted) : null;

                        ReferenceDocument? ReferenceDocument = null;
                        if (!string.IsNullOrEmpty(ReferenceDocumentName) && ReferenceDocumentType != null)
                        {
                            ReferenceDocument = await _referenceDocumentService.GetSingleAsync(x => x.DocumentNumber == ReferenceDocumentName && x.ReferenceDocumentTypeId == ReferenceDocumentType.Id && !x.IsDeleted && x.ProjectId == info.ProjectId);

                            if (ReferenceDocument == null)
                            {
                                var listOfReferenceDocument = await _referenceDocumentService.GetAll(x => x.ReferenceDocumentTypeId == ReferenceDocumentType.Id & !x.IsDeleted && x.ProjectId == info.ProjectId)
                                .OrderBy(s => s.DocumentNumber)
                                .ThenBy(s => s.Version)
                                .ThenBy(s => s.Revision)
                                .ThenBy(s => s.Sheet).ToListAsync();

                                foreach (var checkRefence in listOfReferenceDocument)
                                {
                                    if (GenerateFullReportName(checkRefence) == ReferenceDocumentName)
                                    {
                                        ReferenceDocument = checkRefence;
                                        break;
                                    }
                                }
                            }
                        }

                        CreateOrEditJunctionBoxDto createDto = new();
                        CreateOrEditStandDto createStandDto = new();

                        CommonHelper helper = new();
                        Tuple<bool, List<string>> validationResponse;
                        if (importFileType != FileType.Stand)
                        {
                            createDto = new()
                            {
                                TagId = tagId ?? Guid.Empty,
                                Type = type,
                                Description = description,
                                ReferenceDocumentId = ReferenceDocument?.Id ?? null
                            };
                            validationResponse = helper.CheckImportFileRecordValidations(createDto);
                        }
                        else
                        {
                            createStandDto = new()
                            {
                                TagId = tagId ?? Guid.Empty,
                                Type = type,
                                Description = description,
                                ReferenceDocumentId = ReferenceDocument?.Id ?? null,
                                Area = area
                            };
                            validationResponse = helper.CheckImportFileRecordValidations(createStandDto);
                        }

                        isSuccess = validationResponse.Item1;
                        if (!isSuccess) message.AddRange(validationResponse.Item2);

                        if (string.IsNullOrEmpty(TagName) || (tagId == null && recordId == null))
                            message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "tag"));

                        if (!string.IsNullOrEmpty(ReferenceDocumentTypeName) && ReferenceDocumentTypeName == null)
                            message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "reference document type"));

                        if (!string.IsNullOrEmpty(ReferenceDocumentName) && ReferenceDocument == null)
                            message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "reference document number"));

                        if (isSuccess) isSuccess = message.Count == 0;

                        importLog.Name = TagName;

                        if (isEditImport && editId == Guid.Empty)
                        {
                            isSuccess = false;
                            message.Add("Id is incorrect format.");
                        }

                        if (isSuccess)
                        {
                            bool isUpdate = false;
                            try
                            {
                                if (importFileType == FileType.JunctionBox)
                                {
                                    if (recordId != null && existingJunctionBox != null)
                                    {
                                        isUpdate = true;
                                        createDto.Id = existingJunctionBox.Id;
                                        createDto.TagId = existingJunctionBox.TagId;
                                        JunctionBox model = _mapper.Map<JunctionBox>(createDto);
                                        model.CreatedBy = existingJunctionBox.CreatedBy;
                                        model.CreatedDate = existingJunctionBox.CreatedDate;

                                        importLog.Operation = OperationType.Edit;
                                        importLog.Items = GetChanges(existingJunctionBox, createDto);

                                        var response = _junctionBoxService.Update(model, existingJunctionBox, userId);
                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateJunctionBoxChangeLog(existingJunctionBox, createDto);
                                    }
                                    else
                                    {
                                        JunctionBox model = _mapper.Map<JunctionBox>(createDto);
                                        importLog.Items = GetChanges(model, createDto);
                                        var response = await _junctionBoxService.AddAsync(model, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateJunctionBoxChangeLog(new JunctionBox(), createDto);
                                    }
                                }
                                else if (importFileType == FileType.Panel)
                                {
                                    if (recordId != null && existingPanel != null)
                                    {
                                        isUpdate = true;
                                        createDto.Id = existingPanel.Id;
                                        createDto.TagId = existingPanel.TagId;
                                        Panel model = _mapper.Map<Panel>(createDto);
                                        model.CreatedBy = existingPanel.CreatedBy;
                                        model.CreatedDate = existingPanel.CreatedDate;

                                        importLog.Operation = OperationType.Edit;
                                        importLog.Items = GetChanges(existingPanel, createDto);

                                        var response = _panelService.Update(model, existingPanel, userId);
                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreatePanelChangeLog(existingPanel, createDto);
                                    }
                                    else
                                    {
                                        Panel model = _mapper.Map<Panel>(createDto);
                                        importLog.Items = GetChanges(model, createDto);
                                        var response = await _panelService.AddAsync(model, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreatePanelChangeLog(new Panel(), createDto);
                                    }
                                }
                                else if (importFileType == FileType.Skid)
                                {
                                    if (recordId != null && existingSkid != null)
                                    {
                                        isUpdate = true;
                                        createDto.Id = existingSkid.Id;
                                        createDto.TagId = existingSkid.TagId;
                                        Skid model = _mapper.Map<Skid>(createDto);
                                        model.CreatedBy = existingSkid.CreatedBy;
                                        model.CreatedDate = existingSkid.CreatedDate;

                                        importLog.Operation = OperationType.Edit;
                                        importLog.Items = GetChanges(existingSkid, createDto);

                                        var response = _skidService.Update(model, existingSkid, userId);
                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateSkidChangeLog(existingSkid, createDto);
                                    }
                                    else
                                    {
                                        Skid model = _mapper.Map<Skid>(createDto);
                                        importLog.Items = GetChanges(model, createDto);
                                        var response = await _skidService.AddAsync(model, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateSkidChangeLog(new Skid(), createDto);
                                    }
                                }
                                else if (importFileType == FileType.Stand)
                                {
                                    if (recordId != null && existingStand != null)
                                    {
                                        isUpdate = true;
                                        createStandDto.Id = existingStand.Id;
                                        createStandDto.TagId = existingStand.TagId;
                                        Stand model = _mapper.Map<Stand>(createStandDto);
                                        model.CreatedBy = existingStand.CreatedBy;
                                        model.CreatedDate = existingStand.CreatedDate;

                                        importLog.Operation = OperationType.Edit;
                                        importLog.Items = GetChanges(existingStand, createStandDto);

                                        var response = _standService.Update(model, existingStand, userId);
                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateStandChangeLog(existingStand, createStandDto);
                                    }
                                    else
                                    {
                                        Stand model = _mapper.Map<Stand>(createStandDto);
                                        importLog.Items = GetChanges(model, createStandDto);
                                        var response = await _standService.AddAsync(model, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateStandChangeLog(new Stand(), createStandDto);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                message.Add((isUpdate ? ResponseMessages.ModuleNotUpdated : ResponseMessages.ModuleNotCreated).ToString().Replace("{module}", moduleName));
                            }
                        }

                        JunctionBoxListDto record = (importFileType != FileType.Stand) ? _mapper.Map<JunctionBoxListDto>(createDto) : _mapper.Map<JunctionBoxListDto>(createStandDto);
                        record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                        record.Message = string.Join(", ", message);
                        record.Tag = TagName;
                        record.DocumentNumber = ReferenceDocumentName;
                        record.ReferenceDocumentType = ReferenceDocumentTypeName;
                        if (importFileType == FileType.Stand)
                            record.Area = createStandDto.Area;
                        responseList.Add(record);

                        importLog.Status = record.Status;
                        importLog.Message = record.Message;
                        importLogs.Add(importLog);
                    }
                }
            }
            catch (Exception ex)
            {
                return new()
                {
                    Message = ex.Message
                };
            }

            // Record logs
            await _changeLogHelper.Value.CreateImportLogs(moduleName, importLogs);

            if (responseList.Count == 0)
            {
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };
            }

            if (responseList.All(x => x.Status == ImportFileRecordStatus.Success))
            {
                return new()
                {
                    IsSucceeded = true,
                    Message = ResponseMessages.ImportFile,
                    Records = responseList
                };
            }
            else if (responseList.All(x => x.Status == ImportFileRecordStatus.Fail))
            {
                return new()
                {
                    IsSucceeded = false,
                    Message = ResponseMessages.FailedImportFile,
                    Records = responseList
                };
            }

            return new()
            {
                IsSucceeded = true,
                IsWarning = true,
                Message = ResponseMessages.SomeFailedImportFile,
                Records = responseList
            };
        }

        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateCommonBulkImport([FromForm] FileUploadModel info, FileType importFileType, Guid userId, string moduleName)
        {
            List<ValidationDataDto> validationDataList = [];
            try
            {
                if (!(info.File != null && info.File.Length > 0))
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };

                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (!new[] { FileType.JunctionBox, FileType.Panel, FileType.Skid, FileType.Stand }.Contains(fileType) || typeHeaders == null)
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };

                List<string> requiredKeys = [];
                List<string> requiredExportKeys = [];
                if (importFileType == FileType.JunctionBox)
                {
                    requiredKeys = FileHeadingConstants.JunctionBoxHeadings;
                    requiredExportKeys = FileHeadingConstants.JunctionBoxExportHeadings;
                }
                else if (importFileType == FileType.Panel)
                {
                    requiredKeys = FileHeadingConstants.PanelHeadings;
                    requiredExportKeys = FileHeadingConstants.PanelExportHeadings;
                }
                else if (importFileType == FileType.Skid)
                {
                    requiredKeys = FileHeadingConstants.SkidHeadings;
                    requiredExportKeys = FileHeadingConstants.SkidExportHeadings;
                }
                else if (importFileType == FileType.Stand)
                {
                    requiredKeys = FileHeadingConstants.StandHeadings;
                    requiredExportKeys = FileHeadingConstants.StandExportHeadings;
                }

                var transaction = await _junctionBoxService.BeginTransaction();

                var isEditImport = false;
                if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                    isEditImport = true;

                foreach (var columns in typeHeaders)
                {
                    var dictionary = new Dictionary<string, string>();
                    var editId = Guid.Empty;

                    foreach (var item in columns)
                    {
                        if (item.Key == FileHeadingConstants.IdHeading)
                        {
                            editId = Guid.Parse(item.Value);
                            continue;
                        }

                        dictionary.Add(item.Key, item.Value);
                    }

                    var keys = dictionary.Keys.ToList();
                    if (requiredKeys.All(keys.Contains) || requiredExportKeys.All(keys.Contains))
                    {
                        string? TagName = string.Empty;
                        string? type = string.Empty;
                        string? description = string.Empty;
                        string? ReferenceDocumentTypeName = string.Empty;
                        string? ReferenceDocumentName = string.Empty;
                        string? area = string.Empty;

                        // Use export template to populate the items
                        if (requiredExportKeys.All(keys.Contains))
                        {
                            TagName = dictionary[requiredExportKeys[0]];
                            type = dictionary[requiredExportKeys[7]];
                            description = dictionary[requiredExportKeys[8]];
                            ReferenceDocumentTypeName = dictionary[requiredExportKeys[9]];
                            ReferenceDocumentName = dictionary[requiredExportKeys[10]];

                            if (importFileType == FileType.Stand)
                            {
                                type = dictionary[requiredExportKeys[8]];
                                description = dictionary[requiredExportKeys[7]];
                                area = dictionary[requiredExportKeys[9]];
                                ReferenceDocumentTypeName = dictionary[requiredExportKeys[10]];
                                ReferenceDocumentName = dictionary[requiredExportKeys[11]];
                            }
                        }
                        else
                        {
                            TagName = dictionary[requiredKeys[0]];
                            type = dictionary[requiredKeys[1]];
                            description = dictionary[requiredKeys[2]];
                            ReferenceDocumentTypeName = dictionary[requiredKeys[3]];
                            ReferenceDocumentName = dictionary[requiredKeys[4]];

                            if (importFileType == FileType.Stand)
                            {
                                type = dictionary[requiredKeys[2]];
                                description = dictionary[requiredKeys[1]];
                                area = dictionary[requiredKeys[3]];
                                ReferenceDocumentTypeName = dictionary[requiredKeys[4]];
                                ReferenceDocumentName = dictionary[requiredKeys[5]];
                            }
                        }

                        bool isSuccess = false;
                        List<string> message = [];

                        Guid? tagId = null;

                        JunctionBox? existingJunctionBox = null;
                        Panel? existingPanel = null;
                        Skid? existingSkid = null;
                        Stand? existingStand = null;
                        Guid? recordId = null;

                        ValidationDataDto validationData = new()
                        {
                            Operation = OperationType.Insert
                        };

                        if (!string.IsNullOrEmpty(TagName))
                        {
                            List<DropdownInfoDto> tagsInfo = await GetProjectWiseTagInfo(info.ProjectId, "", null, true);
                            tagId = tagsInfo.FirstOrDefault(x => x.Name == TagName)?.Id ?? null;

                            if (importFileType == FileType.JunctionBox)
                            {
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    validationData.Operation = OperationType.Edit;
                                    existingJunctionBox = await _junctionBoxService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                        x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingJunctionBox == null)
                                    {
                                        message.Add("Record is not found.");
                                        validationData.Changes = GetChanges(new JunctionBox(), new()
                                        {
                                            Type = type,
                                            Description = description,
                                        });
                                    }
                                    else
                                    {
                                        var existingRecordName = await _junctionBoxService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Junction Box Tag is already taken.");
                                            validationData.Changes = GetChanges(existingJunctionBox, new()
                                            {
                                                Type = type,
                                                Description = description,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    existingJunctionBox = await _junctionBoxService.GetSingleAsync(x => x.IsActive && !x.IsDeleted && x.Tag != null && x.Tag.ProjectId == info.ProjectId && x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive && !x.Tag.IsDeleted);
                                }
                                recordId = existingJunctionBox?.Id ?? null;
                            }
                            else if (importFileType == FileType.Panel)
                            {
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    validationData.Operation = OperationType.Edit;
                                    existingPanel = await _panelService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                        x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingPanel == null)
                                    {
                                        message.Add("Record is not found.");
                                        validationData.Changes = GetChanges(new JunctionBox(), new()
                                        {
                                            Type = type,
                                            Description = description,
                                        });
                                    }
                                    else
                                    {
                                        var existingRecordName = await _panelService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Panel Tag is already taken.");
                                            validationData.Changes = GetChanges(existingPanel, new()
                                            {
                                                Type = type,
                                                Description = description,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    existingPanel = await _panelService.GetSingleAsync(x => x.IsActive && !x.IsDeleted && x.Tag != null && x.Tag.ProjectId == info.ProjectId && x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive && !x.Tag.IsDeleted);
                                }
                                recordId = existingPanel?.Id ?? null;
                            }
                            else if (importFileType == FileType.Skid)
                            {
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    validationData.Operation = OperationType.Edit;
                                    existingSkid = await _skidService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                        x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingSkid == null)
                                    {
                                        message.Add("Record is not found.");
                                        validationData.Changes = GetChanges(new JunctionBox(), new()
                                        {
                                            Type = type,
                                            Description = description,
                                        });
                                    }
                                    else
                                    {
                                        var existingRecordName = await _skidService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Skid Tag is already taken.");
                                            validationData.Changes = GetChanges(existingSkid, new()
                                            {
                                                Type = type,
                                                Description = description,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    existingSkid = await _skidService.GetSingleAsync(x => x.IsActive && !x.IsDeleted && x.Tag != null && x.Tag.ProjectId == info.ProjectId && x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive && !x.Tag.IsDeleted);
                                }
                                recordId = existingSkid?.Id ?? null;
                            }
                            else if (importFileType == FileType.Stand)
                            {
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    validationData.Operation = OperationType.Edit;
                                    existingStand = await _standService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                        x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingStand == null)
                                    {
                                        message.Add("Record is not found.");
                                        validationData.Changes = GetChanges(new JunctionBox(), new()
                                        {
                                            Type = type,
                                            Description = description,
                                        });
                                    }
                                    else
                                    {
                                        var existingRecordName = await _standService.GetSingleAsync(x => x.Tag != null && x.Tag.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Stand Tag is already taken.");
                                            validationData.Changes = GetChanges(existingStand, new()
                                            {
                                                Type = type,
                                                Description = description,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    existingStand = await _standService.GetSingleAsync(x => x.IsActive && !x.IsDeleted && x.Tag != null && x.Tag.ProjectId == info.ProjectId && x.Tag.TagName.Trim() == TagName.Trim() && x.Tag.IsActive && !x.Tag.IsDeleted);
                                }
                                recordId = existingStand?.Id ?? null;
                            }
                        }

                        ReferenceDocumentType? ReferenceDocumentType = !string.IsNullOrEmpty(ReferenceDocumentTypeName) ? await _referenceDocumentTypeService.GetSingleAsync(x => x.Type == ReferenceDocumentTypeName && !x.IsDeleted) : null;
                        ReferenceDocument? ReferenceDocument = null;
                        if (!string.IsNullOrEmpty(ReferenceDocumentName) && ReferenceDocumentType != null)
                        {
                            ReferenceDocument = await _referenceDocumentService.GetSingleAsync(x => x.DocumentNumber == ReferenceDocumentName && x.ReferenceDocumentTypeId == ReferenceDocumentType.Id && !x.IsDeleted && x.ProjectId == info.ProjectId);

                            if (ReferenceDocument == null)
                            {
                                var listOfReferenceDocument = await _referenceDocumentService.GetAll(x => x.ReferenceDocumentTypeId == ReferenceDocumentType.Id & !x.IsDeleted && x.ProjectId == info.ProjectId)
                                .OrderBy(s => s.DocumentNumber)
                                .ThenBy(s => s.Version)
                                .ThenBy(s => s.Revision)
                                .ThenBy(s => s.Sheet).ToListAsync();

                                foreach (var checkRefence in listOfReferenceDocument)
                                {
                                    if (GenerateFullReportName(checkRefence) == ReferenceDocumentName)
                                    {
                                        ReferenceDocument = checkRefence;
                                        break;
                                    }
                                }
                            }
                        }
                        

                        CreateOrEditJunctionBoxDto createDto = new();
                        CreateOrEditStandDto createStandDto = new();

                        CommonHelper helper = new();
                        Tuple<bool, List<string>> validationResponse;
                        if (importFileType != FileType.Stand)
                        {
                            createDto = new()
                            {
                                TagId = tagId ?? Guid.Empty,
                                Type = type,
                                Description = description,
                                ReferenceDocumentId = ReferenceDocument?.Id ?? null
                            };
                            validationResponse = helper.CheckImportFileRecordValidations(createDto);
                        }
                        else
                        {
                            createStandDto = new()
                            {
                                TagId = tagId ?? Guid.Empty,
                                Type = type,
                                Description = description,
                                ReferenceDocumentId = ReferenceDocument?.Id ?? null,
                                Area = area
                            };
                            validationResponse = helper.CheckImportFileRecordValidations(createStandDto);
                        }

                        isSuccess = validationResponse.Item1;
                        if (!isSuccess) message.AddRange(validationResponse.Item2);

                        if (string.IsNullOrEmpty(TagName) || (tagId == null && recordId == null))
                            message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "tag"));

                        if (!string.IsNullOrEmpty(ReferenceDocumentTypeName) && ReferenceDocumentTypeName == null)
                            message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "reference document type"));

                        if (!string.IsNullOrEmpty(ReferenceDocumentName) && ReferenceDocument == null)
                            message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "reference document number"));

                        if (isSuccess) isSuccess = message.Count == 0;

                        validationData.Name = TagName;
                        if (isEditImport && editId == Guid.Empty)
                        {
                            isSuccess = false;
                            message.Add("Id is incorrect format.");
                        }

                        if (isSuccess)
                        {
                            bool isUpdate = false;
                            try
                            {
                                if (importFileType == FileType.JunctionBox)
                                {
                                    if (recordId != null && existingJunctionBox != null)
                                    {
                                        isUpdate = true;
                                        createDto.Id = existingJunctionBox.Id;
                                        createDto.TagId = existingJunctionBox.TagId;
                                        JunctionBox model = _mapper.Map<JunctionBox>(createDto);
                                        model.CreatedBy = existingJunctionBox.CreatedBy;
                                        model.CreatedDate = existingJunctionBox.CreatedDate;

                                        validationData.Operation = OperationType.Edit;
                                        validationData.Changes = GetChanges(existingJunctionBox, createDto);

                                        var response = _junctionBoxService.Update(model, existingJunctionBox, userId);
                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateJunctionBoxChangeLog(existingJunctionBox, createDto);
                                    }
                                    else
                                    {
                                        JunctionBox model = _mapper.Map<JunctionBox>(createDto);
                                        validationData.Changes = GetChanges(model, createDto);

                                        var response = await _junctionBoxService.AddAsync(model, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateJunctionBoxChangeLog(new JunctionBox(), createDto);
                                    }
                                }
                                else if (importFileType == FileType.Panel)
                                {
                                    if (recordId != null && existingPanel != null)
                                    {
                                        isUpdate = true;
                                        createDto.Id = existingPanel.Id;
                                        createDto.TagId = existingPanel.TagId;
                                        Panel model = _mapper.Map<Panel>(createDto);
                                        model.CreatedBy = existingPanel.CreatedBy;
                                        model.CreatedDate = existingPanel.CreatedDate;

                                        validationData.Operation = OperationType.Edit;
                                        validationData.Changes = GetChanges(existingPanel, createDto);

                                        var response = _panelService.Update(model, existingPanel, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreatePanelChangeLog(existingPanel, createDto);
                                    }
                                    else
                                    {
                                        Panel model = _mapper.Map<Panel>(createDto);
                                        validationData.Changes = GetChanges(model, createDto);

                                        var response = await _panelService.AddAsync(model, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreatePanelChangeLog(new Panel(), createDto);
                                    }
                                }
                                else if (importFileType == FileType.Skid)
                                {
                                    if (recordId != null && existingSkid != null)
                                    {
                                        isUpdate = true;
                                        createDto.Id = existingSkid.Id;
                                        createDto.TagId = existingSkid.TagId;
                                        Skid model = _mapper.Map<Skid>(createDto);
                                        model.CreatedBy = existingSkid.CreatedBy;
                                        model.CreatedDate = existingSkid.CreatedDate;

                                        validationData.Operation = OperationType.Edit;
                                        validationData.Changes = GetChanges(existingSkid, createDto);

                                        var response = _skidService.Update(model, existingSkid, userId);
                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateSkidChangeLog(existingSkid, createDto);
                                    }
                                    else
                                    {
                                        Skid model = _mapper.Map<Skid>(createDto);
                                        validationData.Changes = GetChanges(model, createDto);

                                        var response = await _skidService.AddAsync(model, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateSkidChangeLog(new Skid(), createDto);
                                    }
                                }
                                else if (importFileType == FileType.Stand)
                                {
                                    if (recordId != null && existingStand != null)
                                    {
                                        isUpdate = true;
                                        createStandDto.Id = existingStand.Id;
                                        createStandDto.TagId = existingStand.TagId;
                                        Stand model = _mapper.Map<Stand>(createStandDto);
                                        model.CreatedBy = existingStand.CreatedBy;
                                        model.CreatedDate = existingStand.CreatedDate;

                                        validationData.Operation = OperationType.Edit;
                                        validationData.Changes = GetChanges(existingStand, createStandDto);

                                        var response = _standService.Update(model, existingStand, userId);
                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateStandChangeLog(existingStand, createStandDto);
                                    }
                                    else
                                    {
                                        Stand model = _mapper.Map<Stand>(createStandDto);
                                        validationData.Changes = GetChanges(model, createStandDto);
                                        var response = await _standService.AddAsync(model, userId);

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", moduleName));
                                        else
                                            await _changeLogHelper.Value.CreateStandChangeLog(new Stand(), createStandDto);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                message.Add((isUpdate ? ResponseMessages.ModuleNotUpdated : ResponseMessages.ModuleNotCreated).ToString().Replace("{module}", moduleName));
                            }
                        }

                        validationData.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                        validationData.Message = string.Join(", ", message);
                        validationDataList.Add(validationData);
                    }
                }
                await _junctionBoxService.RollbackTransaction(transaction);

            }
            catch (Exception ex)
            {
                return new()
                {
                    Message = ex.Message
                };
            }

            if (validationDataList.Count == 0)
            {
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };
            }

            return new()
            {
                IsSucceeded = true,
                Message = ResponseMessages.ImportFile,
                Records = validationDataList
            };
        }

        private List<ChangesDto> GetChanges(JunctionBox entity, CreateOrEditJunctionBoxDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Type),
                    NewValue = createDto.Type,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Type ?? string.Empty : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Description),
                    NewValue = createDto.Description,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }

        private List<ChangesDto> GetChanges(Stand entity, CreateOrEditStandDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Type),
                    NewValue = createDto.Type,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Type ?? string.Empty : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Description),
                    NewValue = createDto.Description,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }

        private List<ChangesDto> GetChanges(Panel entity, CreateOrEditJunctionBoxDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Type),
                    NewValue = createDto.Type,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Type ?? string.Empty : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Description),
                    NewValue = createDto.Description,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }

        private List<ChangesDto> GetChanges(Skid entity, CreateOrEditJunctionBoxDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Type),
                    NewValue = createDto.Type,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Type ?? string.Empty : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Description),
                    NewValue = createDto.Description,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }

        public async Task<List<DropdownInfoDto>> GetProjectWiseTagInfo(Guid projectId, string type, Guid? id, bool AsNoTracking = false)
        {
            List<Tag> allTags = await _tagService.GetAll(s => s.ProjectId == projectId && s.IsActive && !s.IsDeleted, AsNoTracking).ToListAsync();
            List<Stand> allStands = await _standService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted, AsNoTracking).ToListAsync();
            List<Skid> allSkids = await _skidService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && s.IsActive && !s.IsDeleted, AsNoTracking).ToListAsync();
            List<Panel> allPanels = await _panelService.GetAll(s => s.Tag.ProjectId == projectId && !s.IsDeleted, AsNoTracking).ToListAsync();
            List<Device> allDevices = await _deviceService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted, AsNoTracking).ToListAsync();
            List<JunctionBox> allJunctionBox = await _junctionBoxService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted, AsNoTracking).ToListAsync();
            List<DropdownInfoDto> allTagInfo = (from tg in allTags
                                                join st in allStands on tg.Id equals st.TagId into Stand
                                                from st in Stand.DefaultIfEmpty()

                                                join sk in allSkids on tg.Id equals sk.TagId into Skid
                                                from sk in Skid.DefaultIfEmpty()

                                                join pn in allPanels on tg.Id equals pn.TagId into Panel
                                                from pn in Panel.DefaultIfEmpty()

                                                join dv in allDevices on tg.Id equals dv.TagId into Device
                                                from dv in Device.DefaultIfEmpty()

                                                join jb in allJunctionBox on tg.Id equals jb.TagId into Junction
                                                from jb in Junction.DefaultIfEmpty()
                                                where Stand.Count() == 0 && Skid.Count() == 0 && Panel.Count() == 0 && Device.Count() == 0 && Junction.Count() == 0
                                                select new DropdownInfoDto
                                                {
                                                    Id = tg.Id,
                                                    Name = tg.TagName,
                                                }).ToList();

            if (!string.IsNullOrEmpty(type) && id != null && id != Guid.Empty)
            {
                DropdownInfoDto? currentTag = new DropdownInfoDto();
                switch (type)
                {
                    case "Skid":
                        currentTag = allSkids.Where(s => s.Id == id).Select(s => new DropdownInfoDto
                        {
                            Id = s.TagId,
                            Name = s.Tag.TagName,
                        }).FirstOrDefault();
                        break;
                    case "Stand":
                        currentTag = allStands.Where(s => s.Id == id).Select(s => new DropdownInfoDto
                        {
                            Id = s.TagId,
                            Name = s.Tag.TagName,
                        }).FirstOrDefault();
                        break;
                    case "Panel":
                        currentTag = allPanels.Where(s => s.Id == id).Select(s => new DropdownInfoDto
                        {
                            Id = s.TagId,
                            Name = s.Tag.TagName,
                        }).FirstOrDefault();
                        break;
                    case "JunctionBox":
                        currentTag = allJunctionBox.Where(s => s.Id == id).Select(s => new DropdownInfoDto
                        {
                            Id = s.TagId,
                            Name = s.Tag.TagName,
                        }).FirstOrDefault();
                        break;
                    default:
                        currentTag = null;
                        break;
                }

                if (currentTag != null)
                    allTagInfo.Add(currentTag);

            }
            return allTagInfo;
        }

        private static KeyValueInfoDto CreateKeyValue(string key, string value)
        {
            return new KeyValueInfoDto
            {
                Key = key,
                Value = value.ToString()
            };
        }
    }
}
