using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ModelSyncPT
{
    public class SyncPtSearchRequestBody
    {
        public string GTIN { get; set; }
        public string CommercialName { get; set; }
        public string GpcCategoryCode { get; set; }
        public string Brand { get; set; }
        public string UnitDescriptor { get; set; }
        public string ProductClass { get; set; }
        public bool? WithMediaOnly { get; set; }
        public string Ingredients { get; set; }
        public DateTime? ModifiedStartDate { get; set; }
        public DateTime? ModifiedEndDate { get; set; }
        public string PublishedBy { get; set; }
        public string SubscribedBy { get; set; }
        public string Status { get; set; }
        //public int? NumRec { get; set; }
    }
}
