using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.Service
{
    public class CableHierarchyService : GenericRepository<ICMDDbContext, CableHierarchy>, ICableHierarchyService
    {
        public CableHierarchyService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
