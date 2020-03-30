using GS1ProductTracker.Shared.DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Excel
{
    public class EntityImport
    {
        public EntityImport()
        {
            ImportReport = new List<ImportReport>();
            EntitiesToBeCreated = new List<EntitiesDTO>();
            EntitiesToBeUpdated = new List<EntitiesDTO>();
        }

        public List<EntitiesDTO> EntitiesToBeCreated { get; set; }
        public List<EntitiesDTO> EntitiesToBeUpdated { get; set; }

        public List<ImportReport> ImportReport { get; set; }
    }
}
