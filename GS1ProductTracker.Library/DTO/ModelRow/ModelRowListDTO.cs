using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ModelRow
{
    public class ModelRowListDTO
    {
        public ModelRowListDTO()
        {
            ModelRowTextual = new List<ModelRowTextualDTO>();
        }

        public long Id { get; set; }
        public string Description { get; set; }
        public string GLN { get; set; }
        public string GTIN { get; set; }
        public string InternalCode { get; set; }
        public string Company { get; set; }
        public bool HasImages {get;set;}
        public bool HasTextual { get; set; }
        public string Error { get; set; }
        public List<ModelRowTextualDTO> ModelRowTextual { get; set; }
        public string ContentFullPath { get; set; }
    }
}
