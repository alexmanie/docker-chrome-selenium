using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.Constants
{
    public static class TextComparisonConstants
    {
        public enum Algorithm
        {
            Jacord, 
            Levenshtein,
            CustomNumeric //Algoritmo propio
        }
    }
}
