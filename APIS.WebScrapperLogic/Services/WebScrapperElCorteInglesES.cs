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
    public class WebScrapperElCorteInglesES : IWebScrapper
    {
        private readonly string ROOTWEBSITE = "https://www.elcorteingles.es";

        //private readonly string searckLink = "https://www.elcorteingles.es/supermercado/pesquisar/?term=";
        private readonly string searchLink = "https://www.elcorteingles.es/search/?s=";
        private readonly string searchLinkSM = "https://www.elcorteingles.es/supermercado/buscar/?term=";
        private readonly string searchLinkCG = "https://www.elcorteingles.es/tienda-club-del-gourmet/buscar/?term=";
        private readonly string googleUrl = "https://www.google.com/search?q=";

        private readonly string linkSuper, linkGourmet, linkPets;

        private static readonly string Company_Name = "elcorteingles";
        private static readonly string CNToSearch = "elcorteingles";

        #region [ HTML - CONSTS ]

        private const string TITLE_GENERAL_INFO = "Información general";
        private const string TITLE_ING_ALERG = "Ingredientes y alérgenos";
        private const string TITLE_CONTACT_NAME = "Nombre del operador";
        private const string TITLE_ADDRESS = "Dirección del operador";
        private const string TITLE_COUNTRYOFORIGIN = "País de origen";
        private const string TITLE_NETCONTENT = "Cantidad Neta";
        private const string TITLE_RATIONS = "Número de raciones por envase";
        private const string TITLE_CONSUMERUSAGESTORAGE = "Conservación y utilización";
        private const string TITLE_CONSUMERSTORAGE = "Conservación";
        private const string TITLE_REGULATED_PRODUCT_NAME = "Denominación";
        private const string TITLE_PREPARATION_INSTRUCTIONS = "Modo de empleo";


        private const string TABLE_CALORIES = "Valor energético";
        private const string TABLE_FAT = "Grasas";
        private const string TABLE_FAT_SATURATED = "saturadas";
        private const string TABLE_FAT_MONOUNSATURATED = "monoinsaturadas";
        private const string TABLE_FAT_POLIUNSATURATED = "poliinsaturadas";
        private const string TABLE_SALT = "Sal";
        private const string TABLE_SUGAR = "Azúcares";
        private const string TABLE_DIETARY_FIBER = "Fibra alimentaria";
        private const string TABLE_PROTEIN = "Proteínas";
        private const string TABLE_CARBOHYDRATE = "Hidratos de carbono";
        private const string TABLE_VITA = "Vitamina A";
        private const string TABLE_VITB1_THIAMINE = "Tiamina";
        private const string TABLE_VITB2_RIBOFLAVIN = "Riboflavina";
        private const string TABLE_VITB3_NIACIN = "Niacina";
        private const string TABLE_VITB5_PANTOTHENIC_ACID = "Ácido pantoténico";
        private const string TABLE_VITB6 = "Vitamina B6";
        private const string TABLE_VITB7_BIOTIN = "Biotina";
        private const string TABLE_VITB9_FOLIC_ACID = "Ácido fólico";
        private const string TABLE_VITB12 = "Vitamina B12";
        private const string TABLE_VITC = "Vitamina C";
        private const string TABLE_VITD = "Vitamina D";
        private const string TABLE_VITE = "Vitamina E";
        private const string TABLE_VITK = "Vitamina K";
        private const string TABLE_IRON = "Hierro";
        private const string TABLE_ZINC = "Zinc";
        private const string TABLE_PHOSPHORUS = "Fósforo";
        private const string TABLE_CALCIUM = "Calcio";
        private const string TABLE_COPPER = "Cobre";
        private const string TABLE_IODINE = "Yodo";
        private const string TABLE_POTASSIUM = "Potasio";
        private const string TABLE_MAGNESIUM = "Magnesio";
        private const string TABLE_MANGANESE = "Manganeso";
        private const string TABLE_SELENIUM = "Selenio";
        private const string TABLE_FLUORIDE = "Fluoruro";
        private const string TABLE_CHLORIDE = "Cloruro";

        //No encontrados en la web de ECI
        private const string TABLE_CHROMIUM = "cromo";
        private const string TABLE_MOLYBDENUM = "molibdeno";
        private const string TABLE_POLYALCOHOL = "polialcoholes";
        private const string TABLE_STARCH = "almidón";
        #endregion

        readonly RemoteWebDriver browser;

        public WebScrapperElCorteInglesES(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;

            linkSuper = $"{ROOTWEBSITE}/supermercado";
            linkGourmet = $"{ROOTWEBSITE}/gourmet-club-shop";
            linkPets = $"{ROOTWEBSITE}/tienda-de-mascotas";

        }

        public List<string> GetTestData()
        {
            throw new NotImplementedException();
        }

        #region [ Interface ]

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.ELCORTEINGLES_ES;

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

            try
            {
                //First search in google
                result = GetLinkFromGoogle(gtin);
                if (result == null || result.Count == 0)
                {
                    result = GetLinkFromGeneralCI(gtin);

                    if ((result == null || result.Count == 0) && !string.IsNullOrWhiteSpace(description))
                    {
                        result = GetSiteNavigationResult(searchLinkSM, description, gtin);
                    }
                }

                // GotoLink and scrape

                return result;
            }
            catch (Exception)
            {
                return result;
            }

        }

        public WebScrappedData Webscrape(string hyperlink)
        {
            var scrapResult = new WebScrappedData
            {
                ProductUrl = hyperlink
            };

            GoTo(browser, hyperlink);

            //scrap textual data
            if (IsSupermarketLink(hyperlink))
                scrapResult.ScrappedTextual = ProcessTextualData(browser);
            else
                scrapResult.ScrappedTextual = ProcessTextualDataGeneralWeb(browser);

            //scrap Images

            scrapResult.ScrappedImages = ProcessImages(browser, scrapResult.ScrappedImages);

            scrapResult.IsSuccess = true;
            return scrapResult;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            if (gtin.Length > 13)
            {
                gtin = gtin.Substring(1);
            }
            var urlList = Find(gtin, internalCode, description);
            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in ElCorteIngles.es", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }

        #endregion

        #region [ Find - Helpers ]

        /// <summary>
        /// Preforms google search and return the links containing the EAN/Gtin
        /// </summary>
        /// <param name="gtin"></param>
        /// <returns></returns>
        private List<string> GetLinkFromGoogle(string gtin)
        {
            try
            {
                //Search in google by gtin and company name
                GoTo(browser, $"{googleUrl}{gtin}%20{CNToSearch}");

                //Get results list
                var resultList = browser.FindElements(By.XPath("//div[@class='g']"));

                //Filter results with only my GTIN and get the first
                var myResult = resultList.Where(item =>
                {
                    var href = item.FindElement(By.ClassName("r")).FindElement(By.TagName("a")).GetAttribute("href");
                    var text = WebscraperUtils.GetTextFromElement(item.FindElement(By.ClassName("s")));

                    if (href.Contains(CNToSearch))
                    {
                        return true;
                    }
                    return false;
                }).ToList();


                if (myResult != null)
                {
                    //return link
                    var possibleLinks = myResult.Select(i => i.FindElement(By.ClassName("r")).FindElement(By.TagName("a")).GetAttribute("href")).ToList();
                    possibleLinks = possibleLinks.Where(l => l.StartsWith($"{ROOTWEBSITE}/supermercado") || l.StartsWith($"{ROOTWEBSITE}/club-del-gourmet")).ToList();
                    return GetLinksWithGtin(possibleLinks, gtin);
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Search in the ECI - General Website by EAN/Gtin and return the link of the product founded
        /// </summary>
        /// <param name="gtin"></param>
        /// <returns></returns>
        private List<string> GetLinkFromGeneralCI(string gtin)
        {
            try
            {
                //https://www.elcorteingles.es/search/?s=5010103916738
                var searchUrl = $"{searchLink}{gtin}";

                GoTo(browser, searchUrl);

                if (browser.Url != searchUrl)
                {
                    //We found the product because the web redirects you to the product page
                    var resp = new List<string> { browser.Url };

                    if (WebscraperUtils.IsElementPresent(browser, By.Id("product-info"))) //Try to search in Club del Gourmet because we have more data in that website
                    {
                        var productName = WebscraperUtils.GetTextFromElement(browser.FindElement(By.XPath("//div[@id='product-info']/h2[@class='title']")));
                        var CGLinks = GetSiteNavigationResult(searchLinkCG, productName, gtin);
                        if (CGLinks.Count > 0)
                        {
                            resp = CGLinks;
                        }
                    }

                    return resp;
                }
                else
                {
                    //Search in supermarket with description
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Search in the ECI - SM Website the description and return the first link containing the EAN/Gtin
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="gtin"></param>
        /// <param name="isRecursion"></param>
        /// <returns></returns>
        private List<string> GetSiteNavigationResult(string pageToSearch, string searchKeyword, string gtin, bool isRecursion = false)
        {
            var result = new List<string>();

            try
            {
                GoTo(browser, pageToSearch + searchKeyword);

                var resultsFoundNumber = WebscraperUtils.GetTextFromElement(browser.FindElementByCssSelector(".grid-coincidences .semi")).Split(null)[0];

                if (resultsFoundNumber == "0" && !isRecursion)
                {
                    //try to search by partial keyword
                    var searchKeywordSplitted = searchKeyword.Split(null);
                    if (searchKeywordSplitted.Length >= 5)
                    {
                        return GetSiteNavigationResult(pageToSearch, searchKeywordSplitted[0] + " " + searchKeywordSplitted[1] + " " + searchKeywordSplitted[2], gtin, true);
                    }
                    else if (searchKeywordSplitted.Length >= 2)
                    {
                        return GetSiteNavigationResult(pageToSearch, searchKeywordSplitted[0] + " " + searchKeywordSplitted[1], gtin, true);
                    }
                }
                else if (resultsFoundNumber != "0")
                {
                    var resultsFoundLink = browser.FindElements(By.CssSelector(".grid.c12 h3.product_tile-description a")).Select(x => x.GetAttribute("href")).ToList();
                    //var resultsFound = browser.FindElementsByCssSelector(".grid.c12 h3.product_tile-description a");

                    result = GetLinksWithGtin(resultsFoundLink, gtin);
                }

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        /// <summary>
        /// For each link in the list, check and return only the links containing the EAN/Gtin
        /// </summary>
        /// <param name="resultsFoundLink"></param>
        /// <param name="gtin"></param>
        /// <returns></returns>
        private List<string> GetLinksWithGtin(List<string> resultsFoundLink, string gtin)
        {
            var result = new List<string>();
            try
            {
                foreach (var link in resultsFoundLink)
                {
                    GoTo(browser, link);

                    if (CurrentWebContainsGtin(browser, gtin))
                    {
                        result.Add(link);
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        /// <summary>
        /// Check if the current web contains the GTIN/EAN
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="gtin"></param>
        /// <returns>True if EAN/Gtin exists in the current browser</returns>
        private bool CurrentWebContainsGtin(RemoteWebDriver browser, string gtin)
        {
            try
            {
                var productEAN = browser.FindElements(By.CssSelector(".reference-container.pdp-reference span")).FirstOrDefault();
                var productEAN2 = browser.FindElements(By.Id("ean-ref")).FirstOrDefault();

                if (productEAN != null && gtin.Contains(WebscraperUtils.GetTextFromElement(productEAN)))
                {
                    return true;
                }
                else if (productEAN2 != null && gtin.Contains(WebscraperUtils.GetTextFromElement(productEAN2)))
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region [ Scrapper - Processors ]

        /// <summary>
        /// ECI supermarket Web: Process textual data
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        private WebScrappedTextual ProcessTextualData(RemoteWebDriver browser)
        {
            var result = new WebScrappedTextual
            {
                StartProcessingOn = DateTime.Now
            };

            try
            {

                //Ingredients
                var ingredientsTitle = browser.FindElements(By.CssSelector("h3.info-title")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x) == TITLE_ING_ALERG);
                if (ingredientsTitle != null)
                {
                    var parentDiv = ingredientsTitle.FindElement(By.XPath(".."));
                    result.IngredientStatement = WebscraperUtils.GetTextFromElement(parentDiv.FindElement(By.CssSelector("ul.info-list .info-item")));
                }


                //GENERAL INFORMATION (ContactName, Address, CountryOfOrigin..)
                var generalInformationTitle = browser.FindElements(By.CssSelector("h3.info-title")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x) == TITLE_GENERAL_INFO);
                if (generalInformationTitle != null)
                {
                    var parentDiv = generalInformationTitle.FindElement(By.XPath(".."));
                    var childs = parentDiv.FindElements(By.CssSelector("ul.info-list .info-item"));

                    if (childs != null && childs.Any())
                    {
                        result.ContactName = WebscraperUtils.GetTextFromElement(childs.FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Contains(TITLE_CONTACT_NAME))).Replace(TITLE_CONTACT_NAME + ": ", "");
                        result.Address = WebscraperUtils.GetTextFromElement(childs.FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Contains(TITLE_ADDRESS))).Replace(TITLE_ADDRESS + ": ", "");
                        result.CountryOfOrigin = WebscraperUtils.GetTextFromElement(childs.FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Contains(TITLE_COUNTRYOFORIGIN))).Replace(TITLE_COUNTRYOFORIGIN + ": ", "");
                        result.NetContent = WebscraperUtils.GetTextFromElement(childs.FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Contains(TITLE_NETCONTENT))).Replace(TITLE_NETCONTENT + ": ", "");
                        result.Rations = WebscraperUtils.GetTextFromElement(childs.FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Contains(TITLE_RATIONS))).Replace(TITLE_RATIONS + ": ", "");
                    }
                }

                //Nutrients
                //var NutrientTables = browser.FindElements(By.CssSelector("._nutrients .table-row"));
                var nutrientTables = browser.FindElements(By.XPath("//div[contains(@class, 'info') and contains(@class, '_nutrients')]/table[contains(@class, 'table') and contains(@class, '_info')]/tbody/tr"));

                if (nutrientTables != null)
                {

                    foreach (var nutrient in nutrientTables)
                    {
                        var nutrienteSplitted = WebscraperUtils.GetTextFromElement(nutrient).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        var nutrientName = nutrienteSplitted.Length >= 1 ? nutrienteSplitted[0] : null;
                        var nutrientValue = nutrienteSplitted.Length >= 2 ? nutrienteSplitted[1] : null;
                        var nutrientValueWithoutUnits = CleanUnits(nutrientValue);

                        result = SetNutrientsData(nutrientName, nutrientValue, nutrientValueWithoutUnits, result);
                    }
                }

                //ConsumerUsageStorageInstructions
                var consumerUsageStorageInstructionsTitle = browser.FindElements(By.CssSelector("h3.info-title")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x) == TITLE_CONSUMERUSAGESTORAGE);
                if (consumerUsageStorageInstructionsTitle != null)
                {
                    var parentDiv = consumerUsageStorageInstructionsTitle.FindElement(By.XPath(".."));
                    var textNodes = parentDiv.FindElements(By.CssSelector("ul.info-list .info-item"));

                    result.ConsumerUsageStorageInstructions = WebscraperUtils.GetTextFromElement(textNodes[0]);
                    result.PreparationInstructions = WebscraperUtils.GetTextFromElement(textNodes.FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Contains(TITLE_PREPARATION_INSTRUCTIONS))).Replace(TITLE_PREPARATION_INSTRUCTIONS + ": ", ""); ;
                }
            }
            catch (Exception e)
            {
                result.Error = e.Message;
            }

            result.EndProcessingOn = DateTime.Now;
            return result;
        }

        /// <summary>
        /// ECI general Web: Process textual data
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        private WebScrappedTextual ProcessTextualDataGeneralWeb(RemoteWebDriver browser)
        {
            var result = new WebScrappedTextual
            {
                StartProcessingOn = DateTime.Now
            };

            try
            {
                var infoContainer = browser.FindElement(By.TagName("div"));

                if (WebscraperUtils.IsElementPresent(browser, By.Id("media-info")))
                {
                    infoContainer = browser.FindElement(By.Id("media-info"));
                }

                //Ingredients
                result.IngredientStatement = GetGeneralIngredientStatement(infoContainer);

                //ContactName
                SetGeneralInformation(infoContainer, result);

                //Nutrients
                var NutrientTables = infoContainer.FindElements(By.XPath("./table/tbody/tr"));

                if (NutrientTables != null)
                {

                    foreach (var nutrient in NutrientTables)
                    {

                        var nutrientName = WebscraperUtils.GetTextFromElement(nutrient.FindElement(By.TagName("td")));
                        var nutrientValue = WebscraperUtils.GetTextFromElement(nutrient.FindElement(By.XPath("./td[@class = 'tc']")));
                        var nutrientValueWithoutUnits = CleanUnits(nutrientValue);

                        result = SetNutrientsData(nutrientName, nutrientValue, nutrientValueWithoutUnits, result);
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

        /// <summary>
        /// Check if image exists in the browser and return ImageData
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        private List<ImageData> ProcessImages(RemoteWebDriver browser, List<ImageData> oldResult)
        {
            try
            {
                if (WebscraperUtils.IsElementPresent(browser, By.Id("product-image-placer")))
                {
                    var result = new List<ImageData>();
                    var images = browser.FindElements(By.Id("product-image-placer"));

                    var uriList = images.Select(x => x.GetAttribute("src")).ToList();
                    result = BinaryImageHelper.GetImageFromURI(uriList);

                    return result;
                }
                else
                    return oldResult;
            }
            catch (Exception)
            {
                return oldResult;
            }
        }

        #endregion

        #region [ Scrapper - Helpers ]

        /// <summary>
        /// Set nutrients data in the provided object
        /// </summary>
        /// <param name="nutrientName"></param>
        /// <param name="nutrientValue"></param>
        /// <param name="nutrientValueWithoutUnits"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private WebScrappedTextual SetNutrientsData(string nutrientName, string nutrientValue, string nutrientValueWithoutUnits, WebScrappedTextual result)
        {
            try
            {
                switch (nutrientName)
                {
                    case TABLE_CALORIES:
                        if (nutrientValue.Contains("kcal"))
                            result.Nutrients.EnergyKCal = nutrientValueWithoutUnits;
                        else
                            result.Nutrients.EnergyKJ = nutrientValueWithoutUnits;
                        break;

                    case TABLE_FAT_SATURATED:
                    case "- " + TABLE_FAT_SATURATED:
                        result.Nutrients.FatSaturated = nutrientValueWithoutUnits;
                        break;

                    case TABLE_FAT_MONOUNSATURATED:
                    case "- " + TABLE_FAT_MONOUNSATURATED:
                        result.Nutrients.FatMonoUnsaturated = nutrientValueWithoutUnits;
                        break;

                    case TABLE_FAT_POLIUNSATURATED:
                    case "- " + TABLE_FAT_POLIUNSATURATED:
                        result.Nutrients.FatPoliSaturated = nutrientValueWithoutUnits;
                        break;

                    case TABLE_FAT:
                        result.Nutrients.Fat = nutrientValueWithoutUnits;
                        break;

                    case TABLE_SALT:
                        result.Nutrients.Salt = nutrientValueWithoutUnits;
                        break;

                    case TABLE_SUGAR:
                    case "- " + TABLE_SUGAR:
                        result.Nutrients.Sugar = nutrientValueWithoutUnits;
                        break;

                    case TABLE_DIETARY_FIBER:
                        result.Nutrients.Fibre = nutrientValueWithoutUnits;
                        break;

                    case TABLE_PROTEIN:
                        result.Nutrients.Protein = nutrientValueWithoutUnits;
                        break;

                    case TABLE_CARBOHYDRATE:
                        result.Nutrients.CarboHydrates = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITA:
                        result.Nutrients.VitaminA = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITB1_THIAMINE:
                        result.Nutrients.Thiamine = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITB2_RIBOFLAVIN:
                        result.Nutrients.Riboflavin = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITB3_NIACIN:
                        result.Nutrients.Niacin = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITB5_PANTOTHENIC_ACID:
                        result.Nutrients.PantothenicAcid = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITB6:
                        result.Nutrients.VitaminB6 = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITB7_BIOTIN:
                        result.Nutrients.Biot = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITB9_FOLIC_ACID:
                        result.Nutrients.FolicAcid = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITB12:
                        result.Nutrients.VitaminB12 = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITC:
                        result.Nutrients.VitaminC = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITD:
                        result.Nutrients.VitaminD = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITE:
                        result.Nutrients.VitaminE = nutrientValueWithoutUnits;
                        break;

                    case TABLE_VITK:
                        result.Nutrients.VitaminK = nutrientValueWithoutUnits;
                        break;

                    case TABLE_IRON:
                        result.Nutrients.Iron = nutrientValueWithoutUnits;
                        break;

                    case TABLE_ZINC:
                        result.Nutrients.Zinc = nutrientValueWithoutUnits;
                        break;

                    case TABLE_PHOSPHORUS:
                        result.Nutrients.Phosphorus = nutrientValueWithoutUnits;
                        break;

                    case TABLE_CALCIUM:
                        result.Nutrients.Calcium = nutrientValueWithoutUnits;
                        break;

                    case TABLE_COPPER:
                        result.Nutrients.Copper = nutrientValueWithoutUnits;
                        break;

                    case TABLE_IODINE:
                        result.Nutrients.Iodo = nutrientValueWithoutUnits;
                        break;

                    case TABLE_POTASSIUM:
                        result.Nutrients.Potassium = nutrientValueWithoutUnits;
                        break;

                    case TABLE_MAGNESIUM:
                        result.Nutrients.Magnesium = nutrientValueWithoutUnits;
                        break;

                    case TABLE_MANGANESE:
                        result.Nutrients.Manganese = nutrientValueWithoutUnits;
                        break;

                    case TABLE_SELENIUM:
                        result.Nutrients.Selenium = nutrientValueWithoutUnits;
                        break;

                    case TABLE_FLUORIDE:
                        result.Nutrients.Fluoride = nutrientValueWithoutUnits;
                        break;

                    case TABLE_CHLORIDE:
                        result.Nutrients.Chloride = nutrientValueWithoutUnits;
                        break;

                    case TABLE_CHROMIUM:
                        result.Nutrients.Chromium = nutrientValueWithoutUnits;
                        break;

                    case TABLE_MOLYBDENUM:
                        result.Nutrients.Molybdenum = nutrientValueWithoutUnits;
                        break;

                    case TABLE_POLYALCOHOL:
                        result.Nutrients.Polyalcohol = nutrientValueWithoutUnits;
                        break;

                    case TABLE_STARCH:
                        result.Nutrients.Starch = nutrientValueWithoutUnits;
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        /// <summary>
        /// ECI genearl Web: Search ingredient statement in the provided element and return the text
        /// </summary>
        /// <param name="infoContainer"></param>
        /// <returns></returns>
        private string GetGeneralIngredientStatement(IWebElement infoContainer)
        {
            try
            {
                //Ingredients
                var text = "";
                var ingredientsTitle = infoContainer.FindElements(By.CssSelector("h5.heading")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_ING_ALERG, StringComparison.CurrentCultureIgnoreCase));
                if (ingredientsTitle != null)
                {
                    var parentDiv = ingredientsTitle.FindElement(By.XPath(".."));
                    text = WebscraperUtils.GetTextFromElement(parentDiv.FindElement(By.TagName("dd")));
                }

                return !string.IsNullOrWhiteSpace(text) ? text : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// ECI genearl Web: Search contact name in the provided element and return the text
        /// </summary>
        /// <param name="infoContainer"></param>
        /// <returns></returns>
        private void SetGeneralInformation(IWebElement infoContainer, WebScrappedTextual result)
        {
            try
            {
                //ContactName
                var contactName = string.Empty;
                var address = string.Empty;
                var netContent = string.Empty;
                var netContentVolume = string.Empty;
                var storage = string.Empty;
                var regulatedProductName = string.Empty;

                var generalInformationTitle = infoContainer.FindElements(By.CssSelector("h5.heading")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_GENERAL_INFO, StringComparison.CurrentCultureIgnoreCase));
                if (generalInformationTitle != null)
                {
                    var parentDiv = generalInformationTitle.FindElement(By.XPath(".."));
                    var head = parentDiv.FindElements(By.XPath("./dl/dt")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_CONTACT_NAME, StringComparison.CurrentCultureIgnoreCase));

                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::dd"));
                        contactName = WebscraperUtils.GetTextFromElement(res).Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];
                        address = WebscraperUtils.GetTextFromElement(res).Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];
                    }

                    head = parentDiv.FindElements(By.XPath("./dl/dt")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_NETCONTENT, StringComparison.CurrentCultureIgnoreCase));
                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::dd"));
                        var text = WebscraperUtils.GetTextFromElement(res).Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];

                        if (WebscraperUtils.IsVolumeNetContent(text))
                            netContentVolume = text;
                        else
                            netContent = text;
                    }

                    head = parentDiv.FindElements(By.XPath("./dl/dt")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_CONSUMERSTORAGE, StringComparison.CurrentCultureIgnoreCase));
                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::dd"));
                        storage = WebscraperUtils.GetTextFromElement(res).Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];
                    }

                    head = parentDiv.FindElements(By.XPath("./dl/dt")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_REGULATED_PRODUCT_NAME, StringComparison.CurrentCultureIgnoreCase));
                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::dd"));
                        regulatedProductName = WebscraperUtils.GetTextFromElement(res).Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];
                    }
                }

                result.ContactName = !string.IsNullOrWhiteSpace(contactName) ? contactName : null;
                result.Address = !string.IsNullOrWhiteSpace(address) ? address : null;
                result.ConsumerUsageStorageInstructions = !string.IsNullOrWhiteSpace(storage) ? storage : null;
                result.NetContent = !string.IsNullOrWhiteSpace(netContent) ? netContent : null;
                result.NetContentVolume = !string.IsNullOrWhiteSpace(netContentVolume) ? netContentVolume : null;
                result.RegulatedProductName = !string.IsNullOrWhiteSpace(regulatedProductName) ? regulatedProductName : null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region [ Utils ]

        /// <summary>
        /// Check if the provided link starts with one of the "supermercado" type of pages
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private bool IsSupermarketLink(string link)
        {
            try
            {
                if (link.StartsWith(linkSuper) || link.StartsWith(linkGourmet) || link.StartsWith(linkPets))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the units from value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string CleanUnits(string value)
        {
            try
            {
                value = value?.Split(null)[0];
                return value?.Replace(".", ",");
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// Go to the link provided
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="url"></param>
        private void GoTo(RemoteWebDriver browser, string url)
        {
            if (!url.Equals(browser.Url))
            {
                browser.Url = url;
                browser.Navigate();
            }
        }

        #endregion
    }
}
