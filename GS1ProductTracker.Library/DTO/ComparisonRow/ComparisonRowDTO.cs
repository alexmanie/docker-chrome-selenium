using GS1ProductTracker.Shared.DTO.ModelRow;
using System;
using System.Collections.Generic;

namespace GS1ProductTracker.Shared.DTO.ComparisonRow
{
    public class ComparisonRowDTO
    {
        public ComparisonRowDTO()
        {
            //ComparisonRowImage = new HashSet<ComparisonRowImage>();
        }

        public long Id { get; set; }

        public long SourceModelRowId { get; set; }

        public long TargetModelRowId { get; set; }

        public string SourceModelRow { get; set; }

        public string TargetModelRow { get; set; }

        public DateTime? StartProcessingOn { get; set; }

        public DateTime? EndProcessingOn { get; set; }

        public string Error { get; set; }

        public long ComparisonId { get; set; }
        public string ComparisonName { get; set; }
        public string GTIN { get; set; }

        //public virtual ICollection<ComparisonRowImage> ComparisonRowImage { get; set; }
    }
}
