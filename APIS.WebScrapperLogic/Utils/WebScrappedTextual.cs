using System;

namespace APIS.WebScrapperLogic.Utils
{
    public class WebScrappedTextual
    {
        public WebScrappedTextual()
        {
            Nutrients = new NutrientsTextual();
        }

        public string ContactName { get; set; }
        public string IngredientStatement { get; set; }
        public string MarketingMessage { get; set; }
        public NutrientsTextual Nutrients { get; set; }
        public DateTime? StartProcessingOn { get; set; }
        public DateTime? EndProcessingOn { get; set; }
        public string Error { get; set; }
        public string Dimensions { get; set; }
        public string Processor { get; set; }
        public string Battery { get; set; }
        public string RamMemory { get; set; }
        public string RomMemory { get; set; }
        public string ScreenSize { get; set; }
        public string ScreenResolution { get; set; }
        public string BackCamera { get; set; }
        public string FrontCamera { get; set; }
        public string Description { get; set; }
        public string Sizes { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Weigth { get; set; }
        public string OperativeSystem { get; set; }

        public string Address { get; set; }
        public string CountryOfOrigin { get; set; }

        public string NetContent { get; set; }
        public string NetContentVolume { get; set; }
        public string DisaggregateNetContent{ get; set; }

        public string ConsumerUsageStorageInstructions{ get; set; }

        public string Rations { get; set; }

        /// <summary>
        /// Denominacion del alimento
        /// </summary>
        public string RegulatedProductName { get; set; }

        /// <summary>
        /// Cantidad neta escurrida
        /// </summary>
        public string NetContentDrained { get; set; }

        /// <summary>
        /// Modo de empleo
        /// </summary>
        public string PreparationInstructions { get; set; }

    }
}
