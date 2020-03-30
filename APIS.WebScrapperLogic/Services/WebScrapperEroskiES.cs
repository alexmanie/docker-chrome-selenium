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
    public class WebScrapperEroskiES : IWebScrapper
    {
        public static string Eroski_LinkPrefix = "www.compraonline.grupoeroski.com/es/productdetail/";
        public string Eroski_LinkPrefix_2 = "www.eroski.es";

        public static string Company_Name = "eroski";

        /// <summary>
        /// O site da Eroski es permite pesquisar por EAN, limpando os zeros à esquerda
        /// </summary>
        public static string EroskiES_LinkPrefix_EanSearch = "https://www.compraonline.grupoeroski.com/es/search/results/?q=";

        // ATENÇÃO: este set de links de teste tem dois tipos de páginas: supermercado e "outro"
        public List<string> GetTestData()
        {
            return new List<string>() {
                    // 07613032330163 Masa para empanada BUITONI, bolsa 280 g 
                    @"https://www.compraonline.grupoeroski.com/es/productdetail/11880531-masa-para-empanada-buitoni-bolsa-280-g"

                    // Gelatina para gato GOURMET Perle, pack 4x85 g
                    , @"https://www.compraonline.grupoeroski.com/es/productdetail/12346771-gelatina-para-gato-gourmet-perle-pack-4x85-g"
            };
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.EROSKI_ES;

            if (!string.IsNullOrWhiteSpace(productURL))
            {
                var linkLower = productURL.ToLower();
                result = linkLower.Contains(Company_Name);
            }

            return result;
        }


        private List<ImageData> ProcessImages(RemoteWebDriver browser)
        {
            var result = new List<ImageData>();

            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".product-image img")))
            {
                var images = browser.FindElements(By.CssSelector(".product-image img"));
                var uriList = images.Select(x => x.GetAttribute("href"));
                result = BinaryImageHelper.GetImageFromURI(uriList);
            }

            return result;
        }

        public WebScrappedData Webscrape(string hyperlink)
        {
            WebScrappedData ret = new WebScrappedData();
            ret.ProductUrl = hyperlink;

            browser.Url = hyperlink;

            INavigation navigation = browser.Navigate();

            browser.ExecuteScript("window.scrollBy(0,550)", "");

            ret.ScrappedTextual = ProcessTextualData(browser);
            ret.ScrappedImages = ProcessImages(browser);


            ret.IsSuccess = true;
            ret.ErrorMessage = null;
            return ret;
        }


        private WebScrappedTextual ProcessTextualData(RemoteWebDriver browser)
        {
            var result = new WebScrappedTextual
            {
                StartProcessingOn = DateTime.Now
            };

            // $('.itemFood.foodEnergeyContent .kJ').text()
            //string fatPercent, fatSaturatedPercent, carboPercent, sugarPercent, proteinPercent, saltPercent = "";


            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".feature-text-ingredients p.text")))
            {
                var element = browser.FindElements(By.CssSelector(".feature-text-ingredients p.text")).First();
                result.IngredientStatement = element.Text;
            }

            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(".feature-company p.text")))
            {
                var element = browser.FindElements(By.CssSelector(".feature-company p.text")).First();
                result.ContactName = element.Text;
            }

            string cssNutrients = ".feature.feature-list li";
            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(cssNutrients)))
            {
                // Exemplo com infantil: https://www.compraonline.grupoeroski.com/es/productdetail/300947-cheerios-con-miel-nestle-caja-375-g

                var nutrientes = browser.FindElements(By.CssSelector(cssNutrients));
                foreach (var nutriente in nutrientes)
                {

                    string nutrienteName = nutriente.Text.Trim().ToLower();
                    string nutrientValue = nutriente.FindElement(By.CssSelector("span")).Text;

                    if (nutrienteName.StartsWith("sal "))
                    {
                        result.Nutrients.Salt = nutrientValue;
                    }
                    else if (nutrienteName.StartsWith("proteínas "))
                    {
                        result.Nutrients.Protein = nutrientValue;
                    }
                    else if (nutrienteName.StartsWith("fibra alimentaria "))
                    {
                        result.Nutrients.Fibre = nutrientValue;
                    }
                    else if (nutrienteName.StartsWith("azucares "))
                    {
                        result.Nutrients.Sugar = nutrientValue;
                    }
                    else if (nutrienteName.StartsWith("ácidos grasos saturados "))
                    {
                        result.Nutrients.FatSaturated = nutrientValue;
                    }
                    else if (nutrienteName.StartsWith("grasas "))
                    {
                        result.Nutrients.Fat = nutrientValue;
                    }
                    else if (nutrienteName.StartsWith("hidratos de carbono "))
                    {
                        result.Nutrients.CarboHydrates = nutrientValue;
                    }
                    else if (nutrienteName.StartsWith("valos enérgetico "))
                    {
                        // Exemplos de "nutrientValue" para efectuar parse
                        // Valos enérgetico 1595.0 / 377.0 kilojulios / kilocaloría it(international table)
                        // Valos enérgetico 2187/523 kilojulios/kilocaloría it (international table)
                        //
                        nutrientValue = nutrientValue
                            .Replace("Valos enérgetico ", "")
                            .Replace("it (international", "it(international")
                            .Replace(" / ", "")
                            .Replace("kilojulios/kilocaloría it(international table)", "");
                        var _values = nutrientValue.Split('/');
                        decimal _Parse;
                        if (_values.Length == 2
                            && decimal.TryParse(_values[0].Trim(), out _Parse)
                            && decimal.TryParse(_values[1].Trim(), out _Parse))
                        {

                            result.Nutrients.EnergyKJ = _values[0].Trim();
                            result.Nutrients.EnergyKCal = _values[1].Trim();
                        }
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
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in GrupoEroski.com", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }


        private static string cleanGtin(string gtin)
        {
            return gtin.TrimStart(new Char[] { '0' });
        }

        public List<string> Find(string gtin, string internalCode, string description)
        {
            var result = new List<string>();
            gtin = cleanGtin(gtin);

            browser.Url = EroskiES_LinkPrefix_EanSearch + gtin;
            browser.Navigate();

            // COM RESULTADO https://www.compraonline.grupoeroski.com/es/search/results/?q=7613032330163
            // SEM RESULTADO https://www.compraonline.grupoeroski.com/es/search/results/?q=7613032330164
            if (!WebscraperUtils.IsElementPresent(browser, By.CssSelector(".lineal-products-no-products")))
            {
                // O elemento verificado só consta quando não existem resultados
                var products = browser.FindElements(By.CssSelector("a.product-image"));

                if (products.Count == 1)
                {
                    var product = products.First();
                    var productUrl = product.GetAttribute("href");
                    result.Add(productUrl);
                }
               
            }
            return result;
        }

        RemoteWebDriver browser;
        public WebScrapperEroskiES(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

    }
}
