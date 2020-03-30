using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.DTO.ImageComparison
{
    public class ImageComparisonThresholdsDTO
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int RejectionMaxValue { get; set; }
        public int AcceptanceMinValue { get; set; }
    }
}
