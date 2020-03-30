using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ModelSyncPT
{
    public class SyncPtSearchRequestResponse
    {
        public string GTIN { get; set; }
        public string Description { get; set; }
        public string ThumbnailURL { get; set; }
        public string PartnerGLN { get; set; }
        public string PartnerName { get; set; }
    }
}
