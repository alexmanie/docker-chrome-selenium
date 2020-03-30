using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Comparison
{
    public class ComparisonListForAnalysisDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long SourceModelId { get; set; }
        public long TargetModelId { get; set; }
        public string SourceModelName { get; set; }
        public string TargetModelName { get; set; }
        public DateTime CreatedOn { get; set; }
        public double AverageImageMatching { get; set; }
        public double AverageTextMatching { get; set; }

        public double AverageImageMatchingWeight { get; set; }

    }
}
