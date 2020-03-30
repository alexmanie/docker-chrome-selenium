using APIS.WebScrapperLogic.Interfaces;
using APIS.WebScrapperLogic.ImagesMatcher;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using APIS.WebScrapperLogic.Utils;
using GS1ProductTracker.Shared.Constants;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperSanjo : IWebScrapper
    {
        private readonly string searckLink = "https://www.lg.com/pt/search.lg?search=";
        readonly RemoteWebDriver browser;

        public WebScrapperSanjo(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            return companyGLN == CompanyConstants.CompanyGLNs.SANJO;
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
            browser.Url = "http://sanjo.pt/?s=" + searchKeyword + "&post_type=product";
            browser.Navigate();

            var hasResults = WebscraperUtils.IsElementPresent(browser, By.CssSelector(".woocommerce-result-count"));

            if (hasResults)
            {
                var totalResultPages = browser.FindElementsByCssSelector(".gem-pagination .page-numbers").Count();

                for (var currentPage = 1; currentPage <= totalResultPages; currentPage++)
                {
                    browser.Url = "http://sanjo.pt/page/" + currentPage + "/?s=" + searchKeyword + "&post_type=product";
                    browser.Navigate();

                    var elements = browser.FindElementsByCssSelector(".products-list .product-title a");
                    var productsFound = elements.Select(x => x.GetAttribute("href"));
                    result.AddRange(productsFound);
                }
            }

            return result;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);
            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in lg.pt", ProductRealName = description };
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
            browser.Url = hyperlink;
            browser.Navigate();

            var scrapResult = new WebScrappedData
            {
                ProductUrl = hyperlink,
                ProductRealName = browser.FindElementByCssSelector("h3.product_title.light")?.Text
            };

            //scrap textual data
            scrapResult.ScrappedTextual = ProcessTextualData(browser);

            browser.Url = hyperlink;
            browser.Navigate();

            //scrap Images
            if (WebscraperUtils.IsElementPresent(browser, By.ClassName("gem-gallery")))
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

                result.Brand = "Sanjo";
                result.Description = browser.FindElementByCssSelector(".woocommerce-product-details__short-description")?.Text;
                var avaialbleSizes = browser.FindElementsByCssSelector("#pa_tamanho option").Select(x => x.GetAttribute("value"));
                avaialbleSizes = avaialbleSizes.Skip(1); //empty option

                if (avaialbleSizes != null && avaialbleSizes.Any())
                {

                    result.Sizes = string.Join(",", avaialbleSizes);
                }

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

            var images = browser.FindElementsByCssSelector(".gem-gallery-item .gem-gallery-item-image a");

            var uriList = images.Select(x => x.GetAttribute("data-full-image-url")).ToList().Distinct();

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
