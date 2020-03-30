using System;

namespace GS1ProductTracker.Shared.DTO.ComparisonRow
{
    public class ComparisonRowListDTO
    {
        public long Id { get; set; }

        public long SourceModelRowId { get; set; }
        public string GTIN { get; set; }
        public string SourceGLN { get; set; }
        public string TargetGLN { get; set; }
        public long TargetModelRowId { get; set; }
        public double? ImageComparisonAverage { get; set; }
        public double? ImageComparisonBest { get; set; }
        public double? TextComparisonAverage { get; set; }
        public bool? IsTextEmpty { get; set; }
        public string Error { get; set; }
        public Guid? SourceImageId { get; set; }
        public Guid? TargetImageId { get; set; }
        public string SourceThumbnailBinaryBase64 { get; set; }
        public byte[] SourceThumbnailBinary { get; set; }
        public string TargetThumbnailBinaryBase64 { get; set; }
        public byte[] TargetThumbnailBinary { get; set; }
        public string Sentence { get; set; }
        public string Weigth { get; set; }
        public string SourceDescription { get; set; }
        public double? FirstValidationValue { get; set; }
        public double? SecondValidationValue { get; set; }
        public double? ThirdValidationValue { get; set; }
        public double? ReversedFirstValidationValue { get; set; }
        public double? ReversedSecondValidationValue { get; set; }
        public double? ReversedThirdValidationValue { get; set; }
        public int? FirstValidationMatchingPoints { get; set; }
        public int? SecondValidationMatchingPoints { get; set; }
        public int? ThirdValidationMatchingPoints { get; set; }
        public int? ReversedFirstValidationMatchingPoints { get; set; }
        public int? ReversedSecondValidationMatchingPoints { get; set; }
        public int? ReversedThirdValidationMatchingPoints { get; set; }
        public int? FirstValidationPoints { get; set; }
        public int? SecondValidationPoints { get; set; }
        public int? ThirdValidationPoints { get; set; }
        public int? ReversedSecondValidationPoints { get; set; }
        public int? ReversedFirstValidationPoints { get; set; }
        public int? ReversedThirdValidationPoints { get; set; }



    }
}
