using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.Model
{
    public class CopySourceToTargetDTO
    {
        public long SourceModelId { get; set; }
        public int EntityId { get; set; }
        public string ModelName { get; set; }
    }
}
