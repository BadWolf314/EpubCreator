using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubCreator
{
    public abstract class EpubParser
    {

        public abstract string Parse(string url);

    } //END OF CLASS

    public class CFBParser : EpubParser
    {
        public override string Parse(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            //Use the XPath to Navigate
            HtmlNode node = doc.DocumentNode.SelectNodes("//div[contains(@class, 'postContent')]")[0];
            string bodyText = "";
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].HasClass("spdiv")
                    || node.ChildNodes[i].HasClass("sp-wrap")
                    || node.ChildNodes[i].HasClass("sp-head")
                    || node.ChildNodes[i].HasClass("sp-body")
                    )
                {
                    Logger.LogInfo("Special: " + i);
                    //don't do anything
                }
                else if (node.ChildNodes[i].HasClass("crystal-catalog-helper"))
                {
                    Logger.LogInfo("Image: " + i);
                    HtmlNode img = node.ChildNodes[i].ChildNodes[1].ChildNodes[0];
                    bodyText += string.Format(EpubStructure.COMMONSMALLIMAGECONTAINER,
                        string.Format(EpubStructure.COMMONIMAGE, img.Attributes.Where(x => x.Name == "src")
                            .FirstOrDefault().Value.Replace("//crystalcommerce-", "https://crystalcommerce-"), "")
                        );
                }
                else if (node.ChildNodes[i].Name == "p")
                {
                    bodyText += string.Format(EpubStructure.COMMONPARAGRAPH, node.ChildNodes[i].InnerText);
                }
                else if (node.ChildNodes[i].Name == "#text")
                {
                    //Don't do anything for now
                }
                else
                {
                    Logger.LogInfo("Else: " + i);
                    bodyText += "\n" + node.ChildNodes[i].OuterHtml;
                }
            }
            return bodyText;
        }
    }


} //END OF NAMESPACE
