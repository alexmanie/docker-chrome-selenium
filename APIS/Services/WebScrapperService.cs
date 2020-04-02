using APIS.WebScrapperLogic.Interfaces;
using APIS.WebScrapperLogic.Services;
using APIS.WebScrapperLogic.Utils;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace APIS.Services
{
    public class WebScrapperService
    {
        static ChromeDriver _browser;
        
        static ChromeDriver browser
        {
            get 
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--no-sandbox");
                options.AddArgument("--window-size=1420,1080");
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
                options.LeaveBrowserRunning = false;

                if (_browser == null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("ChromeDriver created!");
                    Console.ResetColor();

                    _browser = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options, TimeSpan.FromSeconds(60));
                }

                return _browser;
            }
        }
        
        /// <summary>
        /// Comprobación de si se puede hacer scrapping de la web
        /// </summary>
        /// <param name="gln"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static WebScrappedData WebScrappe(string gln, string path, string gtin, string internalCode, string description)
        {
            try
            {
                // using (var browser = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options, TimeSpan.FromSeconds(60)))
                // {
                    List<IWebScrapper> scrappers = new List<IWebScrapper>()
                    {
                        new WebScrapperDia(browser),
                        //new WebScrapperLearn(browser),
                    };

                    foreach (var scrapper in scrappers)
                    {
                        if (scrapper.CanWebscrape(gln, path))
                        {
                            WebScrappedData scrapResult = default;

                            if (path != null)
                            {
                                scrapResult = scrapper.Webscrape(path);
                            }
                            else
                            {
                                scrapResult = scrapper.FindAndWebscrape(gtin, internalCode, description);
                            }

                            return scrapResult;
                        }
                    }

                    // browser.Quit();

                    // Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    // Console.WriteLine("Quit browser");
                    // Console.ResetColor();
                // }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Exit browser");
                Console.ResetColor();
            }
        }
    }
}
