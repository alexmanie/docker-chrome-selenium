using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.Constants
{
    public static class ModelConstants
    {
        public static readonly string MODEL_TYPE_SOURCE = "SOURCE";
        public static readonly string MODEL_TYPE_SOURCE_SYNCPT = "SOURCE_SYNCPT";
        public static readonly string MODEL_TYPE_TARGET = "TARGET";
        public static readonly string MODEL_TYPE_TARGET_KEYWORDS = "TARGET_KEYWORDS";

        public static readonly string MODEL_TYPE_COMPARISON = "COMPARISON";
        public static readonly string MODEL_STATUS_PENDING = "PENDING";
        public static readonly string MODEL_STATUS_PROCESSING = "PROCESSING";
        public static readonly string MODEL_STATUS_PROCESSING_PAUSED = "PROCESSING PAUSED";
        public static readonly string MODEL_STATUS_PROCESSED = "PROCESSED";

        public static readonly string MODEL_PERIODICITY_WEEKLY = "WEEKLY";
        public static readonly string MODEL_PERIODICITY_MONTHLY = "MONTHLY";
        public static readonly string MODEL_PERIODICITY_QUARTERLY = "QUARTERLY";
        public static readonly string MODEL_PERIODICITY_SEMIANNUALY = "SEMIANNUALY";
        public static readonly string MODEL_PERIODICITY_ANNUALY = "ANNUALY";




    }
}
