using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ComparisonRow
{
    public class ComparisonRowImageDTO
    {
        public long ID { get; set; }

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

        public double? BestValidationResult { get; set; }

        public string Result { get; set; }

        public int? SourceWidthResized { get; set; }

        public int? SourceHeightResized { get; set; }

        public int? TargetWidthResized { get; set; }

        public int? TargetHeightResized { get; set; }

        public DateTime? StartProcessingOn { get; set; }

        public DateTime? EndProcessingOn { get; set; }

        public string Error { get; set; }

        public long ComparisonRowId { get; set; }

        public Guid? FirstValidationBinaryId { get; set; }
        public Guid? SecondValidationBinaryId { get; set; }
        public Guid? ThirdValidationBinaryId { get; set; }
        public byte[] FirstValidationBinary { get; set; }
        public byte[] SecondValidationBinary { get; set; }
        public byte[] ThirdValidationBinary { get; set; }

        public int? ReversedFirstValidationPoints { get; set; }

        public int? ReversedFirstValidationMatchingPoints { get; set; }

        public double? ReversedFirstValidation { get; set; }

        public int? ReversedSecondValidationPoints { get; set; }

        public int? ReversedSecondValidationMatchingPoints { get; set; }

        public double? ReversedSecondValidation { get; set; }

        public int? ReversedThirdValidationPoints { get; set; }

        public int? ReversedThirdValidationMatchingPoints { get; set; }

        public double? ReversedThirdValidation { get; set; }

        public int? ResultSentenceId { get; set; }

        public Guid? ReversedFirstValidationBinaryId { get; set; }
        public Guid? ReversedSecondValidationBinaryId { get; set; }
        public Guid? ReversedThirdValidationBinaryId { get; set; }
        public byte[] ReversedFirstValidationBinary { get; set; }
        public byte[] ReversedSecondValidationBinary { get; set; }
        public byte[] ReversedThirdValidationBinary { get; set; }


    }
}
