﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.OpenCVVariables
{
    public class OpenCVVariablesDTO
    {
        public int Id { get; set; }
        public string Variable { get; set; }
        public string Description { get; set; }
        public double DefaultValue { get; set; }
        public double CurrentValue { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}
