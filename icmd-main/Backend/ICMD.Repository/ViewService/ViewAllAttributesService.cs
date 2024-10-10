using ICMD.Core.ViewDto;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.ViewService
{
    public class ViewAllAttributesService : GenericRepository<ICMDDbContext, ViewAllAttributesDto>
    {
        public ViewAllAttributesService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
