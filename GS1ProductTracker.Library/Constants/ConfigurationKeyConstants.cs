using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS1ProductTracker.Shared.Constants
{
    public static class ConfigurationKeyConstants
    {
        public const string STORAGEACCOUNT_CONNSTRING = "StorageAccountConnString";
        public const string BLOB_CONTAINER_NAME = "images";
        public const string APP_URL = "ApplicationURL";
        public const string EXCELCHUNKSIZE = "ExcelChunkSize";
        public const string COMPARISONCHUNKSIZE = "ComparisonChunkSize";
        public const string NUMBEROFTHREADS = "NumThreads";

    }
}
