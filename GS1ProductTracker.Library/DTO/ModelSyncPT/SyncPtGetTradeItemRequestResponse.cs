using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ModelSyncPT
{
    public class SyncPtGetTradeItemRequestResponse
    {
        public SyncPtGetTradeItemRequestResponse()
        {
            NutrientHeaders = new List<NutrientHeader>();
        }

        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactType { get; set; }
        public string IngredientStatement { get; set; }
        public string RegulatedProductName { get; set; }

        public List<NutrientHeader> NutrientHeaders { get; set; }
    }

    public class NutrientHeader
    {
        public NutrientHeader()
        {
            Nutrients = new List<Nutrients>();
        }

        public string PreparationType { get; set; }
        public string DailyValueIntakeReference { get; set; }
        public string ServingSizeDescription { get; set; }
        public decimal? ServingSizeValue { get; set; }
        public string ServingSizeUnitOfMeasure { get; set; }
        public List<Nutrients> Nutrients { get; set; }
    }

    public class Nutrients
    {
        public string Nutrient { get; set; }
        public decimal? Quantity { get; set; }
        public string QuantityUnitOfMeasure { get; set; }

    }
}
