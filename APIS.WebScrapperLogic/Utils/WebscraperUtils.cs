using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using APIS.WebScrapperLogic.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace APIS.WebScrapperLogic.Utils
{
    public class WebscraperUtils
    {
        private static List<IWebScrapper> GetWebscrapers()
        {
            List<IWebScrapper> ret = new List<IWebScrapper>()
            {
                //new WebscraperCarrefourES(),
                //new WebscraperMercadonaES(),
                //new WebscraperContinentePT(),
                //new WebscraperAlcampoES(),
                //new WebscraperEroskiES(),
                //new WebscraperDiaES()
            };
            return ret;
        }

        static WebscraperUtils _instance = null;
        public static WebscraperUtils GetInstance()
        {
            if (_instance == null)
            {
                _instance = new WebscraperUtils();
            }
            return _instance;
        }

        protected WebscraperUtils()
        {

        }

        //public static bool CanWebscrape(string hyperlink)
        //{
        //    foreach (var ws in GetWebscrapers())
        //        if (ws.CanWebscrape(hyperlink))
        //            return true;
        //    return false;
        //}

        //public static WebscrapedData Webscrape(string hyperlink,
        //    RemoteWebDriver browser,
        //    //TextWriter debugLog,
        //    bool skipImages)
        //{
        //    foreach (var ws in GetWebscrapers())
        //        if (ws.CanWebscrape(hyperlink))
        //            return ws.Webscrape(hyperlink,
        //                //browser,
        //                //debugLog,
        //                skipImages);
        //    throw new Exception("Bad invoke - GS1ProductTracking.Webscraper doesn't know how to scrape the URI " + hyperlink);
        //}

        public static List<string> GetTestData()
        {
            List<string> ret = new List<string>();

            //foreach (var webscraper in GetWebscrapers())
            //    ret.AddRange(webscraper.GetTestData());

            return ret;
        }


        public static string GetText(IWebElement elem)
        {
            try
            {
                if (elem == null)
                    return null;
                else
                    return elem.Text;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // public static string getTextById(
        //     OpenQA.Selenium.Remote.RemoteWebDriver browser,
        //     string elementId)
        // {
        //     IWebElement elem = null;

        //     try
        //     {
        //         elem = browser.FindElementById(elementId);
        //     }
        //     catch (OpenQA.Selenium.NoSuchElementException)
        //     {
        //         return null;
        //     }
        //     catch (Exception)
        //     {
        //         throw;
        //     }
        //     return elem.Text;
        // }

        public static bool IsElementPresent(IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            // catch (Exception ex)
            // {
            //     Console.ForegroundColor = ConsoleColor.Red;
            //     Console.WriteLine($"[EXCEPTION] {ex.Message}");
            //     Console.ResetColor();

            //     throw;
            // }
        }

        /// <summary>
        /// Find by CSS Selector and return the text
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="selectorString"></param>
        /// <returns></returns>
        public static string GetTextByCssSelector(RemoteWebDriver browser, string selectorString)
        {
            try
            {
                var element = browser.FindElement(By.CssSelector(selectorString));
                return GetTextFromElement(element);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check if element is displayed and return the text
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetTextFromElement(IWebElement element)
        {
            try
            {
                if (element == null) return string.Empty;
                if (element.Displayed)
                    return element.Text.Trim();
                else
                    return element.GetAttribute("textContent").Trim();

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static bool IsVolumeNetContent(string value)
        {
            try
            {
                var regEx = new Regex(@"ml|cm3|l|cl|dl");
                return regEx.Matches(value.ToString()).Count > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
