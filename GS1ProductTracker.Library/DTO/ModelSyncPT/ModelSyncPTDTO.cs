using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ModelSyncPT
{
    public class ModelSyncPTDTO
    {
        public int Id { get; set; }
        public string PublisherGLN { get; set; }
        public string SubscriberGLN { get; set; }
        public bool IsMediaOnly { get; set; }
    }
}
