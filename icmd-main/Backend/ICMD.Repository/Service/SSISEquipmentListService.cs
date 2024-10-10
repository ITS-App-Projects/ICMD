using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.Service
{
    public class SSISEquipmentListService : GenericRepository<ICMDDbContext, SSISEquipmentList>, ISSISEquipmentListService
    {
        public SSISEquipmentListService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
