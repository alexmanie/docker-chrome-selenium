using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System.Net;
using System.Drawing;
using APIS.WebScrapperLogic.Interfaces;
using APIS.WebScrapperLogic.Utils;
using APIS.WebScrapperLogic.ImagesMatcher;
using GS1ProductTracker.Shared.Constants;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperJumboPT : IWebScrapper
    {
        public static string JumboPT_LinkPrefix_SKUSearch = "https://www.jumbo.pt/Frontoffice/search/";
        public static string JumboPT_LinkPrefix = "jumbo.pt";

        public static string Company_Name = "jumbo";

        RemoteWebDriver browser;
        public WebScrapperJumboPT(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.JUMBO_PT;

            if (!string.IsNullOrWhiteSpace(productURL))
            {
                var linkLower = productURL.ToLower();
                result = linkLower.Contains(Company_Name);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gtin"></param>
        /// <param name="internalCode"></param>
        /// <returns></returns>
        public List<string> Find(string gtin, string internalCode, string description)
        {
            var result = new List<string>();

            // Jumbo funciona com código interno. Tenho de testar com EAN / GTIN
            if (!string.IsNullOrWhiteSpace(internalCode))
            {
                browser.Url = JumboPT_LinkPrefix_SKUSearch + internalCode;
            }
            else
            {
                var gtinCleanse = CleanGtinFromLeadingZeroes(gtin);
                browser.Url = JumboPT_LinkPrefix_SKUSearch + gtin;
            }

            //go to url and get result
            browser.Navigate();

            string cssResultItemLinks = ".product-item .product-item-header a";
            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(cssResultItemLinks)))
            {
                // O elemento verificado só consta quando não existem resultados
                var elemsResultItemLinks = browser.FindElements(By.CssSelector(cssResultItemLinks));
                if (elemsResultItemLinks.Count == 1)
                {
                    result.Add(elemsResultItemLinks.First().GetAttribute("href"));
                }
            }

            return result;
        }

        public WebScrappedData FindAndWebscrape(string gtin, string internalCode, string description)
        {
            var urlList = Find(gtin, internalCode, description);
            if (!urlList.Any())
            {
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in Jumbo.PT", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }

        public List<string> GetTestData()
        {
            return new List<string>() {
                @"https://www.jumbo.pt/Frontoffice/mercearia_doce/pastilhas_chupas_e_rebucados/gomas_algodao_doce_e_outros/amendoas_caseirakrugertipo_milao150_gr/702594/Auchan_Amadora?sid=a4b54ee0-ed6f-4d20-b3f4-fead6b8d1a97_0",

                @"https://www.jumbo.pt/Frontoffice/produtos_frescos/padaria_e_pastelaria/bolos_fabrico_proprio_e_outros/bolo_fintokg/748281/Auchan_Amadora?theme=Destaques%20Folheto",

                @"https://www.jumbo.pt/Frontoffice/produtos_lacteos/iogurtes/liquidos/bebida_lacteayoggicremoso_chocolate4x160_g/2205562/Auchan_Amadora"

            };
        }

        public WebScrappedData Webscrape(string hyperlink)
        {

            var scrapResult = new WebScrappedData();
            scrapResult.ProductUrl = hyperlink;

            browser.Url = hyperlink;
            browser.Navigate();

            scrapResult.ScrappedTextual = ProcessTextualData();

            scrapResult.ScrappedImages = ProcessImages();




            scrapResult.IsSuccess = true;
            return scrapResult;
        }

        private List<ImageData> ProcessImages()
        {
            var result = new List<ImageData>();

            /* PGA: Não encontrei qualquer produto com mais do que uma imagem no JumboPT, por isso
             *      em princípio 
             * */
            string cssImageLinks = "img.product-detail-img-main";
            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(cssImageLinks)))
            {
                var elemImageLinks = browser.FindElements(By.CssSelector(cssImageLinks));

                using (var webcli = new WebClient())
                {
                    foreach (var item in elemImageLinks)
                    {
                        var image = new ImageData();
                        image.StartProcessingOn = DateTime.Now;
                        try
                        {
                            image.URI = item.GetAttribute("src");

                            var URI = new Uri(image.URI);

                            image.Filename = URI.Segments.Last();
                            image.BinaryData = webcli.DownloadData(image.URI);
                            image.MD5 = BinaryImageHelper.GetMD5(image.BinaryData);
                            image.Filesize = image.BinaryData.Length;
                            var img = Image.FromStream(new MemoryStream(image.BinaryData));
                            image.Height = img.Height;
                            image.Width = img.Width;
                            image.MIMEType = BinaryImageHelper.GetImageFormat(img);
                        }
                        catch (Exception e)
                        {
                            image.Error = e.Message;
                        }

                        image.EndProcessingOn = DateTime.Now;
                        result.Add(image);
                    }
                }
            }

            return result;
        }

        private WebScrappedTextual ProcessTextualData()
        {

            var result = new WebScrappedTextual();
            result.StartProcessingOn = DateTime.Now;

            string ingredientes = "//h2[.=\"Ingredientes e alergénios\"]/following-sibling::p[@class=\"text-primary\"]";
            if (WebscraperUtils.IsElementPresent(browser, By.XPath(ingredientes)))
            {
                var hidratosCarbonoElem = browser.FindElements(By.XPath(ingredientes)).FirstOrDefault();
                result.IngredientStatement = hidratosCarbonoElem.Text.Trim();
            }

            string informacoesNutricionais = "//h2[.=\"Informações Nutricionais\"]/following-sibling::p[@class=\"text-primary\"]";
            if (WebscraperUtils.IsElementPresent(browser, By.XPath(informacoesNutricionais)))
            {
                var informacoesNutricionaisElem = browser.FindElements(By.XPath(informacoesNutricionais)).FirstOrDefault();

                string informacoesNutricionaisText = informacoesNutricionaisElem.Text;
            }

            result.EndProcessingOn = DateTime.Now;
            return result;

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
            var uriList = imagens.Select(x => x.GetAttribute("href"));
            ret.ScrappedImages = BinaryImageHelper.GetImageFromURI(uriList);
        }

    }
}
