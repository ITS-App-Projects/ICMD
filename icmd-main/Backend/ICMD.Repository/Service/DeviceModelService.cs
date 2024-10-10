using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Repository.Service
{
    public class DeviceModelService : GenericRepository<ICMDDbContext, DeviceModel>, IDeviceModelService
    {
        public DeviceModelService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
