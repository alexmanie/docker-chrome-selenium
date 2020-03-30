using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Excel
{
    public class ImportReport
    {
        public string Worksheet { get; set; }
        public string Col { get; set; }
        public int? Row { get; set; }
        public string Error { get; set; }
    }
}
