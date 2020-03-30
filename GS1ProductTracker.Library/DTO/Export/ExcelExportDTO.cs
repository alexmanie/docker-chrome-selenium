using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Export
{
    public class ExcelExportDTO
    {
        public Guid? FileBinaryId { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public string Error { get; set; }
        public virtual string ExcelExportType { get; set; }
        public int Id { get; set; }
    }
}
