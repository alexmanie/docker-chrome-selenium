using GS1ProductTracker.Shared.DTO.ComparisonRow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Comparison
{
    public class ComparisonDTO
    {
        public ComparisonDTO()
        {
            ComparisonRow = new List<ComparisonRowDTO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long SourceModelId { get; set; }
        public long TargetModelId { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public List<ComparisonRowDTO> ComparisonRow { get; set; }
    }
}
