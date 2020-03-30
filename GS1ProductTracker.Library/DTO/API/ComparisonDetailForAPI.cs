using GS1ProductTracker.Shared.DTO.ComparisonRow;
using System.Collections.Generic;

namespace GS1ProductTracker.Shared.DTO.API
{
    public class ComparisonDetailForAPI
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long SourceModelId { get; set; }
        public string SourceModel { get; set; }
        public long TargetModelId { get; set; }
        public string TargetModel { get; set; }
        public bool IsProcessingPaused { get; set; }
        public List<ComparisonRowListDTO> ComparisonRows { get; set; }
             
    }
}
