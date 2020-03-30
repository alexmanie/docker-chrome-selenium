using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ComparisonRow
{
    public class ComparisonValidationsDTO
    {
        public string Label { get; set; }
        public int? TotalPoints { get; set; }
        public int? MatchingPoints { get; set; }
        public bool IsBest { get; set; }
        public double? PointsRatioPercentage { get; set; }
        public bool? IsAccepted { get; set; }
        public Guid? BinaryId { get; set; }
        public string BinaryName { get; set; }

    }
}
