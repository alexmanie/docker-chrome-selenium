using System;

namespace GS1ProductTracker.Shared.DTO.Model
{
    public class ModelListDTO
    {
        public long Id { get; set; }
        public string ModelName { get; set; }
        public string Type { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public int TotalModelRows { get; set; }
        public int TotalModelRowsToBeProcessed { get; set; }
        public int TotalModelRowsWithError { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsProcessingPaused { get; set; }
        public int ModelPeriodicityId { get; set; }

    }
}
