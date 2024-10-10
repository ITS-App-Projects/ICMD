using ICMD.Core.Dtos.Instrument;
using ICMD.Core.ViewDto;
using ICMD.EntityFrameworkCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Repository.ViewService
{
    public class ViewCountDPPADeviceService : GenericRepository<ICMDDbContext, ViewCountDPPADevicesDto>
    {
        public ViewCountDPPADeviceService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
