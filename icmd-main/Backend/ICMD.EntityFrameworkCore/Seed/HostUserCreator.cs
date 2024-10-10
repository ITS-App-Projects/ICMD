using ICMD.Core.Authorization;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Reports;
using ICMD.EntityFrameworkCore.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ICMD.EntityFrameworkCore.Seed
{
    public class HostUserCreator
    {
        private readonly UserManager<ICMDUser> _userManager;
        private readonly RoleManager<ICMDRole> _roleManager;
        private readonly ICMDDbContext _dbContext;

        public HostUserCreator(UserManager<ICMDUser> userManager, RoleManager<ICMDRole> roleManager, ICMDDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task Create()
        {
            await CreateHostRole();
            await CreateHostUser();
        }

        private async Task CreateHostRole()
        {
            Dictionary<string, string> normalizedName = new Dictionary<string, string>()
            {
                {RoleConstants.Administrator,"Administrator" },
                {RoleConstants.User,"User" },
                {RoleConstants.SuperUser,"SuperUser" }
            };


            var oldRole = await _roleManager.Roles.Where(x => x.DisplayName == "Principal").FirstOrDefaultAsync();
            if (oldRole != null)
            {
                var superUser = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == RoleConstants.SuperUser);
                if (superUser == null)
                {
                    string normalized = normalizedName.FirstOrDefault(x => x.Key == RoleConstants.SuperUser).Value;
                    oldRole.Name = RoleConstants.SuperUser;
                    oldRole.NormalizedName = normalized;
                    oldRole.DisplayName = "Super User";
                    oldRole.CreatedDate = DateTime.UtcNow;

                    await _roleManager.UpdateAsync(oldRole);
                }
                else
                {
                    await _roleManager.DeleteAsync(oldRole);
                }
                await _dbContext.SaveChangesAsync();
            }

            var exitRolesList = _roleManager.Roles.Select(x => x.Name).ToList();
            if (exitRolesList.Any())
            {
                var notExist = normalizedName.Keys.Except(exitRolesList);
                foreach (var notRole in notExist)
                {
                    string normalized = normalizedName.FirstOrDefault(x => x.Key == notRole).Value;
                    var roleResult = await _roleManager.CreateAsync(new ICMDRole { Name = notRole, NormalizedName = normalized, DisplayName = normalized, CreatedDate = DateTime.UtcNow });
                }
            }
            else
            {
                foreach (var objRole in normalizedName.Keys)
                {
                    string normalized = normalizedName.FirstOrDefault(x => x.Key == objRole).Value;
                    var roleResult = await _roleManager.CreateAsync(new ICMDRole { Name = objRole, NormalizedName = normalized, DisplayName = normalized, CreatedDate = DateTime.UtcNow });
                }
            }
        }

        private async Task CreateHostUser()
        {
            var systemAdmin = await _userManager.FindByNameAsync("admin");
            if (systemAdmin == null)
            {
                var user = new ICMDUser
                {
                    UserName = "admin",
                    FirstName = "Admin",
                    Email = "admin@gmail.com",
                    NormalizedUserName = "ADMIN",
                    NormalizedEmail = "admin",
                    EmailConfirmed = true,
                    IsActive = true,
                };

                var result = await _userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, RoleConstants.Administrator);
                    //await CreateMenuItems(user.Id);
                    await CreateReportList(user?.Id ?? Guid.Empty);
                }
            }
            else
            {
                await CreateReportList(systemAdmin?.Id ?? Guid.Empty);
            }
            await CreateMenuPermission();
        }

        private async Task CreateReportList(Guid? createdBy)
        {
            List<Report> getReports = await _dbContext.Report.Where(a => a.IsActive && !a.IsDeleted).ToListAsync();
            List<Report> reportList = new List<Report>
             {
                new Report {URL="PSSTags", Group = "PSS", Name = "PSS Tags", Description = null, Order = 0 },
                new Report {URL="PnIDException", Group = "Exception Reports", Name = "P&ID Exception Report", Description = "Tags in Instrument & Control List that don't appear in the P&ID Database.", Order = 0 },
                new Report {URL="PnID_Device_MismatchedDocumentNumber", Group = "Exception Reports", Name = "P&ID Document Reference Exception Report 1", Description = "Compare Instrument / Control's P&ID Document References with Tag's P&ID References and show any mismatch in Document Number.", Order = 1 },
                new Report {URL="PnID_Device_MismatchedDocumentNumber_VersionRevision", Group = "Exception Reports", Name = "P&ID Document Reference Exception Report 2", Description = "Compare Instrument / Control's P&ID Document References with Tag's P&ID References and show any mismatch in Document Number, Version and/or Revision.", Order = 2 },
                new Report {URL="PnID_Device_MismatchedDocumentNumber_VersionRevisionInclNulls", Group = "Exception Reports", Name = "P&ID Document Reference Exception Report 3", Description = "Compare Instrument / Control's P&ID Document References with Tag's P&ID References and show any mismatch in Document Number, Version and/or Revision (including those without Revision and/or Version).", Order = 3 },
                new Report {URL="AuditLog", Group = "Audit Log", Name = "Audit Log", Description = "An audit trail of all system changes.", Order = 0 },
                new Report {URL="UnassociatedTags", Group = "Unassociated", Name = "Unassociated Tags", Description = "Report showing Tags that aren't associated with Devices, Stands, Skids, Panels, Cables, and Junction Boxes.", Order = 0 },
                new Report {URL="UnassociatedSkids", Group = "Unassociated", Name = "Unassociated Skids", Description = "Report showing Skids that aren't used.", Order = 1 },
                new Report {URL="UnassociatedStands", Group = "Unassociated", Name = "Unassociated Stands", Description = "Report showing Stands that aren't used.", Order = 2 },
                new Report {URL="UnassociatedJunctionBoxes", Group = "Unassociated", Name = "Unassociated Junction Boxes", Description = "Report showing Junction Boxes that aren't used.", Order = 3 },
                new Report {URL="UnassociatedPanels", Group = "Unassociated", Name = "Unassociated Panels", Description = "Report showing Panels that aren't used.", Order = 4 },
                new Report {URL="InstrumentList", Group = "Lists", Name = "Instrument List", Description = "Full instrument list.", Order = 0 },
                new Report {URL="NonInstrumentList", Group = "Lists", Name = "Non-Instrument List", Description = "Full list of 'non' instruments.", Order = 1 },
                new Report {URL="NatureOfSignalValidation", Group = "Validation", Name = "Nature of Signal Validation", Description = "List of instruments that fail Nature of Signal validation, wrt Node addressing, Rack / Slot / Channel.", Order = 0 },
                new Report {URL="DuplicateDPNodeAddresses", Group = "Duplicate", Name = "Duplicate DP Node Addresses", Description = "Shows tags connected to DP/DP Couplers that have the same DP Node Address as other tags on the same DP/DP Coupler.", Order = 0 },
                new Report {URL="DuplicatePANodeAddresses", Group = "Duplicate", Name = "Duplicate PA Node Addresses", Description = "Shows tags connected to DP/PA Couplers that have the same PA Node Address as other tags on the same DP/PA Coupler.", Order = 1 },
                new Report {URL="DuplicateRackSlotChannels", Group = "Duplicate", Name = "Duplicate Rack Slot Channels", Description = "Tags that are connected to a Rack (RIO/VMB) over the same Slot and Channel.", Order = 2 },
                new Report {URL="OMItemInstrumentList", Group = "Lists", Name = "O&M Item Instrument List", Description = "Full list of O&M instruments.", Order = 2 },
                new Report {URL="NoOfDPPADevices", Group = "Aggregates", Name = "Number of DP and PA Devices Report", Description = null, Order = 0 },
                new Report {URL="SparesReport", Group = "Spares", Name = "Spares Report", Description = "Summary Spares Report", Order = 0 },
                new Report {URL="SparesReportDetailed", Group = "Spares", Name = "PLC Spare I/O Report 1", Description = "Detailed Spares Report", Order = 1 },
                new Report {URL="SparesReportPLC", Group = "Spares", Name = "PLC Spare I/O report 2", Description = "PLC Spares I/O Report", Order = 2 },
            };

            if (!getReports.Any())
            {
                foreach (var item in reportList)
                {
                    item.CreatedDate = DateTime.UtcNow;
                    item.CreatedBy = createdBy ?? Guid.Empty;
                    await _dbContext.Report.AddAsync(item);
                    _dbContext.SaveChanges();
                }
            }
        }

        private async Task CreateMenuPermission()
        {
            List<ICMDRole> allRoles = await _roleManager.Roles.ToListAsync();
            List<MenuItems> allMenus = await _dbContext.MenuItems.Where(a => a.IsActive && !a.IsDeleted).ToListAsync();

            //For Admin
            Guid adminId = allRoles.Where(a => a.Name == RoleConstants.Administrator).Select(a => a.Id).FirstOrDefault();
            if (!_dbContext.MenuPermission.Any(a => a.RoleId == adminId))
            {
                await AddMenuPermission(allMenus, adminId, adminId, RoleConstants.Administrator);
            }

            //For User
            Guid userId = allRoles.Where(a => a.Name == RoleConstants.User).Select(a => a.Id).FirstOrDefault();
            if (!_dbContext.MenuPermission.Any(a => a.RoleId == userId))
            {
                List<string> notIncludeMenus = new List<string>() { "Manage Users", "Import", "Menu-Role", "Role Management", "Permission", "Permission-Management" };
                allMenus = allMenus.Where(a => !notIncludeMenus.Contains(a.MenuDescription ?? "")).ToList();
                await AddMenuPermission(allMenus, userId, adminId, RoleConstants.User);
            }

            //For User
            Guid principleId = allRoles.Where(a => a.Name == RoleConstants.SuperUser).Select(a => a.Id).FirstOrDefault();
            if (!_dbContext.MenuPermission.Any(a => a.RoleId == principleId))
            {
                List<string> notIncludeMenus = new List<string>() { "Manage Users", "Import", "Menu-Role", "Role Management", "Permission", "Permission-Management" };
                allMenus = allMenus.Where(a => !notIncludeMenus.Contains(a.MenuDescription ?? "")).ToList();
                await AddMenuPermission(allMenus, principleId, adminId, RoleConstants.SuperUser);
            }
        }

        private async Task AddMenuPermission(List<MenuItems> allMenus, Guid roleId, Guid createdBy, string role)
        {
            foreach (var item in allMenus)
            {
                MenuPermission menuPermission = new MenuPermission()
                {
                    MenuId = item.Id,
                    RoleId = roleId,
                    IsGranted = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };
                await _dbContext.MenuPermission.AddAsync(menuPermission);
                await _dbContext.SaveChangesAsync();
            }
            if (role == RoleConstants.Administrator)
            {
                List<MenuPermission> allMenupermission = await _dbContext.MenuPermission.Where(a => a.RoleId == roleId && a.IsActive && !a.IsDeleted).ToListAsync();
                List<Operations> OperationEnums = Enum.GetValues(typeof(Operations)).Cast<Operations>().ToList();
                foreach (var item in allMenupermission)
                {
                    foreach (var operation in OperationEnums)
                    {
                        PermissionManagement permissionManagement = new PermissionManagement()
                        {
                            MenuPermissionId = item.Id,
                            Operation = operation,
                            IsGranted = true,
                            CreatedBy = createdBy,
                            CreatedDate = DateTime.UtcNow,
                        };
                        await _dbContext.PermissionManagement.AddAsync(permissionManagement);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            if (role == RoleConstants.User || role == RoleConstants.SuperUser)
            {
                List<MenuPermission> allMenupermission = await _dbContext.MenuPermission.Where(a => a.RoleId == roleId && a.IsActive && !a.IsDeleted).ToListAsync();
                List<Operations> operations = new List<Operations>() { Operations.Add, Operations.Edit, Operations.Delete, Operations.Download };
                foreach (var item in allMenupermission)
                {
                    foreach (var operation in operations)
                    {
                        if (operation == Operations.Download || (operation != Operations.Download && (item.MenuItems.MenuDescription == "Instrument List" || item.MenuItems.MenuDescription == "Non-Instrument List")))
                        {
                            PermissionManagement permissionManagement = new PermissionManagement()
                            {
                                MenuPermissionId = item.Id,
                                Operation = operation,
                                IsGranted = true,
                                CreatedBy = createdBy,
                                CreatedDate = DateTime.UtcNow,
                            };
                            await _dbContext.PermissionManagement.AddAsync(permissionManagement);
                            await _dbContext.SaveChangesAsync();
                        }

                    }

                }
            }
        }
    }
}
