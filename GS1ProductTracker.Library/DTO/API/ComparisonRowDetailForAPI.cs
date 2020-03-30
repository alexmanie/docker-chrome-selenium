using GS1ProductTracker.Shared.DTO.ComparisonRow;
using System;
using System.Collections.Generic;

namespace GS1ProductTracker.Shared.DTO.API
{
    public class ComparisonRowDetailForAPI
    {
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

        public List<ComparisonRowImageResultsDTO> ImageResults { get; set; }
        public List<ComparisonRowTextualResultsDTO> TextResults { get; set; }


    }
}
