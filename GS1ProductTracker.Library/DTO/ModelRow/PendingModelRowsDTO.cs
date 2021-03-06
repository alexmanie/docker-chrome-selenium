﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ModelRow
{
    public class PendingModelRowsDTO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string GLN { get; set; }
        public string GTIN { get; set; }
        public string InternalCode { get; set; }
        public string Company { get; set; }
        public string ContentBasePath { get; set; }
        public string ContentFullPath { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public string Error { get; set; }
        public long ModelId { get; set; }
    }
}
