using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using APIS.WebScrapperLogic.Interfaces;
using APIS.WebScrapperLogic.Utils;
using APIS.WebScrapperLogic.ImagesMatcher;
using GS1ProductTracker.Shared.Constants;
using OpenQA.Selenium.Chrome;
using System.Reflection;
using System.IO;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperDia : IWebScrapper
    {
        RemoteWebDriver browser;

        public WebScrapperDia(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public static string DiaES_LinkPrefix = "www.dia.es/compra-online/productos";

        /// <summary>
        /// O site da DiaES permite pesquisar por EAN, limpando os zeros à esquerda :
        /// </summary>
        public static string DiaES_LinkPrefix_EanSearch = "https://www.dia.es/compra-online/search?x=0&y=0&text=";

        public static string Company_Name = "dia";

        private string CleanUnits(string value)
        {
            value = value.Trim();

            if (value.Contains(" "))
            {
                value = value.Substring(0, value.IndexOf(" "));
            }
            return value.Replace(".", ",");
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.DIA;

            if (!string.IsNullOrWhiteSpace(productURL))
            {
                var linkLower = productURL.ToLower();
                result = linkLower.Contains(Company_Name);
            }

            return result;
        }

        public WebScrappedData Webscrape(string hyperlink)
        {
            WebScrappedData ret = new WebScrappedData
            {
                IsSuccess = false,
                ProductUrl = hyperlink,
                ScrappedTextual = new WebScrappedTextual()
            };

            try
            {
                browser.Url = hyperlink;

                // INavigation navigation = browser.Navigate();

                #region [ Process page data ]

                string ingredientesXpath = "//*[@id=\"nutritionalinformation\"]/*[text()=\"Ingredientes\"]/following-sibling::div[1]";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(ingredientesXpath)))
                {
                    var elem = browser.FindElements(By.XPath(ingredientesXpath)).FirstOrDefault();
                    ret.ScrappedTextual.IngredientStatement = WebscraperUtils.GetTextFromElement(elem);
                }

                string columnaRacion100 = "//*[@id=\"nutritionalinformation\"]/div[@class=\"tabs-nutritionalinfo-table-nutrients\"]/table/tbody/tr[1]/td/*[contains(text(), 100)]";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(columnaRacion100)))
                {
                    columnaRacion100 = columnaRacion100 + "/ancestor::td";
                    string col = browser.FindElement(By.XPath(columnaRacion100)).GetAttribute("colspan");
                    if (!string.IsNullOrWhiteSpace(col))
                    {
                        int column;
                        Int32.TryParse(col, out column);

                        string infoBasePath = "//*[@id=\"nutritionalinformation\"]/div[@class=\"tabs-nutritionalinfo-table-nutrients\"]/table/tbody/tr/td/*[text()=\"{0}\"]/ancestor::td/following-sibling::td[" + (column - 1) + "]/div";

                        ret.ScrappedTextual.Nutrients.CarboHydrates = GetNutrientInfo(ret, infoBasePath, "hidratos de carbono");
                        ret.ScrappedTextual.Nutrients.Fibre = GetNutrientInfo(ret, infoBasePath, "fibra alimentaria");
                        ret.ScrappedTextual.Nutrients.Fat = GetNutrientInfo(ret, infoBasePath, "grasas");
                        ret.ScrappedTextual.Nutrients.Protein = GetNutrientInfo(ret, infoBasePath, "proteínas");
                        // ret.ScrappedTextual.Nutrients.FatSaturated = GetNutrientInfo(ret, infoBasePath, "ácidos grasos saturados");
                        // ret.ScrappedTextual.Nutrients.FatMonoUnsaturated = GetNutrientInfo(ret, infoBasePath, "ácidos grasos monoinsaturados");
                        // ret.ScrappedTextual.Nutrients.FatPoliSaturated = GetNutrientInfo(ret, infoBasePath, "ácidos grasos poliinsaturados");
                        // ret.ScrappedTextual.Nutrients.Sugar = GetNutrientInfo(ret, infoBasePath, "azúcares");
                        // ret.ScrappedTextual.Nutrients.Salt = GetNutrientInfo(ret, infoBasePath, "sal");
                        // ret.ScrappedTextual.Nutrients.Niacin = GetNutrientInfo(ret, infoBasePath, "niacina");
                        // ret.ScrappedTextual.Nutrients.Riboflavin = GetNutrientInfo(ret, infoBasePath, "riboflavina");
                        // ret.ScrappedTextual.Nutrients.FolicAcid = GetNutrientInfo(ret, infoBasePath, "ácido fólico");
                        // ret.ScrappedTextual.Nutrients.Thiamine = GetNutrientInfo(ret, infoBasePath, "tiamina");
                        // ret.ScrappedTextual.Nutrients.Iron = GetNutrientInfo(ret, infoBasePath, "hierro");
                        // ret.ScrappedTextual.Nutrients.VitaminA = GetNutrientInfo(ret, infoBasePath, "vitamina A");
                        // ret.ScrappedTextual.Nutrients.VitaminB12 = GetNutrientInfo(ret, infoBasePath, "vitamina B12");
                        // ret.ScrappedTextual.Nutrients.VitaminB6 = GetNutrientInfo(ret, infoBasePath, "vitamina B6");
                        // ret.ScrappedTextual.Nutrients.VitaminC = GetNutrientInfo(ret, infoBasePath, "vitamina C");
                        // ret.ScrappedTextual.Nutrients.VitaminD = GetNutrientInfo(ret, infoBasePath, "vitamina D");
                        // ret.ScrappedTextual.Nutrients.VitaminE = GetNutrientInfo(ret, infoBasePath, "vitamina E");
                        // ret.ScrappedTextual.Nutrients.VitaminK = GetNutrientInfo(ret, infoBasePath, "vitamina K");
                        // ret.ScrappedTextual.Nutrients.Zinc = GetNutrientInfo(ret, infoBasePath, "zinc");
                        // ret.ScrappedTextual.Nutrients.Phosphorus = GetNutrientInfo(ret, infoBasePath, "fósforo");
                        // ret.ScrappedTextual.Nutrients.Biot = GetNutrientInfo(ret, infoBasePath, "biotina");
                        // ret.ScrappedTextual.Nutrients.Calcium = GetNutrientInfo(ret, infoBasePath, "calcio");
                        // ret.ScrappedTextual.Nutrients.Chloride = GetNutrientInfo(ret, infoBasePath, "cloruro");
                        // ret.ScrappedTextual.Nutrients.Chromium = GetNutrientInfo(ret, infoBasePath, "cromo");
                        // ret.ScrappedTextual.Nutrients.Copper = GetNutrientInfo(ret, infoBasePath, "cobre");
                        // ret.ScrappedTextual.Nutrients.Fluoride = GetNutrientInfo(ret, infoBasePath, "fluoruro");
                        // ret.ScrappedTextual.Nutrients.DietaryFiber = GetNutrientInfo(ret, infoBasePath, "fibra alimentaria");
                        // ret.ScrappedTextual.Nutrients.Iodo = GetNutrientInfo(ret, infoBasePath, "yodo");
                        // ret.ScrappedTextual.Nutrients.Potassium = GetNutrientInfo(ret, infoBasePath, "potasio");
                        // ret.ScrappedTextual.Nutrients.Magnesium = GetNutrientInfo(ret, infoBasePath, "magnesio");
                        // ret.ScrappedTextual.Nutrients.Manganese = GetNutrientInfo(ret, infoBasePath, "manganeso");
                        // ret.ScrappedTextual.Nutrients.Molybdenum = GetNutrientInfo(ret, infoBasePath, "molibdeno");
                        // ret.ScrappedTextual.Nutrients.PantothenicAcid = GetNutrientInfo(ret, infoBasePath, "ácido pantoténico");
                        // ret.ScrappedTextual.Nutrients.Polyalcohol = GetNutrientInfo(ret, infoBasePath, "polialcoholes");
                        // ret.ScrappedTextual.Nutrients.Selenium = GetNutrientInfo(ret, infoBasePath, "selenio");
                        // ret.ScrappedTextual.Nutrients.Starch = GetNutrientInfo(ret, infoBasePath, "almidón");

                        string energyXpath = string.Format(infoBasePath, "valor energético");
                        if (WebscraperUtils.IsElementPresent(browser, By.XPath(energyXpath)))
                            foreach (var elem in browser.FindElements(By.XPath(energyXpath)))
                                if (WebscraperUtils.GetTextFromElement(elem).ToLower().Contains("kj"))
                                    ret.ScrappedTextual.Nutrients.EnergyKJ = WebscraperUtils.GetTextFromElement(elem);
                                else if (WebscraperUtils.GetTextFromElement(elem).ToLower().Contains("kcal"))
                                    ret.ScrappedTextual.Nutrients.EnergyKCal = WebscraperUtils.GetTextFromElement(elem);
                    }
                }

                if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".zoomPad img")))
                {
                    var uriList = new List<string>().AsEnumerable();

                    var images = browser.FindElements(By.CssSelector("#productDetailUpdateable .owl-item img"));
                    if (images != null && images.Any())
                    {
                        uriList = images.Select(x => x.GetAttribute("data-zoomimagesrc"));
                    }
                    else
                    {
                        images = browser.FindElements(By.CssSelector("#zoomImagen"));
                        uriList = images.Select(x => x.GetAttribute("href"));
                    }

                    ret.ScrappedImages = BinaryImageHelper.GetImageFromURI(uriList);
                }

                // Manufacturer
                string manufacturer = "//div[@class='tabs-nutritionalinfo-manufact-informationcontent' and ./h4/text() = 'Manufacturado']";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(manufacturer)))
                {
                    var address = browser.FindElements(By.XPath($"{manufacturer}/following::div[2]")).FirstOrDefault();
                    ret.ScrappedTextual.Address = WebscraperUtils.GetTextFromElement(address);

                    var country = browser.FindElements(By.XPath($"{manufacturer}/following::div[3]")).FirstOrDefault();
                    if (WebscraperUtils.GetTextFromElement(country).Contains("Pais de origen:"))
                    {
                        ret.ScrappedTextual.CountryOfOrigin = WebscraperUtils.GetTextFromElement(country).Replace("Pais de origen:", "").Trim();
                    }
                }

                // Net Content
                string netContent = "//div[@class='tabs-nutritionalinfo-table-div' and ./div[@class='tabs-nutritionalinfo-manufact-value' and contains(text(),'CANTIDAD NETA (en masa)')]]/div[@class='tabs-nutritionalinfo-manufact-quantity']";
                string netContentVolume = "//div[@class='tabs-nutritionalinfo-table-div' and ./div[@class='tabs-nutritionalinfo-manufact-value' and contains(text(),'CANTIDAD NETA (en volumen)')]]/div[@class='tabs-nutritionalinfo-manufact-quantity']";
                string netContentdisaggregate = "//div[@class='tabs-nutritionalinfo-table-div' and ./div[@class='tabs-nutritionalinfo-manufact-value' and contains(text(),'CANTIDAD NETA (disgregada)')]]/div[@class='tabs-nutritionalinfo-manufact-quantity']";
                string netContentDrained = "//div[@class='tabs-nutritionalinfo-table-div' and ./div[@class='tabs-nutritionalinfo-manufact-value' and contains(text(),'Peso neto escurrido')]]/div[@class='tabs-nutritionalinfo-manufact-quantity']";

                if (WebscraperUtils.IsElementPresent(browser, By.XPath(netContent)))
                {
                    var netContentElement = browser.FindElements(By.XPath(netContent)).FirstOrDefault();
                    ret.ScrappedTextual.NetContent = WebscraperUtils.GetTextFromElement(netContentElement);
                }

                if (WebscraperUtils.IsElementPresent(browser, By.XPath(netContentVolume)))
                {
                    var netContentVolumeElement = browser.FindElements(By.XPath(netContentVolume)).FirstOrDefault();
                    ret.ScrappedTextual.NetContentVolume = WebscraperUtils.GetTextFromElement(netContentVolumeElement);
                }

                if (WebscraperUtils.IsElementPresent(browser, By.XPath(netContentdisaggregate)))
                {
                    var netContentdisaggregateElement = browser.FindElements(By.XPath(netContentdisaggregate)).FirstOrDefault();
                    ret.ScrappedTextual.DisaggregateNetContent = WebscraperUtils.GetTextFromElement(netContentdisaggregateElement);
                }

                if (WebscraperUtils.IsElementPresent(browser, By.XPath(netContentDrained)))
                {
                    var netContentDrainedElement = browser.FindElements(By.XPath(netContentDrained)).FirstOrDefault();
                    ret.ScrappedTextual.NetContentDrained = WebscraperUtils.GetTextFromElement(netContentDrainedElement);
                }

                // RegulatedProductName
                string regulatedProductName = "//div[@id='nutritionalinformation']/h4[ text() = 'Información Adicional']/following::div[@class='form_field-label']";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(regulatedProductName)))
                {
                    var regulatedProductNameElement = browser.FindElements(By.XPath(regulatedProductName)).FirstOrDefault();
                    ret.ScrappedTextual.RegulatedProductName = WebscraperUtils.GetTextFromElement(regulatedProductNameElement);
                }

                // ConsumerInstrucctions
                string consumerUsageStorageInstructions = "//div[@id='nutritionalinformation']/h4[ text() = 'Condiciones de conservación']/following::div[@class='form_field-label']";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(consumerUsageStorageInstructions)))
                {
                    var consumerUsageStorageInstructionsElement = browser.FindElements(By.XPath(consumerUsageStorageInstructions)).FirstOrDefault();
                    ret.ScrappedTextual.ConsumerUsageStorageInstructions = WebscraperUtils.GetTextFromElement(consumerUsageStorageInstructionsElement);
                }

                // PreparationInstructions
                string preparationInstructions = "//div[@id='nutritionalinformation']/h4[ text() = 'Modo de Empleo']/following::div[@class='form_field-label']";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(preparationInstructions)))
                {
                    var preparationInstructionsElement = browser.FindElements(By.XPath(preparationInstructions)).FirstOrDefault();
                    ret.ScrappedTextual.PreparationInstructions = WebscraperUtils.GetTextFromElement(preparationInstructionsElement);
                }

                // Rations
                string rations = "//div[@id='nutritionalinformation']/h4[ text() = 'Número de raciones por envase']/following::div[@class='form_field-label']"; ;
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(rations)))
                {
                    var rationsElement = browser.FindElements(By.XPath(rations)).FirstOrDefault();
                    ret.ScrappedTextual.Rations = WebscraperUtils.GetTextFromElement(rationsElement);
                }

                #endregion

                ret.IsSuccess = true;
                ret.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[EXCEPTION] {ex.Message}");
                Console.WriteLine($"[STACKTRACE] {ex.StackTrace}");
                Console.ResetColor();

                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
            }

            return ret;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);

            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in Dia.es", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }

        public List<string> Find(string gtin, string internalCode, string description)
        {
            var result = new List<string>();
            string searchKeyword = "";

            if (!string.IsNullOrWhiteSpace(gtin))
            {
                searchKeyword = CleanGtinFromLeadingZeroes(gtin);
                searchKeyword = searchKeyword.TrimStart('0').PadLeft(13, '0');
                result = GetSiteNavigationResult(searchKeyword);

                //tries to find by internalCode
                if (!result.Any() && !string.IsNullOrWhiteSpace(internalCode))
                {
                    result = GetSiteNavigationResult(internalCode);
                }

            }
            else if (!string.IsNullOrWhiteSpace(internalCode))
            {
                result = GetSiteNavigationResult(internalCode);
            }

            return result;
        }

        private List<string> GetSiteNavigationResult(string searchKeyword)
        {
            var result = new List<string>();

            browser.Url = DiaES_LinkPrefix_EanSearch + searchKeyword;
            browser.Navigate();
            
            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".prod_grid a")))
            {
                // O elemento verificado só consta quando não existem resultados
                var products = browser.FindElements(By.CssSelector(".prod_grid a"));

                if (products.Count == 1)
                {
                    var product = products.First();
                    var productUrl = product.GetAttribute("href");

                    result.Add(productUrl);
                }
            }
            return result;
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
