using ICMD.Core.ViewDto;
using ICMD.EntityFrameworkCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Repository.ViewService
{
    public class ViewPnIDTagExceptionService : GenericRepository<ICMDDbContext, ViewPnIDTagExceptionDto>
    {
        public ViewPnIDTagExceptionService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
