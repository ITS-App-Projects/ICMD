using ICMD.Core.Authorization;
using ICMD.EntityFrameworkCore.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ICMD.EntityFrameworkCore.Seed
{
    public class InitialHostDbBuilder
    {
        private readonly UserManager<ICMDUser> _userManager;
        private readonly RoleManager<ICMDRole> _roleManager;
        private readonly ICMDDbContext _dbContext;
        public InitialHostDbBuilder(UserManager<ICMDUser> userManager, RoleManager<ICMDRole> roleManager, ICMDDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task Create()
        {
            await new HostUserCreator(_userManager, _roleManager, _dbContext).Create();
        }
    }
}
