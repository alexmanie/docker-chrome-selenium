using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Net;
using System.Drawing;
using APIS.WebScrapperLogic.Interfaces;
using APIS.WebScrapperLogic.Utils;
using APIS.WebScrapperLogic.ImagesMatcher;
using GS1ProductTracker.Shared.Constants;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperCarrefourES : IWebScrapper
    {
        public static string CarrefourES_LinkPrefix_supermercado = "https://www.carrefour.es/supermercado/";
        public static string CarrefourES_LinkPrefix_other = "https://www.carrefour.es/";
        public static string CarrefourES_LinkPrefix_EanSearch = "https://www.carrefour.es/global/?Dy=1&Nty=1&Ntx=mode+matchallany&Ntt={0}&search=Buscar";

        public static string Company_Name = "carrefour";

        #region [ HTML - CONSTS ]

        private const string TITLE_GENERAL_INFO = "Información general";
        private const string TITLE_PRODUCT_DATA = "Datos del producto";
        private const string TITLE_CONTACT_NAME = "Nombre del operador de la empresa alimentaria";
        private const string TITLE_ADDRESS = "Dirección del operador/importador";
        private const string TITLE_MEASURES = "Medidas";
        private const string TITLE_NETCONTENT = "Cantidad neta del alimento";
        private const string TITLE_INSTRUCTIONS = "Instrucciones";
        private const string TITLE_BRAND = "Marca";
        private const string TITLE_DRAINED_WEIGHT = "Peso neto escurrido";
        private const string TITLE_PREPARATION_INSTRUCTIONS = "Modo de empleo";
        private const string TITLE_REGULATED_PRODUCT_NAME = "Denominación del alimento";

        private const string TABLE_CALORIES = "Valor energético";
        private const string TABLE_FAT = "Grasas";
        private const string TABLE_FAT_SATURATED = "de las cuales Saturadas";
        private const string TABLE_FAT_MONOUNSATURATED = "monoinsaturadas";
        private const string TABLE_FAT_POLIUNSATURATED = "poliinsaturadas";
        private const string TABLE_SALT = "Sal";
        private const string TABLE_SUGAR = "de los cuales Azúcares";
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
        private const string TABLE_VITB9_FOLIC_ACID = "Ácido Fólico";
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

        private const string TABLE_CHROMIUM = "cromo";
        private const string TABLE_MOLYBDENUM = "molibdeno";
        private const string TABLE_POLYALCOHOL = "polialcoholes";
        private const string TABLE_STARCH = "almidón";

        #endregion

        RemoteWebDriver browser;

        public WebScrapperCarrefourES(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.CARREFOUR_ES;

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

            browser.Url = string.Format(CarrefourES_LinkPrefix_EanSearch, searchKeyword);
            browser.Navigate();

            if (WebscraperUtils.IsElementPresent(browser, By.ClassName("result-list")))
            {
                // O elemento verificado só consta quando não existem resultados
                string totalNumber = browser.FindElementByClassName("result-list").GetAttribute("childElementCount");
                if (totalNumber == "1")
                {
                    result.Add(browser.FindElementByCssSelector(".producto a").GetAttribute("href"));
                }
            }

            return result;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);
            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in Carrefour.es", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }

        public List<string> GetTestData()
        {
            return new List<string>()
            {
                //Helado Cornetto distintos sabores
                @"https://www.carrefour.es/supermercado/detail/Helado-cono-mix-mini-chocolate-caramelo-y-clasico/_/R-759609273?sb=true",
                //Kellogs Chocolate
                @"https://www.carrefour.es/supermercado/detail/Cereales-de-chocolate/_/R-783315154?sb=true",
                //Lomo merluza Pescanova
                @"https://www.carrefour.es/supermercado/detail/Lomos-de-merluza/_/R-521031241?sb=true",
                //Hero Baby Tarrina Merienda
                @"https://www.carrefour.es/pack-dos-tarrinas-merienda-hero-baby-platano-y-yogur/2003043942/p",
                //TV SAMSUNG
                @"https://www.carrefour.es/tv-led-43-samsung-43mu6125-uhd-4k-smart-tv/VC4A-2822375/p"
            };
        }

        public WebScrappedData Webscrape(string hyperlink)
        {
            var scrapResult = new WebScrappedData
            {
                ProductUrl = hyperlink
            };

            browser.Url = hyperlink;
            browser.Navigate();

            var isSupermercado = hyperlink.ToLower().Contains(CarrefourES_LinkPrefix_supermercado.ToLower());

            try
            {
                if (isSupermercado)
                {
                    WebDriverWait wait =
                        new WebDriverWait(browser, new TimeSpan(0, 0, 0, 0, 3000));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("title-03")));

                    //productName = browser.FindElementByClassName("title-03").Text.Trim();
                }
                else
                {
                    WebDriverWait wait =
                        new WebDriverWait(browser, new TimeSpan(0, 0, 0, 0, 500));
                    wait.Until(ExpectedConditions.ElementExists(By.ClassName("product-header__name")));

                    //productName = browser.FindElementByClassName("product_name").Text.Trim();
                }
            }
            catch (Exception ex)
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in Carrefour.es" };
            }

            //scrap textual data
            scrapResult.ScrappedTextual = ProcessTextualData(browser);

            //scrap Images
            if (isSupermercado && WebscraperUtils.IsElementPresent(browser, By.ClassName("owl-stage")))
            {
                scrapResult.ScrappedImages = ProcessImages(browser, isSupermercado);
            }
            else if (WebscraperUtils.IsElementPresent(browser, By.ClassName("pics-slider__container")))
            {
                scrapResult.ScrappedImages = ProcessImages(browser, isSupermercado);
            }

            scrapResult.IsSuccess = true;
            return scrapResult;
        }

        private List<ImageData> ProcessImages(RemoteWebDriver browser, bool isSupermercado)
        {
            var result = new List<ImageData>();
            List<IWebElement> images = new List<IWebElement>();
            List<string> uriList = new List<string>();
            List<string> finalList = new List<string>();

            if (isSupermercado)
            {
                images = browser.FindElementsByCssSelector("#carr1 > div.owl-stage-outer > div.owl-stage div a img").ToList();
                uriList = images.Select(x => x.GetAttribute("src")).ToList();
                if (uriList.Count > 0)
                {
                    foreach (var item in uriList)
                    {
                        var correctUri = item.Replace("hd_1600x_", "hd_510x_");
                        if (!correctUri.Contains("https:"))
                        {
                            correctUri = "https:" + correctUri;
                        }

                        finalList.Add(correctUri);
                    }
                }
            }
            else
            {
                images = browser.FindElementsByCssSelector(".pics-slider__thumbnails li img").ToList();
                uriList = images.Select(x => x.GetAttribute("src")).ToList();
                if (uriList.Count > 0)
                {
                    //Cogemos la resolucion que nos interesa
                    foreach (var item in uriList)
                    {
                        var correctUri = item.Replace("hd_65x_", "hd_510x_");
                        if (!correctUri.Contains("https:"))
                        {
                            correctUri = "https:" + correctUri;
                        }

                        finalList.Add(correctUri);
                    }
                }
            }

            if (finalList.Count > 0)
            {
                result = BinaryImageHelper.GetImageFromURI(finalList);
            }

            return result;
        }

        private WebScrappedTextual ProcessTextualData(RemoteWebDriver browser)
        {
            var result = new WebScrappedTextual
            {
                StartProcessingOn = DateTime.Now
            };

            try
            {
                if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".aceptocookiesgdpr.btn-cookies")))
                {
                    var cookies = browser.FindElementByCssSelector(".aceptocookiesgdpr.btn-cookies"); ;
                    cookies.Click();
                }

                if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".more-info")))
                {
                    var moreInfoNode = browser.FindElementByCssSelector(".more-info");

                    //CONSUMERMESSAGEINSTRUCTIONS
                    var head = moreInfoNode.FindElements(By.CssSelector(".info-title")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_INSTRUCTIONS, StringComparison.CurrentCultureIgnoreCase));
                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::p"));
                        result.ConsumerUsageStorageInstructions = WebscraperUtils.GetTextFromElement(res);
                    }

                    //CONTACTNAME - ADDRESS
                    head = moreInfoNode.FindElements(By.CssSelector(".info-title")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_PRODUCT_DATA, StringComparison.CurrentCultureIgnoreCase));
                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::p"));
                        var text = WebscraperUtils.GetTextFromElement(res).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        result.ContactName = text.FirstOrDefault(x => x.Contains(TITLE_CONTACT_NAME)) != null ? text.FirstOrDefault(x => x.Contains(TITLE_CONTACT_NAME)).Replace(TITLE_CONTACT_NAME + ":", "") : null;
                        result.Address = text.FirstOrDefault(x => x.Contains(TITLE_ADDRESS)) != null ? text.FirstOrDefault(x => x.Contains(TITLE_ADDRESS)).Replace(TITLE_ADDRESS + ":", "") : null;
                        result.RegulatedProductName = text.FirstOrDefault(x => x.Contains(TITLE_REGULATED_PRODUCT_NAME)) != null ? text.FirstOrDefault(x => x.Contains(TITLE_REGULATED_PRODUCT_NAME)).Replace(TITLE_REGULATED_PRODUCT_NAME + ":", "") : null;
                    }

                    //NETCONTENT - DRAINEDWEIGHT
                    head = moreInfoNode.FindElements(By.CssSelector(".info-title")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_MEASURES, StringComparison.CurrentCultureIgnoreCase));
                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::p"));
                        var text = WebscraperUtils.GetTextFromElement(res).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        var netContentRaw = text.FirstOrDefault(x => x.Contains(TITLE_NETCONTENT)) != null ? text.FirstOrDefault(x => x.Contains(TITLE_NETCONTENT)).Replace(TITLE_NETCONTENT + ":", "") : null;
                        result.NetContentDrained = text.FirstOrDefault(x => x.Contains(TITLE_DRAINED_WEIGHT)) != null ? text.FirstOrDefault(x => x.Contains(TITLE_DRAINED_WEIGHT)).Replace(TITLE_DRAINED_WEIGHT + ":", "") : null;

                        if (WebscraperUtils.IsVolumeNetContent(netContentRaw))
                            result.NetContentVolume = netContentRaw;
                        else
                            result.NetContent = netContentRaw;
                    }

                    //PREPARATIONINSTRUCTIONS
                    head = moreInfoNode.FindElements(By.CssSelector(".info-title")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_PREPARATION_INSTRUCTIONS, StringComparison.CurrentCultureIgnoreCase));
                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::p"));
                        result.PreparationInstructions = WebscraperUtils.GetTextFromElement(res);
                    }

                    //BRAND
                    head = moreInfoNode.FindElements(By.CssSelector(".info-title")).FirstOrDefault(x => WebscraperUtils.GetTextFromElement(x).Equals(TITLE_BRAND, StringComparison.CurrentCultureIgnoreCase));
                    if (head != null)
                    {
                        var res = head.FindElement(By.XPath("./following-sibling::p"));
                        result.Brand = WebscraperUtils.GetTextFromElement(res);
                    }
                }

                //INGREDIENTSTATEMENT
                if (WebscraperUtils.IsElementPresent(browser, By.Id("ingredientes")))
                {
                    result.IngredientStatement = browser.FindElementById("ingredientes").FindElement(By.ClassName("contenido-ingredientes")).Text;
                }

                //NUTRITIONAL INFO
                if (WebscraperUtils.IsElementPresent(browser, By.Id("nutrition")))
                {
                    var nutritionNode = browser.FindElementById("nutrition");

                    var graphic = nutritionNode.FindElements(By.Id("grafico")).FirstOrDefault();
                    if (graphic != null)
                    {
                        var kcalNode = graphic.FindElements(By.Id("kcal")).FirstOrDefault();
                        var kjNode = graphic.FindElements(By.Id("kj")).FirstOrDefault();

                        result.Nutrients.EnergyKJ = WebscraperUtils.GetTextFromElement(kjNode);
                        result.Nutrients.EnergyKCal = WebscraperUtils.GetTextFromElement(kcalNode);
                    }

                    var nutrientsNode = nutritionNode.FindElements(By.CssSelector(".nutrition-box")).FirstOrDefault();
                    if (nutrientsNode != null)
                    {
                        var allNutrients = nutrientsNode.FindElements(By.XPath("./p")).ToList();
                        allNutrients.AddRange(nutrientsNode.FindElements(By.XPath("./ul/li/p")));

                        //Hacemos click en el botón "ver todos".
                        var showMoreElement = nutrientsNode.FindElements(By.CssSelector(".show-more-info.icono-arrowright2.js-show-more-info-nutrition")).FirstOrDefault();
                        if (showMoreElement != null)
                        {
                            showMoreElement.Click();
                        }

                        foreach (var nutrient in allNutrients)
                        {
                            var text = WebscraperUtils.GetTextFromElement(nutrient).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                            if (text.Count() > 1)
                            {
                                var nutrientKey = text[0];
                                var nutrientValue = text[1];

                                SetNutrient(result, nutrientKey, nutrientValue);
                            }
                        }
                    }
                }

                string infoBasePath = "//h3[.=\"Información Alimentaria\"]/following-sibling::P/*[contains( text(),\"Nutrientes:\")]";

                if (WebscraperUtils.IsElementPresent(browser, By.XPath(infoBasePath)))
                {
                    //Nutrientes
                    string energy = "//*[text()=\"Valor Energético\"]";
                    if (WebscraperUtils.IsElementPresent(browser, By.XPath(energy)))
                    {
                        string foodInfo = browser.FindElementByXPath(energy).Text;

                        string[] elements = foodInfo.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < (elements.Length - 1); i++)
                        {
                            string currString = elements[i].ToLower().Trim();
                            string nextString = elements[i + 1].ToLower().Trim();

                            switch (currString)
                            {
                                case "valor energético": // 824.0 kJ ou 197.0 Kcal, existem várias ocorrências
                                    if (nextString.Contains("kcal") || nextString.Contains("kilocalorías"))
                                        result.Nutrients.EnergyKCal = CleanUnits(nextString);
                                    else
                                        result.Nutrients.EnergyKJ = CleanUnits(nextString);
                                    break;

                                case "grasas":
                                    result.Nutrients.Fat = CleanUnits(nextString);
                                    break;

                                case "saturadas":
                                    result.Nutrients.FatSaturated = CleanUnits(nextString);
                                    break;

                                case "hidratos de carbono":
                                    result.Nutrients.CarboHydrates = CleanUnits(nextString);
                                    break;

                                case "azúcares":
                                    result.Nutrients.Sugar = CleanUnits(nextString);
                                    break;

                                case "proteínas":
                                    result.Nutrients.Protein = CleanUnits(nextString);
                                    break;

                                case "sal":
                                    result.Nutrients.Salt = CleanUnits(nextString);
                                    break;

                                default:
                                    break;
                            }
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

        private void SetNutrient(WebScrappedTextual result, string nutrientKey, string nutrientValue)
        {
            try
            {

                var cleanNutrientValue = CleanUnits(nutrientValue);

                switch (nutrientKey)
                {
                    case TABLE_FAT:
                        result.Nutrients.Fat = cleanNutrientValue;
                        break;

                    case TABLE_FAT_SATURATED:
                        result.Nutrients.FatSaturated = cleanNutrientValue;
                        break;

                    case TABLE_CARBOHYDRATE:
                        result.Nutrients.CarboHydrates = cleanNutrientValue;
                        break;

                    case TABLE_SUGAR:
                        result.Nutrients.Sugar = cleanNutrientValue;
                        break;

                    case TABLE_PROTEIN:
                        result.Nutrients.Protein = cleanNutrientValue;
                        break;

                    case TABLE_SALT:
                        result.Nutrients.Salt = cleanNutrientValue;
                        break;

                    case TABLE_FAT_MONOUNSATURATED:
                        result.Nutrients.FatMonoUnsaturated = cleanNutrientValue;
                        break;

                    case TABLE_FAT_POLIUNSATURATED:
                        result.Nutrients.FatPoliSaturated = cleanNutrientValue;
                        break;

                    case TABLE_DIETARY_FIBER:
                        result.Nutrients.DietaryFiber = cleanNutrientValue;
                        break;

                    case TABLE_VITA:
                        result.Nutrients.VitaminA = cleanNutrientValue;
                        break;

                    case TABLE_VITB1_THIAMINE:
                        result.Nutrients.Thiamine = cleanNutrientValue;
                        break;

                    case TABLE_VITB2_RIBOFLAVIN:
                        result.Nutrients.Riboflavin = cleanNutrientValue;
                        break;

                    case TABLE_VITB3_NIACIN:
                        result.Nutrients.Niacin = cleanNutrientValue;
                        break;

                    case TABLE_VITB5_PANTOTHENIC_ACID:
                        result.Nutrients.PantothenicAcid = cleanNutrientValue;
                        break;

                    case TABLE_VITB6:
                        result.Nutrients.VitaminB6 = cleanNutrientValue;
                        break;

                    case TABLE_VITB7_BIOTIN:
                        result.Nutrients.Biot = cleanNutrientValue;
                        break;

                    case TABLE_VITB9_FOLIC_ACID:
                        result.Nutrients.FolicAcid = cleanNutrientValue;
                        break;

                    case TABLE_VITB12:
                        result.Nutrients.VitaminB12 = cleanNutrientValue;
                        break;

                    case TABLE_VITC:
                        result.Nutrients.VitaminC = cleanNutrientValue;
                        break;

                    case TABLE_VITD:
                        result.Nutrients.VitaminD = cleanNutrientValue;
                        break;

                    case TABLE_VITE:
                        result.Nutrients.VitaminE = cleanNutrientValue;
                        break;

                    case TABLE_VITK:
                        result.Nutrients.VitaminK = cleanNutrientValue;
                        break;

                    case TABLE_IRON:
                        result.Nutrients.Iron = cleanNutrientValue;
                        break;

                    case TABLE_ZINC:
                        result.Nutrients.Zinc = cleanNutrientValue;
                        break;

                    case TABLE_PHOSPHORUS:
                        result.Nutrients.Phosphorus = cleanNutrientValue;
                        break;

                    case TABLE_CALCIUM:
                        result.Nutrients.Calcium = cleanNutrientValue;
                        break;

                    case TABLE_COPPER:
                        result.Nutrients.Copper = cleanNutrientValue;
                        break;

                    case TABLE_IODINE:
                        result.Nutrients.Iodo = cleanNutrientValue;
                        break;

                    case TABLE_POTASSIUM:
                        result.Nutrients.Potassium = cleanNutrientValue;
                        break;

                    case TABLE_MAGNESIUM:
                        result.Nutrients.Magnesium = cleanNutrientValue;
                        break;

                    case TABLE_MANGANESE:
                        result.Nutrients.Manganese = cleanNutrientValue;
                        break;

                    case TABLE_SELENIUM:
                        result.Nutrients.Selenium = cleanNutrientValue;
                        break;

                    case TABLE_FLUORIDE:
                        result.Nutrients.Fluoride = cleanNutrientValue;
                        break;

                    case TABLE_CHLORIDE:
                        result.Nutrients.Chloride = cleanNutrientValue;
                        break;

                    case TABLE_CHROMIUM:
                        result.Nutrients.Chromium = cleanNutrientValue;
                        break;

                    case TABLE_MOLYBDENUM:
                        result.Nutrients.Molybdenum = cleanNutrientValue;
                        break;

                    case TABLE_POLYALCOHOL:
                        result.Nutrients.Polyalcohol = cleanNutrientValue;
                        break;

                    case TABLE_STARCH:
                        result.Nutrients.Starch = cleanNutrientValue;
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string CleanUnits(string value)
        {
            value = value.Trim();
            if (value.Contains(" "))
            {
                value = value.Substring(0, value.IndexOf(" "));
            }
            return value.Replace(".", ",");
        }

        private string CleanGtinFromLeadingZeroes(string gtin)
        {
            return gtin.TrimStart(new Char[] { '0' });
        }

        private void ExtractImage(RemoteWebDriver browser, TextWriter debugOutput, WebScrappedData ret)
        {
            ICollection<IWebElement> imagens = browser.FindElementsByCssSelector(".image.easyzoom a");
            int i = browser.FindElementsByCssSelector(".thumb li").Count();
            debugOutput.WriteLine("images counter:" + i);

            using (var webcli = new WebClient())
            {
                foreach (var item in imagens)
                {
                    ImageData wi = new ImageData();
                    //imagensString += item.GetAttribute("href") + Environment.NewLine;
                    wi.URI = item.GetAttribute("href");
                    debugOutput.WriteLine("Scraping image: " + wi.URI);

                    Uri uri = new Uri(wi.URI);

                    wi.Filename = ""; // Filename aquisition failed - invalid Uri?
                    wi.Filename = uri.Segments.Last();
                    //if (uri.IsFile) // doesn't work
                    //    wi.Filename = System.IO.Path.GetFileName(uri.LocalPath);

                    wi.BinaryData = webcli.DownloadData(wi.URI);
                    wi.MD5 = BinaryImageHelper.GetMD5(wi.BinaryData);
                    wi.Filesize = wi.BinaryData.Length;

                    try
                    {
                        Image a = Bitmap.FromStream(new MemoryStream(wi.BinaryData));
                        wi.Height = a.Height;
                        wi.Width = a.Width;
                    }
                    catch (Exception)
                    {
                        wi.Height = -1;
                        wi.Width = -1;
                    }

                    ret.ScrappedImages.Add(wi);
                    //wc.DownloadFile()
                }
            }
        }
    }
}
