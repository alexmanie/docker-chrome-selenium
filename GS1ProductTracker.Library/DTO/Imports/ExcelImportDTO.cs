using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Imports
{
    public class ExcelImportDTO
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public long? ModelId { get; set; }
        public int? Model_SYNCPTId { get; set; }
        public string Filename { get; set; }
        public int ExcelImportTypeId { get; set; }
        public int ExcelImportStatusId { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public Guid? Filebinary { get; set; }
    }
}
