using System;

namespace GS1ProductTracker.Shared.DTO.ModelRow
{
    public class ModelRowTextualDTO
    {
        public long ID { get; set; }
        public long FieldMetaID { get; set; }
        public string Field { get; set; }
        public string Codevalue { get; set; }
        public string Value { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public string Error { get; set; }
        public long ModelRowId { get; set; }
    }
}
