using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Imports
{
    public class ExcelImportForListDTO
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public DateTime UploadedOn { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }
}
