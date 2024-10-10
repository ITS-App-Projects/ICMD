﻿using ICMD.Core.ViewDto;
using ICMD.EntityFrameworkCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Repository.ViewService
{
    public class ViewUnassociatedTagsService : GenericRepository<ICMDDbContext, ViewUnassociatedTagsDto>
    {
        public ViewUnassociatedTagsService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
