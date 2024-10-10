using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.Service
{
    public class AttributeValueService : GenericRepository<ICMDDbContext, AttributeValue>, IAttributeValueService
    {
        public AttributeValueService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
