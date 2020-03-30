using GS1ProductTracker.Shared.DTO.ImageComparison;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ComparisonRow
{
    public class ComparisonRowImageResultsDTO
    {

        public Guid? SourceBinaryDataID { get; set; }
        public Guid? TargetBinaryDataID { get; set; }
        public int? FirstValidationPoints { get; set; }
        public int? FirstValidationMatchingPoints { get; set; }
        public double? FirstValidation { get; set; }
        public int? SecondValidationPoints { get; set; }
        public int? SecondValidationMatchingPoints { get; set; }
        public double? SecondValidation { get; set; }
        public int? ThirdValidationPoints { get; set; }
        public int? ThirdValidationMatchingPoints { get; set; }
        public double? ThirdValidation { get; set; }
        public string Result => ImageComparisonResultSentences?.Sentence;
        public int? Weight => ImageComparisonResultSentences?.Weight;
        public string SourceBinaryBase64 { get; set; }
        public string TargetBinaryBase64 { get; set; }
        public Guid? FirstValidationBinaryId { get; set; }
        public Guid? SecondValidationBinaryId { get; set; }
        public Guid? ThirdValidationBinaryId { get; set; }

        public int? ReversedFirstValidationPoints { get; set; }
        public int? ReversedFirstValidationMatchingPoints { get; set; }
        public double? ReversedFirstValidation { get; set; }
        public int? ReversedSecondValidationPoints { get; set; }
        public int? ReversedSecondValidationMatchingPoints { get; set; }
        public double? ReversedSecondValidation { get; set; }
        public int? ReversedThirdValidationPoints { get; set; }
        public int? ReversedThirdValidationMatchingPoints { get; set; }
        public double? ReversedThirdValidation { get; set; }

        public Guid? ReversedFirstValidationBinaryId { get; set; }
        public Guid? ReversedSecondValidationBinaryId { get; set; }
        public Guid? ReversedThirdValidationBinaryId { get; set; }

        public double? BestValidationResult { get; set; }

        public ImageComparisonResultSentencesDTO ImageComparisonResultSentences  { get;set;}
}
}
