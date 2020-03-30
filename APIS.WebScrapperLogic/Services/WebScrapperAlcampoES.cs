using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using APIS.WebScrapperLogic.Interfaces;
using APIS.WebScrapperLogic.Utils;
using APIS.WebScrapperLogic.ImagesMatcher;
using GS1ProductTracker.Shared.Constants;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperAlcampoES : IWebScrapper
    {
        public static string AlcampoES_LinkPrefix = "www.alcampo.es/compra-online/alimentacion/";
        public static string AlcampoES_LinkPrefix2 = "www.alcampo.es/compra-online/bebidas/";

        public static string Company_Name = "alcampo";


        RemoteWebDriver browser;
        public WebScrapperAlcampoES(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public List<string> GetTestData()
        {

            return new List<string>() {
// 5449000000996 - Coca cola lata 33cl http://portalsyncptqa.gs1pt.org/gs1producttrackingtestimages/5449000000996.jpg
@"http://www.alcampo.es/compra-online/bebidas/bebidas-refrescantes/refresco-cola/normal/coca-cola-refresco-de-cola-lata-de-33-centilitros/p/34053"

// 8410408067369 - Bitter kas pack 6x 200 ml http://portalsyncptqa.gs1pt.org/gs1producttrackingtestimages/8410408067369.png
, @"https://www.alcampo.es/compra-online/bebidas/bebidas-refrescantes/tonica-y-bitter/bitter/kas-bitter-botella-de-20-centilitros-pack-de-6-unidades/p/31659"

// 8410113001023 - Viña Sol 75 cl http://portalsyncptqa.gs1pt.org/gs1producttrackingtestimages/8410113001023.jpg
, @"https://www.alcampo.es/compra-online/bebidas/vinos/vino-blanco/penedes-y-otros-cataluna/do-catalunya/vinasol-vina-blanco-75-cl/p/30981"

//8410087002026 - Valenciana dulce sol http://portalsyncptqa.gs1pt.org/gs1producttrackingtestimages/8410087002026.jpg
, @"http://www.alcampo.es/compra-online/alimentacion/desayuno-y-merienda/bolleria-y-pasteleria/croissants-magdalenas-y-muffins/magdalenas/dulcesol-magdalena-valenciana-350-gramos/p/19194"

//8410500001360 - Danone 8x125 gr natural azucarado http://portalsyncptqa.gs1pt.org/gs1producttrackingtestimages/8410500001360.jpg
, @"http://www.alcampo.es/compra-online/alimentacion/huevos-leche-yogures-y-lacteos/yogures/natural/sin-azucar/danone-yogur-natural-8-x-125-gr/p/52416"
            };

        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.ALCAMPO_ES;

            if (!string.IsNullOrWhiteSpace(productURL))
            {
                var linkLower = productURL.ToLower();
                result = linkLower.Contains(Company_Name);
            }

            return result;
        }

        private WebScrappedTextual ProcessTextualData(RemoteWebDriver browser)
        {
            var result = new WebScrappedTextual();
            result.StartProcessingOn = DateTime.Now;

            try
            {

                if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".pictogramasFood .itemFood.foodEnergeyContent")))
                {
                    result.Nutrients.EnergyKJ = WebscraperUtils.GetTextByCssSelector(browser, ".itemFood.foodEnergeyContent .kJ");
                    result.Nutrients.EnergyKCal = WebscraperUtils.GetTextByCssSelector(browser, ".itemFood.foodEnergeyContent .itemKcalTitle");
                    result.Nutrients.Fat = WebscraperUtils.GetTextByCssSelector(browser, ".itemFood.foodFats .kJ");
                    result.Nutrients.FatSaturated = WebscraperUtils.GetTextByCssSelector(browser, ".itemFood.foodSaturatedFats .kJ");
                    result.Nutrients.CarboHydrates = WebscraperUtils.GetTextByCssSelector(browser, ".itemFood.foodCarboHydrates .kJ");
                    result.Nutrients.Sugar = WebscraperUtils.GetTextByCssSelector(browser, ".itemFood.foodSugars .kJ");
                    result.Nutrients.Protein = WebscraperUtils.GetTextByCssSelector(browser, ".itemFood.foodProteins .kJ");
                    result.Nutrients.Salt = WebscraperUtils.GetTextByCssSelector(browser, ".itemFood.foodSalt .kJ");

                }

                if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".foodIngredients")))
                {
                    result.IngredientStatement = WebscraperUtils.GetTextByCssSelector(browser, ".foodIngredients");
                    //prune titles
                    result.IngredientStatement = result.IngredientStatement.Replace("Ingredientes:", "").Trim();
                }

                //Cosumer instructions
                if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".productConservationCoditions")))
                {
                    result.ConsumerUsageStorageInstructions = WebscraperUtils.GetTextByCssSelector(browser, ".productConservationCoditions");
                }

                //Nombre del operador
                string xpath_nombreDelOperador = "//span[contains(text(),\"Nombre del operador\")]//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_nombreDelOperador)))
                {
                    var nombreDelOperador = browser.FindElements(By.XPath(xpath_nombreDelOperador)).FirstOrDefault();
                    result.ContactName = WebscraperUtils.GetTextFromElement(nombreDelOperador);
                }

                string xpath_address = "//span[contains(text(),\"Dirección del operador\")]//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_address)))
                {
                    var addressOperador = browser.FindElements(By.XPath(xpath_address)).FirstOrDefault();
                    result.Address = WebscraperUtils.GetTextFromElement(addressOperador);
                }

                string xpath_countryOfOrigin = "//span[contains(text(),\"País de origen\")]//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_countryOfOrigin)))
                {
                    var countryOfOrigin = browser.FindElements(By.XPath(xpath_countryOfOrigin)).FirstOrDefault();
                    result.CountryOfOrigin = WebscraperUtils.GetTextFromElement(countryOfOrigin);
                }

                //Peso neto
                string xpath_netContent = "//span[text() = \"Peso Neto\"]//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_netContent)))
                {
                    //Puede haber mas de una cantidad neta, en masa y volumen
                    var netContent = browser.FindElements(By.XPath(xpath_netContent)).FirstOrDefault();
                    result.NetContent = WebscraperUtils.GetTextFromElement(netContent);
                }

                //Peso neto en volumen
                string xpath_netContentVolume = "//span[text() = 'Peso neto en volumen']//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_netContentVolume)))
                {
                    //Puede haber mas de una cantidad neta, en masa y volumen
                    var netContent = browser.FindElements(By.XPath(xpath_netContentVolume)).FirstOrDefault();
                    result.NetContentVolume = WebscraperUtils.GetTextFromElement(netContent);
                }

                //Peso neto escurrido
                string xpath_netContentDrained = "//span[text() = \"Peso neto escurrido\"]//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_netContentDrained)))
                {
                    //Puede haber mas de una cantidad neta, en masa y volumen
                    var netContent = browser.FindElements(By.XPath(xpath_netContent)).FirstOrDefault();
                    result.NetContentDrained = WebscraperUtils.GetTextFromElement(netContent);
                }

                //Cantidad neta disgregada
                string xpath_netContentDisgregated = "//span[contains(text(),\"Cantidad neta disgregada\")]//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_netContentDisgregated)))
                {
                    var netContentDisgregated = browser.FindElements(By.XPath(xpath_netContentDisgregated)).FirstOrDefault();
                    result.DisaggregateNetContent = WebscraperUtils.GetTextFromElement(netContentDisgregated);
                }

                //Número de raciones
                string xpath_rations = "//span[contains(text(),\"Número de raciones\")]//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_rations)))
                {
                    var rations = browser.FindElements(By.XPath(xpath_rations)).FirstOrDefault();
                    result.Rations = WebscraperUtils.GetTextFromElement(rations);
                }

                //Denomicnación legal del alimento
                string xpath_regProdName = "//span[contains(text(),\"Denominación legal del alimento\")]//following-sibling::span";
                if (WebscraperUtils.IsElementPresent(browser, By.XPath(xpath_regProdName)))
                {
                    var regprod = browser.FindElements(By.XPath(xpath_regProdName)).FirstOrDefault();
                    result.RegulatedProductName = WebscraperUtils.GetTextFromElement(regprod);
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

        private List<ImageData> ProcessImages(RemoteWebDriver browser)
        {
            var result = new List<ImageData>();

            string css_cookieNotice = ".closeButton";

            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(css_cookieNotice)))
            {
                var elem_Imagens_POP_POP_POP_POPUP = browser.FindElements(By.CssSelector(css_cookieNotice)).FirstOrDefault();
                elem_Imagens_POP_POP_POP_POPUP.Click();
            }

            string css_Imagens_POP_POP_POP_POPUP = ".productImageZoomLink";

            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(css_Imagens_POP_POP_POP_POPUP)))
            {
                var elem_Imagens_POP_POP_POP_POPUP = browser.FindElements(By.CssSelector(css_Imagens_POP_POP_POP_POPUP)).FirstOrDefault();

                string imagesURL = elem_Imagens_POP_POP_POP_POPUP.GetAttribute("href");
                browser.Url = imagesURL;
                browser.Navigate();


                string css_Imagens_lista = ".easyzoom a.superZoomImage";

                if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(css_Imagens_lista)))
                {

                    var elem_imagens = browser.FindElements(By.CssSelector(css_Imagens_lista));
                    var uriList = elem_imagens.Select(x => x.GetAttribute("href"));
                    result = BinaryImageHelper.GetImageFromURI(uriList);
                }

            }
            return result;
        }

        public WebScrappedData Webscrape(string hyperlink)
        {
            var scrapResult = new WebScrappedData
            {
                ProductUrl = hyperlink
            };

            browser.Url = hyperlink;
            browser.Navigate();

            scrapResult.ScrappedTextual = ProcessTextualData(browser);

            scrapResult.ScrappedImages = ProcessImages(browser);

            scrapResult.IsSuccess = true;

            scrapResult.ErrorMessage = null;

            return scrapResult;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);
            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in Alcampo.es", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }

        private string CleanGtinFromLeadingZeroes(string gtin)
        {
            return gtin.TrimStart(new Char[] { '0' });
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

                return result;
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
            try
            {

                browser.Url = "http://www.alcampo.es/compra-online/search/?text=" + searchKeyword;
                browser.Navigate();

                if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".paginationBar.bottom > .totalResults")))
                {
                    var totalResults = WebscraperUtils.GetTextByCssSelector(browser, ".paginationBar.bottom > .totalResults");
                    var dataLayerProducts = ((IJavaScriptExecutor)browser).ExecuteScript("return dataLayer[0].page.totalProducts");

                    if (totalResults.StartsWith("1 Product") || dataLayerProducts.Equals("1"))
                    {
                        if (WebscraperUtils.IsElementPresent(browser, By.CssSelector("a.productMainLink.productTooltipClass")))
                        {
                            var a = browser.FindElement(By.CssSelector("a.productMainLink.productTooltipClass"));
                            result.Add(a.GetAttribute("href"));
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
