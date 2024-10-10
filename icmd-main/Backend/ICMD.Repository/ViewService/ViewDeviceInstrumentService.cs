using ICMD.Core.ViewDto;
using ICMD.EntityFrameworkCore.Database;

namespace ICMD.Repository.ViewService
{
    public class ViewDeviceInstrumentService : GenericRepository<ICMDDbContext, ViewDeviceInstrumentsDto>
    {
        public ViewDeviceInstrumentService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
