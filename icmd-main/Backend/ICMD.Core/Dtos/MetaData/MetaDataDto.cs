﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.MetaData
{
    public class MetaDataDto
    {
        public Guid Id { get; set; }

        public string Property { get; set; }

        public string? Value { get; set; }

        public bool IsDefault { get; set; }
    }
}
