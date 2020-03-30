using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Net;
using OpenQA.Selenium.Interactions;
using System.Drawing;
using APIS.WebScrapperLogic.Interfaces;
using APIS.WebScrapperLogic.Utils;
using APIS.WebScrapperLogic.ImagesMatcher;
using GS1ProductTracker.Shared.Constants;

namespace APIS.WebScrapperLogic.Services
{
    public class WebScrapperContinentePT : IWebScrapper
    {
        public static string ContinentePT_LinkPrefix_SKUSearch = "https://www.continente.pt/stores/continente/pt-pt/public/Pages/searchResults.aspx?k=";
        public static string ContinentePT_LinkPrefix = "continente.pt";
        public static string CarrefourES_LinkPrefix_EanSearch = "https://www.carrefour.es/supermercado/c?Ntt=";

        public static string Company_Name = "continente";

        RemoteWebDriver browser;
        public WebScrapperContinentePT(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.CONTINENTE_PT;

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

            // Continente prefere código interno pois esse funciona e está confirmado. Exemplo: 5845494
            if (!string.IsNullOrWhiteSpace(internalCode))
            {
                browser.Url = ContinentePT_LinkPrefix_SKUSearch + internalCode;
            }
            else
            {
                var gtinCleanse = CleanGtinFromLeadingZeroes(gtin);
                browser.Url = ContinentePT_LinkPrefix_SKUSearch + gtin;
            }

            //go to url and get result
            browser.Navigate();

            string cssResultItemLinks = ".productsGrid .productItem .image a";
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
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in www.continente.pt", ProductRealName = description };
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
            var scrapResult = new WebScrappedData();
            scrapResult.ProductUrl = hyperlink;

            browser.Url = hyperlink;
            browser.Navigate();


            scrapResult.ScrappedImages = ProcessImages();

            scrapResult.ScrappedTextual = ProcessTextualData();



            scrapResult.IsSuccess = true;
            return scrapResult;
        }

        private List<ImageData> ProcessImages()
        {
            var result = new List<ImageData>();

            string cssImageLinks = ".productImage a";
            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(cssImageLinks)))
            {
                var elemImageLinks = browser.FindElements(By.CssSelector(cssImageLinks));
                var uriList = elemImageLinks.Select(x => x.GetAttribute("href"));

                result = BinaryImageHelper.GetImageFromURI(uriList);
            }

            #region toBeTested
            //string cssImagesDetail = ".productDetailSubArea a";
            //if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(cssImagesDetail)))
            //{
            //    var elemImageLinks = browser.FindElements(By.CssSelector(cssImagesDetail));

            //    using (var webcli = new WebClient())
            //    {

            //        foreach (var elemImageLink in elemImageLinks)
            //        {

            //            string onclickImageLink = elemImageLink.GetAttribute("onclick");

            //            var image = new ImageData()
            //            {
            //                StartProcessingOn = DateTime.Now,
            //                Filename = "-1",
            //                Filesize = -1,
            //                Width = -1,
            //                Height = -1,
            //                MD5 = "-1",
            //                MIMEType = "-1",
            //                URI = onclickImageLink,
            //                Error = null
            //            };

            //            try
            //            {
            //                string elemImageLinkUri = onclickImageLink.Replace("window.open('", "").Replace("window.open( '", "");
            //                int lastIndex = elemImageLinkUri.IndexOf("'");
            //                image.URI = elemImageLinkUri.Substring(0, lastIndex);

            //                var URI2 = new Uri(image.URI);

            //                image.Filename = URI2.Segments.Last();
            //                image.BinaryData = webcli.DownloadData(image.URI);
            //                image.MD5 = BinaryImageHelper.GetMD5(image.BinaryData);
            //                image.Filesize = image.BinaryData.Length;
            //                var img = Image.FromStream(new MemoryStream(image.BinaryData));
            //                image.Height = img.Height;
            //                image.Width = img.Width;
            //                image.MIMEType = BinaryImageHelper.GetImageFormat(img);
            //            }
            //            catch (Exception e)
            //            {
            //                image.Error = e.Message;
            //            }
            //            image.EndProcessingOn = DateTime.Now;
            //            result.Add(image);
            //        }
            //    }
            //}

            #endregion
            return result;
        }

        private WebScrappedTextual ProcessTextualData()
        {
            var result = new WebScrappedTextual
            {
                StartProcessingOn = DateTime.Now
            };
            string cssModeInfoButton = ".moreInfoButtonArea input";

            Actions actions = new Actions(browser);
            actions
                .MoveToElement(browser.FindElementByName("MoreInfoButton"))
                .Click()
                .Perform();

            //OpenQA.Selenium.Support.UI.WebDriverWait wait =
            //    new WebDriverWait(browser, new TimeSpan(0, 0, 0, 0, 2500));
            //wait.Until(ExpectedConditions.ElementExists(By.Name("MoreInfoButton")));

            /* 
             * Carregar no botão de mais informação e aguardar pelo ajax. 
             * Nota: é necessário carregar via JS caso contrário será interceptado por elemento de UI
             * */
            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(cssModeInfoButton)))
            {
                var inputMoreInfo = browser.FindElement(By.CssSelector(cssModeInfoButton));

                string jsExecute = @"$('[name=""MoreInfoButton""]')[0].click()";
                IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
                js.ExecuteScript(jsExecute);

                //Actions actionsMoreInfo = new Actions(browser);
                //actions
                //    .MoveToElement(browser.FindElementByCssSelector(cssModeInfoButton))
                //    .Click()
                //    .Perform();               

                try
                {
                    WebDriverWait wait2 =
                        new WebDriverWait(browser, new TimeSpan(0, 0, 0, 0, 1500));
                    wait2.Until(ExpectedConditions.ElementExists(By.CssSelector(".body.additionalProductInformation")));


                }
                catch (Exception)
                {
                    throw;
                }
                //var dadosDeProduto = browser.FindElementById("Datos del producto").FindElement(By.ClassName("item-inner")).Text;
            }
            result.ContactName = WebscraperUtils.getTextById(browser, "contactName");
            result.IngredientStatement = WebscraperUtils.getTextById(browser, "ingredientStatement");
            result.MarketingMessage = WebscraperUtils.getTextById(browser, "ingredientStatement");

            var regulatedProductName = WebscraperUtils.getTextById(browser, "regulatedProductName");
            var communicationAddress = WebscraperUtils.getTextById(browser, "communicationAddress");
            var allergenStatement = WebscraperUtils.getTextById(browser, "allergenStatement");

            //Encontrei o GTIN: <input class="EanList" type="hidden" value="7613035659728">
            string EanList = browser.FindElementByClassName("EanList").GetAttribute("value");

            // O selector chaves repetem para netContent e "peso líquido"
            var netContent = WebscraperUtils.getTextById(browser, "netContent");
            var netContent_unitOfMeasure = WebscraperUtils.getTextById(browser, "netContent_unitOfMeasure");

            //TODO: Efectuar parese à tabela de nutrientes:
            // Exemplo: https://www.continente.pt/stores/continente/pt-pt/public/Pages/ProductDetail.aspx?ProductId=5845494(eCsf_RetekProductCatalog_MegastoreContinenteOnline_Continente)
            // Slução: parsing Selenium com xpath 


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
