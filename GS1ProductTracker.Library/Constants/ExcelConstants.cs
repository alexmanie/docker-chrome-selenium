using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.Constants
{
    public static class ExcelConstants
    {
        public const string EXCEL_TYPE_SOURCE = "SOURCE";
        public const string EXCEL_TYPE_TARGET = "TARGET";
        public const string EXCEL_TYPE_MODELROWS = "MODELROWS";
        public const string EXCEL_TYPE_COMPANIES = "COMPANIES";
        public const string EXCEL_TYPE_MODEL_SYNCPT = "MODEL_SYNCPT";
        public const string EXCEL_MODELROWS_UPDATE = "MODELROWS_UPDATE";


        public const string EXCEL_STATUS_PENDING = "PENDING";
        public const string EXCEL_STATUS_PROCESSING = "PROCESSING";
        public const string EXCEL_STATUS_SUCCESS = "SUCCESS";
        public const string EXCEL_STATUS_ERROR = "ERROR";

        public const string EXCEL_EXPORT_TYPE_COMPARISON_ROWS = "COMPARISON_ROWS";
        public const string EXCEL_EXPORT_TYPE_COMPARISON_ROWS_WITH_IMAGES = "COMPARISON_ROWS_WITH_IMAGES";
        public const string EXCEL_EXPORT_TYPE_MODEL_ROWS = "MODEL_ROWS";



    }
}
