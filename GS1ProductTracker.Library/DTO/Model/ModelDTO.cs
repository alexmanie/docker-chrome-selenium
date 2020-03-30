using GS1ProductTracker.Shared.DTO.ModelKeywords;
using GS1ProductTracker.Shared.DTO.ModelRow;
using System;
using System.Collections.Generic;

namespace GS1ProductTracker.Shared.DTO.Model
{
    public class ModelDTO
    {
        public ModelDTO()
        {
            ModelRow = new List<ModelRowDTO>();
            ModelKeywords = new List<string>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long ModelTypeId { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int ModelPeriodicityId { get; set; }
        public int? KeywordEntityId { get; set; }

        public List<ModelRowDTO> ModelRow { get; set; }
        public List<string> ModelKeywords { get; set; }
    }
}
