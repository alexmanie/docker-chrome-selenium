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
    public class WebScrapperRecheioPT : IWebScrapper
    {
        /* A pesquisa do Recheio.PT funciona com o código interno de produto que é exibido na página do produto.
          
          Uma vez que a pesquisa não oferece resultados apenas por exact match, é necessário um passo posterior 
          para seleccionar qual dos produtos é o que realmente se está à procura 

            Pesquisa por Código "60710"

            60710 https://www.recheio.pt/catalogo/catalogsearch/result/?q=60710&order=relevance&dir=desc&cat=745

            Dois produtos encontrados:
            60710 https://www.recheio.pt/catalogo/produtos/bebidas/cerveja/cerveja-c-alc-super-bock-33cl-tr.html
            760710 https://www.recheio.pt/catalogo/passa-uva-thompson-s-gra-150gr.html

            Quando se procura pelo código 710, então são oferecidos 46 resultados, sendo necessário paginar.
            710 https://www.recheio.pt/catalogo/catalogsearch/result/?order=relevance&dir=desc&cat=745&q=710
            De forma a evitar este custo, vou apenas pesquisar na primeira página e cancelar o Find caso o resultado não conste

*/
        public static string LinkPrefix_SKUSearch = "https://www.recheio.pt/catalogo/catalogsearch/result/?order=relevance&dir=desc&cat=745&q=";
        public static string LinkPrefix = "recheio.pt";

        public static string Company_Name = "recheio";

        RemoteWebDriver browser;
        public WebScrapperRecheioPT(RemoteWebDriver remoteWebDriver)
        {
            browser = remoteWebDriver;
        }

        public bool CanWebscrape(string companyGLN, string productURL)
        {
            var result = companyGLN == CompanyConstants.CompanyGLNs.RECHEIRO_PT;

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
            string codeToMatch = "";
            if (!string.IsNullOrWhiteSpace(internalCode))
            {
                browser.Url = LinkPrefix_SKUSearch + internalCode;
                codeToMatch = internalCode;
            }
            else
            {
                var gtinCleanse = CleanGtinFromLeadingZeroes(gtin);
                browser.Url = LinkPrefix_SKUSearch + gtin;
                codeToMatch = gtinCleanse;
            }

            //go to url and get result
            browser.Navigate();

            string cssResultItemLinks = "li.item";
            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(cssResultItemLinks)))
            {
                // O elemento verificado só consta quando não existem resultados
                var elemsResultItemLinks = browser.FindElements(By.CssSelector(cssResultItemLinks));

                foreach (var elem in elemsResultItemLinks)
                {
                    string id = elem.GetAttribute("id");
                    if (id.StartsWith("catalog-li-") && id == "catalog-li-" + codeToMatch)
                    {
                        var pi = elem.FindElements(By.CssSelector("a.product-image")).FirstOrDefault();
                        var piLink = pi.GetAttribute("href");
                        result.Add(piLink);
                    }
                }

                if (!result.Any() && elemsResultItemLinks.Count == 1)
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
                return new WebScrappedData() { IsSuccess = false, ErrorMessage = "Product was not found in Recheio.pt", ProductRealName = description };
            }
            else
            {
                return Webscrape(urlList.FirstOrDefault());
            }
        }

        public List<string> GetTestData()
        {
            return new List<string>() {
                @"https://www.recheio.pt/catalogo/passa-uva-thompson-s-gra-150gr.html",
                @"https://www.recheio.pt/catalogo/cerveja-c-alc-super-bock-33cl-tr.html"
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
            string cssImageLinks = ".product-image-gallery img.zoomImg";
            if (WebscraperUtils.IsElementPresent(browser, By.CssSelector(cssImageLinks)))
            {
                var elemImageLinks = browser.FindElements(By.CssSelector(cssImageLinks));
                var uriList = elemImageLinks.Select(x => x.GetAttribute("href"));
                result = BinaryImageHelper.GetImageFromURI(uriList);
            }

            return result;
        }

        private WebScrappedTextual ProcessTextualData()
        {
            var result = new WebScrappedTextual();
            result.StartProcessingOn = DateTime.Now;

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
