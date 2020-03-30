using APIS.WebScrapperLogic.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using APIS.WebScrapperLogic.Utils;
using APIS.WebScrapperLogic.ImagesMatcher;
using GS1ProductTracker.Shared.Constants;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperLG : IWebScrapper
    {
        private readonly string searckLink = "https://www.lg.com/pt/search.lg?search=";

        private static readonly string Company_Name = "lg";
        readonly RemoteWebDriver browser;

        public WebScrapperLG(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            return companyGLN == CompanyConstants.CompanyGLNs.LG;
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
            browser.Url = "https://www.lg.com/pt/search.lg?tabType=product&search=" + searchKeyword + "&srchActionURL=/pt/search.lg&majorCategory=2005%2020025%204294967054&sort=Best";
            browser.Navigate();

            var resultsFoundNumber = browser.FindElementByCssSelector(".search-result-area .matching-count strong").Text;

            if (resultsFoundNumber != "0")
            {
                var totalResultPages = 1;
                var lastPageEl = browser.FindElementByCssSelector(".search-pagenation .next.double");

                if(lastPageEl != null)
                {
                    var lastPageLink = lastPageEl.GetAttribute("href");
                    totalResultPages = Convert.ToInt32(lastPageLink.Split('=').Last());
                }

                for (var currentPage = 1; currentPage <= totalResultPages; currentPage++)
                {
                    browser.Url = "https://www.lg.com/pt/search.lg?tabType=product&search=" + searchKeyword + "&srchActionURL=/pt/search.lg&majorCategory=2005%2020025%204294967054&sort=Best&nowPage=" + currentPage;
                    browser.Navigate();

                    var elements = browser.FindElementsByCssSelector(".result-item .type-product .item-text a[data-sc-item='search-results-products']");
                    var productsFound = elements.Select(x => x.GetAttribute("href"));
                    result.AddRange(productsFound.Where(x=> !x.Contains("#")));
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
            browser.Url = hyperlink + "#tech-specs";
            browser.Navigate();

            var scrapResult = new WebScrappedData
            {
                ProductUrl = hyperlink,
                ProductRealName = browser.FindElementByCssSelector(".info-text-top h1")?.Text
            };


            //accept cookies 
            //browser.FindElementByCssSelector(".btns-cookie button.all-cookies-save").Click();

            //scrap textual data
            scrapResult.ScrappedTextual = ProcessTextualData(browser);


            browser.Url = hyperlink;
            browser.Navigate();

            //scrap Images
            if (WebscraperUtils.IsElementPresent(browser, By.ClassName("pdp-improve-gallery-nav")))
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
                result.Brand = "LG";
                result.Description = browser.FindElementByCssSelector(".info-text-top h1")?.Text;

                browser.FindElementByCssSelector(".tech_spec .tech-spec-container .spec_toggle.center a").Click();

                var specs = browser.FindElementsByCssSelector(".tech-spec-container .tech_spec_wrap li");

                var isCameraAgain = false;
                
                foreach (var spec in specs)
                {
                    try
                    {
                        var title = spec.FindElement(By.ClassName("title")).Text;
                        var value = spec.FindElement(By.ClassName("value")).Text;

                        switch (title)
                        {
                            case "Dimensões (A x L x P) (mm)":
                                result.Dimensions = value;
                                break;
                            case "Dimensões :AxLxP (mm)":
                                result.Dimensions = value;
                                break;
                            case "Application Chipset":
                                result.Processor = value;
                                break;
                            case "Processador":
                                result.Processor = value;
                                break;
                            case "Velocidade do Processador":
                                result.Processor += " " + value;
                                break;
                            case "Tipo de Processador":
                                result.Processor += " " + value;
                                break;
                            case "Capacidade (mAh)":
                                result.Battery = value;
                                break;
                            case "Capacidade da Bateria (mAh)":
                                result.Battery = value;
                                break;
                            case "Capacidade RAM (GB)":
                                result.RamMemory = value;
                                break;
                            case "Memória RAM":
                                result.RamMemory = value;
                                break;
                            case "Capacidade ROM (GB)":
                                result.RomMemory = value;
                                break;
                            case "Memória Interna":
                                result.RomMemory = value;
                                break;
                            case "Ecrã (Polegadas)":
                                result.ScreenSize = value;
                                break;
                            case "Ecrã":
                                result.ScreenSize = value;
                                break;
                            case "Resolução":
                                result.ScreenResolution = value;
                                break;
                            case "Resolução e Display":
                                result.ScreenResolution = value;
                                break;
                            case "Resolução (MP)":
                                if (isCameraAgain)
                                {
                                    result.FrontCamera = value;
                                }
                                else
                                {
                                    result.BackCamera = value;
                                    isCameraAgain = true;
                                }
                                break;
                            case "Cor":
                                result.Color = value;
                                break;
                            case "Cores":
                                result.Color = value;
                                break;
                            case "Peso (g)":
                                result.Weigth = value + " g";
                                break;
                            case "Sistema Operativo":
                                result.OperativeSystem = value;
                                break;



                            default: break;
                        }
                    }
                    catch (Exception)
                    {
                      //do nothing
                    }
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
            //open image modals
            browser.FindElementByCssSelector(".pdp-improve-visual-img a").Click();

            var images = browser.FindElementsByCssSelector(".hero-carousel-nav ul a");

            var uriList = images.Select(x => "https://www.lg.com" + x.GetAttribute("data-large-image")).ToList();

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
