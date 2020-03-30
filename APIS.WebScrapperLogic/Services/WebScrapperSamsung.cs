using APIS.WebScrapperLogic.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using APIS.WebScrapperLogic.Utils;
using APIS.WebScrapperLogic.ImagesMatcher;
using GS1ProductTracker.Shared.Constants;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperSamsung : IWebScrapper
    {
        private readonly string searckLink = "https://www.samsung.com/pt/search/?searchvalue=";

        private static readonly string Company_Name = "samsung";
        readonly RemoteWebDriver browser;

        public WebScrapperSamsung(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            return companyGLN == CompanyConstants.CompanyGLNs.SAMSUNG;
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


            var hasResults = WebscraperUtils.IsElementPresent(browser, By.CssSelector(".result-group .group-header[data-omni='products']"));

            if (hasResults)
            {
                browser.FindElementByCssSelector("a[data-omni='view Products']").Click();

                var totalResultPages = 1;
                browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                var lastPageEl = browser.FindElementsByCssSelector(".result-pagination .result-pagination__pages")?.Last()?.Text;
                totalResultPages = Convert.ToInt32(lastPageEl);

                for (var currentPage = 1; currentPage <= totalResultPages + 1; currentPage++)
                {

                    var elements = browser.FindElementsByCssSelector(".product-details .product-details__title a");
                    var productsFound = elements.Select(x => x.GetAttribute("href"));
                    result.AddRange(productsFound.Where(x=> !x.Contains("accessories") && !x.Contains("pen")));

                    //next page
                    var element = browser.FindElement(By.CssSelector(".result-pagination a.btn-arrow.btn-next"));
                    Actions actions = new Actions(browser);
                    actions.MoveToElement(element).Click().Perform();

                }
            }

            return result;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);
            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in samsung.pt", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }

        public List<string> GetTestData()
        {
            throw new NotImplementedException();
        }

        public WebScrappedData Webscrape(string hyperlink)
        {

            var scrapResult = new WebScrappedData
            {
                ProductUrl = hyperlink
            };

            if (!hyperlink.Contains("smartphones"))
            {
                hyperlink = hyperlink.Replace("https://www.samsung.com/pt/", "https://www.samsung.com/pt/smartphones/");
            }

            browser.Url = hyperlink + "specs/";
            browser.Navigate();

            //scrap textual data
            scrapResult.ScrappedTextual = ProcessTextualData(browser);


            browser.Url = hyperlink + "shop/";
            browser.Navigate();

            //scrap Images
            if (WebscraperUtils.IsElementPresent(browser, By.ClassName("slick-gallery")))
            {
                scrapResult.ScrappedImages = ProcessImages(browser);
            }

            scrapResult.IsSuccess = true;
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

                result.Processor = browser.FindElementByCssSelector(".spec-list [data-schmattnm2='F0F7647C-2B0D-E0A0-E043-CBFEDFDDC465'] .detail")?.Text;
                result.Processor += " (" + browser.FindElementByCssSelector(".spec-list [data-schmattnm2='F05FB015-3C04-71BA-E043-CBFEDFDDC462'] .detail")?.Text + ")";
                result.ScreenSize = browser.FindElementByCssSelector(".spec-list [data-schmattnm2='PSA-FD16F5D2-FB47-403A-82CA-45979D61B509'] .detail")?.Text;
                result.ScreenResolution = browser.FindElementByCssSelector(".spec-list [data-schmattnm2='F0F7647C-2AF7-E0A0-E043-CBFEDFDDC465'] .detail")?.Text;
                result.BackCamera = browser.FindElementByCssSelector(".spec-list [data-schmattnm2='F0F7647C-2AEB-E0A0-E043-CBFEDFDDC465'] .detail")?.Text;
                result.FrontCamera = browser.FindElementByCssSelector(".spec-list [data-schmattnm2='F0F7647C-2AE8-E0A0-E043-CBFEDFDDC465'] .detail")?.Text;
                result.RamMemory = browser.FindElementByCssSelector(".spec-list [data-schmattnm2='PSA-8DAB0D67-25D8-43C9-B687-78064BC28E2D'] .detail")?.Text;
                result.RomMemory = browser.FindElementByCssSelector(".spec-list [data-schmattnm2='F0F7647C-2B05-E0A0-E043-CBFEDFDDC465'] .detail")?.Text;
                result.Dimensions = browser.FindElementByCssSelector(".spec-list [data-schmattnm2='PSA-90A9A78C-8B3B-4FF0-A4F8-B0CBE2EFC645'] .detail")?.Text;



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

            var images = browser.FindElementsByCssSelector(".slick-gallery .slick-list .slick-gallery-item img");

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
