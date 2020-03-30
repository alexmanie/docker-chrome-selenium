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
    public class WebScrapperElCorteInglesPT : IWebScrapper
    {
        private readonly string searckLink = "https://www.elcorteingles.pt/supermercado/pesquisar/?term=";

        private static readonly string Company_Name = "elcorteingles";
        readonly RemoteWebDriver browser;

        public WebScrapperElCorteInglesPT(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.ELCORTEINGLES_PT;

            if (!string.IsNullOrWhiteSpace(productURL))
            {
                var linkLower = productURL.ToLower();
                result = linkLower.Contains(Company_Name);
            }

            return result;
        }

        public List<string> Find(string gtin, string internalCode, string description)
        {
            var result = new List<string>();
            //currenly only search by name is available
            if (!string.IsNullOrWhiteSpace(description))
            {
                result = GetSiteNavigationResult(description, gtin);
            }

            return result;
        }

        private List<string> GetSiteNavigationResult(string searchKeyword, string gtin, bool isRecursion = false)
        {
            var result = new List<string>();
            browser.Url = searckLink + searchKeyword;
            browser.Navigate();

            var resultsFoundNumber = browser.FindElementByCssSelector(".grid-coincidences .semi").Text.Split(null)[0];

            if (resultsFoundNumber == "0" && !isRecursion)
            {
                //try to search by partial keyword
                var searchKeywordSplitted = searchKeyword.Split(null);
                if (searchKeywordSplitted.Length >= 5)
                {
                    return GetSiteNavigationResult(searchKeywordSplitted[0] + " " + searchKeywordSplitted[1] + " " + searchKeywordSplitted[2], gtin, true);
                }
                else if (searchKeywordSplitted.Length >= 2)
                {
                    return GetSiteNavigationResult(searchKeywordSplitted[0] + " " + searchKeywordSplitted[1], gtin, true);
                }
            }
            else if (resultsFoundNumber != "0")
            {
                var resultsFoundLink = browser.FindElementsByCssSelector(".grid.c12 h3.product_tile-description a").Select(x => x.GetAttribute("href")).ToList();
                //var resultsFound = browser.FindElementsByCssSelector(".grid.c12 h3.product_tile-description a");

                foreach (var link in resultsFoundLink)
                {
                    browser.Url = link;
                    browser.Navigate();

                    var productEAN = browser.FindElementsByCssSelector(".reference-container.pdp-reference span").FirstOrDefault();

                    if (productEAN != null && gtin.Contains(productEAN.Text))
                    {
                        result.Add(link);
                    }
                }
            }

            return result;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);
            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in ElCorteIngles.pt", ProductRealName = description };
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

            browser.Url = hyperlink;
            browser.Navigate();

            //scrap textual data
            scrapResult.ScrappedTextual = ProcessTextualData(browser);

            //scrap Images
            if (WebscraperUtils.IsElementPresent(browser, By.Id("product-image-placer")))
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
                //Ingredients
                var ingredientsTitle = browser.FindElementsByCssSelector("h3.info-title").FirstOrDefault(x => x.Text == "Ingredientes e alergénios");
                if (ingredientsTitle != null)
                {
                    var parentDiv = ingredientsTitle.FindElement(By.XPath(".."));
                    result.IngredientStatement = parentDiv.FindElement(By.CssSelector("ul.info-list .info-item"))?.Text;
                }


                //ContactName
                var generalInformationTitle = browser.FindElementsByCssSelector("h3.info-title").FirstOrDefault(x => x.Text == "Informação geral");
                if (generalInformationTitle != null)
                {
                    var parentDiv = generalInformationTitle.FindElement(By.XPath(".."));
                    var childs = parentDiv.FindElements(By.CssSelector("ul.info-list .info-item"));

                    if (childs != null && childs.Any())
                    {
                        result.ContactName = childs.FirstOrDefault(x => x.Text.Contains("Nome do fornecedor:"))?.Text.Replace("Nome do fornecedor: ", "");
                    }
                }

                //Nutrients
                var NutrientTables = browser.FindElementsByCssSelector("._nutrients .table-row");

                if (NutrientTables != null)
                {

                    foreach (var nutrient in NutrientTables)
                    {
                        var nutrienteSplitted = nutrient.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        var nutrientName = nutrienteSplitted.Length >= 1 ? nutrienteSplitted[0] : null;
                        var nutrientValue = nutrienteSplitted.Length >= 2 ? nutrienteSplitted[1] : null;
                        var nutrientValueWithoutUnits = CleanUnits(nutrientValue);

                        switch (nutrientName)
                        {
                            case "Valor energético":
                                if (nutrientValue.Contains("kcal"))
                                {
                                    result.Nutrients.EnergyKCal = nutrientValueWithoutUnits;
                                }
                                else
                                {
                                    result.Nutrients.EnergyKJ = nutrientValueWithoutUnits;
                                }
                                break;
                            case "Ácidos gordos saturados":
                                result.Nutrients.FatSaturated = nutrientValueWithoutUnits;
                                break;
                            case "Gorduras":
                                result.Nutrients.Fat = nutrientValueWithoutUnits;
                                break;
                            case "Sal":
                                result.Nutrients.Salt = nutrientValueWithoutUnits;
                                break;
                            case "Açúcares":
                                result.Nutrients.Sugar = nutrientValueWithoutUnits;
                                break;
                            case "Fibra alimentar":
                                result.Nutrients.Fibre = nutrientValueWithoutUnits;
                                break;
                            case "Proteínas":
                                result.Nutrients.Protein = nutrientValueWithoutUnits;
                                break;
                            case "Hidratos de carbono":
                                result.Nutrients.CarboHydrates = nutrientValueWithoutUnits;
                                break;
                            default:
                                break;
                        }
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
            var images = browser.FindElementsById("product-image-placer");

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
