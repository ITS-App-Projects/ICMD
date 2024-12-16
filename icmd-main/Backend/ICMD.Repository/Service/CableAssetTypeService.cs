using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.Service
{
    public class CableAssetTypeService : GenericRepository<ICMDDbContext, CableAssetType>, ICableAssetTypeService
    {
        public CableAssetTypeService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
