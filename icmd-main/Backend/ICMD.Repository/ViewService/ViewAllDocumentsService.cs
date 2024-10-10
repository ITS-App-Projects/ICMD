using ICMD.Core.ViewDto;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.ViewService
{
    public class ViewAllDocumentsService : GenericRepository<ICMDDbContext, ViewAllDocumentsDto>
    {
        public ViewAllDocumentsService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
