using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.Service
{
    public class MetaDataService : GenericRepository<ICMDDbContext, MetaData>, IMetaDataService
    {
        public MetaDataService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
