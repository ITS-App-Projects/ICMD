using ICMD.Core.ViewDto;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.ViewService
{
    public class TagViewService : GenericRepository<ICMDDbContext, ViewTagDto>
    {
        public TagViewService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
