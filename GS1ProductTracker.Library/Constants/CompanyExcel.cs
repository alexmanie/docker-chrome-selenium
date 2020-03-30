using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.Constants
{
    public class EntityExcel
    {
        public const string ENTITY_NAME = "Empresa";
        public const string ENTITY_GLN = "Código";


        public static List<string> GetRequiredHeaders()
        {
            return new List<string>
            {
                ENTITY_NAME,
                ENTITY_GLN,
            };
        }
    }
}
