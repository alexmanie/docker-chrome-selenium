using APIS.WebScrapperLogic.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using APIS.WebScrapperLogic.Utils;
using APIS.WebScrapperLogic.ImagesMatcher;
using GS1ProductTracker.Shared.Constants;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperApple : IWebScrapper
    {
        private readonly string searckLink = "https://www.apple.com/pt/search/";
        readonly RemoteWebDriver browser;

        public WebScrapperApple(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            return companyGLN == CompanyConstants.CompanyGLNs.APPLE;
        }

        public List<string> Find(string gtin, string internalCode, string description)
        {
            var result = new List<string>();
            //currenly only search by name is available
            if (!string.IsNullOrWhiteSpace(description))
            {
                result = GetSiteNavigationResult(description);
            }

            return result;
        }

        private List<string> GetSiteNavigationResult(string searchKeyword)
        {
            var result = new List<string>();
            browser.Url = searckLink + searchKeyword;
            browser.Navigate();
            Thread.Sleep(3000);
            var resultsFoundNumber = browser.FindElementByCssSelector(".as-search-results-value").Text;

            if (!resultsFoundNumber.Contains("0 resultados encontrados"))
            {
                var totalResultPages = Convert.ToInt32(browser.FindElementByClassName("as-pagination-totalnumbers")?.Text);

                for (var currentPage = 1; currentPage <= totalResultPages; currentPage++)
                {
                    browser.Url = searckLink + searchKeyword + "?page=" + currentPage + "&sel=explore&src=serp";
                    browser.Navigate();

                    var elements = browser.FindElementsByCssSelector(".as-relatedproduct .as-link a");
                    var productsFound = elements.Select(x => x.GetAttribute("href"));

                    productsFound = productsFound.Where(x => x.Contains("specs"));
                    productsFound = productsFound.Distinct();
                    result.AddRange(productsFound);
                }

            }

            return result;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);
            return Webscrape(urlList.FirstOrDefault());

        }

        public List<string> GetTestData()
        {
            throw new NotImplementedException();
        }

        public WebScrappedData Webscrape(string hyperlink)
        {
            var scrapResult = new WebScrappedData
            {
                ProductUrl = hyperlink,
                IsSuccess = true
            };


            try
            {

                browser.Url = hyperlink;
                browser.Navigate();

                //scrap textual data
                scrapResult.ScrappedTextual = ProcessTextualData(browser);


                browser.Url = hyperlink;
                browser.Navigate();

                //scrap Images
                if (WebscraperUtils.IsElementPresent(browser, By.ClassName(".ase-gallery .ase-gallery-scroll")))
                {
                    scrapResult.ScrappedImages = ProcessImages(browser);
                }

            }
            catch (Exception e)
            {
                scrapResult.IsSuccess = false;
            }
            return scrapResult;
        }

        private WebScrappedTextual ProcessTextualData(RemoteWebDriver browser)
        {
            var result = new WebScrappedTextual
            {
                StartProcessingOn = DateTime.Now
            };

            try
            {

                var dimensions = browser.FindElementsByCssSelector("figure .diagram-text");
                var dimensionsA = dimensions[0]?.Text;
                var dimensionsL = dimensions[1]?.Text;
                var dimensionsP = dimensions[2]?.Text;

                result.Dimensions = dimensionsA +" x " + dimensionsL + " x " + dimensionsP;
                result.Processor = browser.FindElementByCssSelector(".techspecs .section-chip li").Text;
                result.Battery = browser.FindElementByCssSelector(".techspecs .section-battery .techspecs-list li").Text;
                //result.RamMemory = browser.FindElementByCssSelector(".techspecs .section-battery .techspecs-list li").Text;
                result.RomMemory = browser.FindElementByCssSelector(".techspecs .section-capacity th").Text;
                result.ScreenSize = browser.FindElementsByCssSelector(".techspecs .section-display li").FirstOrDefault(x=> x.Text.Contains("polegadas")).Text;
                result.ScreenResolution = browser.FindElementsByCssSelector(".techspecs .section-display li").FirstOrDefault(x => x.Text.Contains("Resolução")).Text;
                result.BackCamera = browser.FindElementsByCssSelector(".techspecs .section-camera .techspecs-column").FirstOrDefault(x => x.Text.Contains("MP")).Text;
                result.FrontCamera = browser.FindElementsByCssSelector(".techspecs .section-facetime li").FirstOrDefault(x => x.Text.Contains("MP")).Text;
            }
            catch (Exception e)
            {
                result.Error = e.Message;
            }

            result.EndProcessingOn = DateTime.Now;
            return result;
        }

        private List<ImageData> ProcessImages(RemoteWebDriver browser)
        {
            var result = new List<ImageData>();
            //open image modals

            var images = browser.FindElementsByCssSelector(".ase-gallery-item .as-carousel-image");

            var uriList = images.Select(x => x.GetAttribute("src")).ToList();

            result = BinaryImageHelper.GetImageFromURI(uriList);

            return result;
        }

        private string CleanUnits(string value)
        {
            value = value?.Split(null)[0];
            return value?.Replace(".", ",");
        }
    }
}
