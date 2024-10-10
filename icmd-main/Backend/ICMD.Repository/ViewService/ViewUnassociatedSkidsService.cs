using ICMD.Core.ViewDto;
using ICMD.EntityFrameworkCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Repository.ViewService
{
    public class ViewUnassociatedSkidsService : GenericRepository<ICMDDbContext, ViewUnassociatedSkidsDto>
    {
        public ViewUnassociatedSkidsService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
