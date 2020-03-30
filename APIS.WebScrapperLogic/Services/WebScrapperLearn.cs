using APIS.WebScrapperLogic.Interfaces;
using APIS.WebScrapperLogic.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperLearn : IWebScrapper
    {
        RemoteWebDriver browser;

        public WebScrapperLearn(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public static string Company_Name = "Learn";

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            return true;
        }

        public WebScrappedData Webscrape(string hyperlink)
        {
            WebScrappedData ret = new WebScrappedData
            {
                ProductUrl = hyperlink,
                ScrappedTextual = new WebScrappedTextual()
            };

            browser.Url = hyperlink;

            INavigation navigation = browser.Navigate();

            var levelData = string.Empty;
            var levelStatusPointsData = string.Empty;
            var pointsData = string.Empty;
            var nameData = string.Empty;

            // Level
            string level = "//span[@id='level-status-text']";
            if (WebscraperUtils.IsElementPresent(browser, By.XPath(level)))
            {
                var levelElement = browser.FindElements(By.XPath(level)).FirstOrDefault();
                levelData = WebscraperUtils.GetTextFromElement(levelElement);
            }

            // Level status points
            string levelStatusPoints = "//span[@id='level-status-points']";
            if (WebscraperUtils.IsElementPresent(browser, By.XPath(levelStatusPoints)))
            {
                var levelStatusPointsElement = browser.FindElements(By.XPath(levelStatusPoints)).FirstOrDefault();
                levelStatusPointsData = WebscraperUtils.GetTextFromElement(levelStatusPointsElement);
            }

            // Points
            string points = "//span[@id='level-status-points']/span[@class='has-text-weight-semibold']";
            if (WebscraperUtils.IsElementPresent(browser, By.XPath(points)))
            {
                var pointsElement = browser.FindElements(By.XPath(points)).FirstOrDefault();
                pointsData = WebscraperUtils.GetTextFromElement(pointsElement);
            }

            // Name
            string name = "//h1[@class='title has-margin-top-small has-margin-bottom-extra-small']";
            if (WebscraperUtils.IsElementPresent(browser, By.XPath(name)))
            {
                var nameElement = browser.FindElements(By.XPath(name)).FirstOrDefault();
                nameData = WebscraperUtils.GetTextFromElement(nameElement);
            }

            ret.IsSuccess = true;
            ret.ErrorMessage = null;

            return ret;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);
            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }

        public List<string> Find(string gtin, string internalCode, string description)
        {
            return null;
        }

        private List<string> GetSiteNavigationResult(string searchKeyword)
        {
            return null;
        }

        private string CleanGtinFromLeadingZeroes(string gtin)
        {
            return gtin.TrimStart(new Char[] { '0' });
        }

        private string GetNutrientInfo(WebScrappedData ret, string infoBasePath, string nutrientName)
        {
            try
            {
                string nutrient = string.Empty;

                string nutrientXpath = string.Format(infoBasePath, nutrientName);
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(nutrientXpath)))
                {
                    var nutrientElem = browser.FindElements(By.XPath(nutrientXpath)).FirstOrDefault();
                    if (nutrientElem != null)
                    {
                        nutrient = WebscraperUtils.GetTextFromElement(nutrientElem);
                    }
                }

                return nutrient;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
