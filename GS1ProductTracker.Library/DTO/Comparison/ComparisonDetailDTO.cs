namespace GS1ProductTracker.Shared.DTO.Comparison
{
    public class ComparisonDetailDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long SourceModelId { get; set; }
        public string SourceModel { get; set; }
        public long TargetModelId { get; set; }
        public string TargetModel{ get; set; }
        public bool IsProcessingPaused { get; set; }
        public long? OlderVersionId { get; set; }
        public long? MostRecentVersionId { get; set; }
    }
}
