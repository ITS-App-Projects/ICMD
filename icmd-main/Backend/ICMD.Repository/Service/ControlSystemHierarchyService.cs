using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.Service
{
    public class ControlSystemHierarchyService : GenericRepository<ICMDDbContext, ControlSystemHierarchy>, IControlSystemHierarchyService
    {
        public ControlSystemHierarchyService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
