using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubCreator
{
    class WebReader
    {
        public WebReader()
        {

        }

        public string ReadWebPage(string url)
        {
            string result = "";

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            //Use the XPath to Navigate
            result = doc.DocumentNode.SelectNodes("//*[@id=\"mainContent\"]/article")[0].InnerHtml;
            return result;
        }



    }
}
