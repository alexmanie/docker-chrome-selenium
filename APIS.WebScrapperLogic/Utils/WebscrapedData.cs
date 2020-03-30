using System.Collections.Generic;

namespace APIS.WebScrapperLogic.Utils
{
    public class WebScrappedData
    {

        public WebScrappedData()
        {
            IsSuccess = false;
            ScrappedImages = new List<ImageData>();
        }

        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string ProductUrl { get; set; }
        public string ProductRealName { get; set; }

        public WebScrappedTextual ScrappedTextual { get; set; }
        public List<ImageData> ScrappedImages { get; set; }

    }

}

