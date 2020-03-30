using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ComparisonRow
{
    public class ComparisonRowTextualDTO
    {
        public long ID { get; set; }

        public long SourceTextID { get; set; }

        public long TargetTextID { get; set; }

        public double? Result { get; set; }

        public DateTime? StartProcessingOn { get; set; }

        public DateTime? EndProcessingOn { get; set; }

        public string Error { get; set; }

        public long ComparisonRowId { get; set; }
    }
}
