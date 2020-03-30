using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.Constants
{
    public static class SourceModelExcel
    {
        public const string DESCRIPTION = "SourceDescription";
        public const string GLN = "SourceGLN";
        public const string GTIN = "SourceGTIN";
        public const string INTERNALCODE = "SourceInternalCode";
        public const string CONTACTNAME = "ContactName";
        public const string INGREDIENTSTATMENT = "IngredientStatement";
        public const string MARKETINGMESSAGE = "MarketingMessage";
        public const string ADDRESS = "Address";
        public const string COUNTRYORIGIN = "Country of origin";
        public const string NETCONTENT = "Net Content";
        public const string NETCONTENTVOLUME = "Net Content Volume";
        public const string DISAGGREGATEDNETCONTENT = "Disaggregated Net Content";
        public const string CONSUMERINSTRUCTIONS = "ConsumerInstructions";
        public const string RATIONS = "Number of rations";
        public const string IMAGES = "Images";

        // Nutrients
        public const string NUTRIENT_ENERGY_KJ = "Energy (KJ)";
        public const string NUTRIENT_ENERGY_KCAL = "Energy (KCal)";
        public const string NUTRIENT_FAT = "Fats";
        public const string NUTRIENT_FATSATURATED = "Saturated fatty acids";
        public const string NUTRIENT_SUGAR = "Sugars";
        public const string NUTRIENT_CHOAVL = "Carbohydrates";
        public const string NUTRIENT_FIBER = "Fiber";
        public const string NUTRIENT_SALTEQ = "Salt";
        public const string NUTRIENT_FAMSCIS = "Monounsaturated fatty acids";
        public const string NUTRIENT_FAPUCIS = "Polyunsaturated fatty acids";
        public const string NUTRIENT_PRO = "Proteins";
        public const string NUTRIENT_BIOT = "Biotin";
        public const string NUTRIENT_CA = "Cinnamyl alcohol";
        public const string NUTRIENT_CLD = "Chlorine";
        public const string NUTRIENT_CR = "Chrome";
        public const string NUTRIENT_CU = "Copper";
        public const string NUTRIENT_FD = "Fluorine";
        public const string NUTRIENT_FE = "Iron";
        public const string NUTRIENT_FOLDFE = "Folic Acid";
        public const string NUTRIENT_ID = "Iodine";
        public const string NUTRIENT_K = "Potassium";
        public const string NUTRIENT_MG = "Magnesium";
        public const string NUTRIENT_MN = "Manganese";
        public const string NUTRIENT_MO = "Molybdenum";
        public const string NUTRIENT_NIA = "Niacin";
        public const string NUTRIENT_P = "Phosphorus";
        public const string NUTRIENT_PANTAC = "Pantothenic acid";
        public const string NUTRIENT_POLYL = "Polyols";
        public const string NUTRIENT_RIBF = "Riboflavin";
        public const string NUTRIENT_SE = "Selenium";
        public const string NUTRIENT_STARCH = "Starch";
        public const string NUTRIENT_THIA = "Thiamine";
        public const string NUTRIENT_VITA = "Vitamin A";
        public const string NUTRIENT_VITB12 = "Vitamin B12";
        public const string NUTRIENT_VITB6 = "Vitamin B6";
        public const string NUTRIENT_VITC = "Vitamin C";
        public const string NUTRIENT_VITD = "Vitamin D";
        public const string NUTRIENT_VITE = "Vitamin E";
        public const string NUTRIENT_VITK = "Vitamin K";
        public const string NUTRIENT_ZN = "Zinc";

        public static List<string> GetRequiredHeaders()
        {
            return new List<string>
            {
                GLN,
                GTIN,
            };
        }
    }
}
