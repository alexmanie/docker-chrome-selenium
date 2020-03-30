using GS1ProductTracker.Shared.DTO.ModelKeywords;
using System;
using System.Collections.Generic;

namespace GS1ProductTracker.Shared.DTO.Model
{
    public class ModelDetailDTO
    {
        public ModelDetailDTO()
        {
            ModelKeywords = new List<string>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string ModelType { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public bool IsProcessingPaused { get; set; }
        public long? OlderVersionId { get; set; }
        public long? MostRecentVersionId { get; set; }
        public int ModelPeriodicityId { get; set; }
        public string CreatedBy { get; set; }
        public int? KeywordEntityId { get; set; }
        public string KeywordEntityGLN { get; set; }

        public List<string> ModelKeywords { get; set; }

    }
}

