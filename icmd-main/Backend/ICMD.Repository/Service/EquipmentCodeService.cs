﻿using ICMD.Core.DBModels;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Repository.Service
{
    public class EquipmentCodeService : GenericRepository<ICMDDbContext, EquipmentCode>, IEquipmentCodeService
    {
        public EquipmentCodeService(ICMDDbContext dbContext) : base(dbContext) { }
    }
}
