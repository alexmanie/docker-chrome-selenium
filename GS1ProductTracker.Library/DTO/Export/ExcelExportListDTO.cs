using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Export
{
    public class ExcelExportListDTO
    {
        public long Id { get; set; }
        public Guid? FileBinaryId { get; set; }
        public string Filename { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Error { get; set; }
        public virtual string ExcelExportType { get; set; }
        public int NumFiles { get; set; }
    }
}
