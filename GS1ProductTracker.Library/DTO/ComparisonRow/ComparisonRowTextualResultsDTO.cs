using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ComparisonRow
{
    public class ComparisonRowTextualResultsDTO
    {
        public string Field { get; set; }
        public string SourceText { get; set; }
        public string TargetText { get; set; }
        public double? Result { get; set; }
        public bool IsNutrient { get; set; }

    }
}
