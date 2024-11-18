using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.Service
{
    public class CableSystemHierarchyService : GenericRepository<ICMDDbContext, CableSystemHierarchy>, ICableSystemHierarchyService
    {
        public CableSystemHierarchyService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
