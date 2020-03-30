
using GS1ProductTracker.Shared.DTO.Model;
using System.Collections.Generic;

namespace GS1ProductTracker.Shared.DTO.Excel
{
    public class ModelImport
    {
        public ModelImport()
        {
            ImportReport = new List<ImportReport>();
        }

        public ModelDTO ModelToBeCreated { get; set; }
        public List<ImportReport> ImportReport { get; set; }
    }
}
