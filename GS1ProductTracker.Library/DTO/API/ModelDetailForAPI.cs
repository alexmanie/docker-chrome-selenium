using GS1ProductTracker.Shared.DTO.ModelRow;
using System;
using System.Collections.Generic;

namespace GS1ProductTracker.Shared.DTO.API
{
    public class ModelDetailForAPI
    {
        public string Name { get; set; }
        public string ModelType { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public List<ModelRowListDTO> ModelRows { get; set; }

    }
}
