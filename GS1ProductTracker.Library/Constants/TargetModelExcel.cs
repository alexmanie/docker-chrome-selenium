using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.Constants
{
    public static class TargetModelExcel
    {
        public const string TARGET_DESCRIPTION = "TargetDescription";
        public const string TARGET_GLN = "TargetGLN";
        public const string TARGET_GTIN = "TargetGTIN";
        public const string TARGET_INTERNALCODE = "TargetInternalCode";
        public const string TARGET_CONTENTFULLPATH = "TargetProductUrl";


        public static List<string> GetRequiredHeaders()
        {
            return new List<string>
            {
                TARGET_GLN,
                TARGET_GTIN,
            };
        }
    }
}
