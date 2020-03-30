using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Comparison
{
    public class ComparisonListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long SourceModelId { get; set; }
        public long TargetModelId { get; set; }
        public string SourceModelName { get; set; }
        public string TargetModelName { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public int TotalComparisonRows { get; set; }
        public int TotalComparisonRowsToBeProcessed { get; set; }
        public int TotalComparisonRowsWithError { get; set; }
        public double ImagesAboveThreshHold { get; set; }
        public bool IsProcessingPaused { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? Status { get; set; }
    }
}
