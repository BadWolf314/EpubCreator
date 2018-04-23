using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EpubCreator
{
    public abstract class EpubParser
    {
        public abstract string Parse(string url, Epub epub);

        public string SaveImage(string url, Epub epub)
        {
            WebClient webClient = new WebClient();
            string finalLocation = "";
            try
            {
                string imgName = url.Split('/')[url.Split('/').Length - 1].Split('?')[0];
                finalLocation = EpubStructure.IMAGELOCATION.Split('\\')[1] + "\\" + imgName;
                epub.package.Manifest.Item.Add(new Item()
                {
                    Id = imgName, 
                    Href = finalLocation,
                    Mediatype = EpubStructure.GetMediaType(imgName)
                });
                webClient.DownloadFile(url, epub.location + EpubStructure.IMAGELOCATION.Split('\\')[0] + "\\" + finalLocation);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to download: " + url);
                Logger.LogError(ex.Message);
            }
            finally
            {
                webClient.Dispose();
            }
            return finalLocation;
        }

    } //END OF CLASS

    public class CFBParser : EpubParser
    {
        public override string Parse(string url, Epub epub)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            //Use the XPath to Navigate
            HtmlNode node = doc.DocumentNode.SelectNodes("//div[contains(@class, 'postContent')]")[0];
            string bodyText = "";
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                //If it's a special div we don't care and don't want to add it
                if (node.ChildNodes[i].HasClass("spdiv")
                    || node.ChildNodes[i].HasClass("sp-wrap")
                    || node.ChildNodes[i].HasClass("sp-head")
                    || node.ChildNodes[i].HasClass("sp-body")
                    || node.ChildNodes[i].Name == "#text"
                    )
                {
                    //don't do anything
                }
                //Special Case if it's an image
                else if (node.ChildNodes[i].HasClass("crystal-catalog-helper"))
                {

                    try
                    {
                        HtmlNode img = node.ChildNodes[i].ChildNodes[1].ChildNodes[0];
                        string imgSrc = img.Attributes.Where(x => x.Name == "src")
                                .FirstOrDefault().Value.Split('?')[0];
                        if (!imgSrc.StartsWith("https:"))
                        {
                            imgSrc = "https:" + imgSrc;
                        }
                        bodyText += string.Format(EpubStructure.COMMONSMALLIMAGECONTAINER,
                            string.Format(EpubStructure.COMMONIMAGE, SaveImage(imgSrc, epub), "")
                            );
                    }
                    catch(Exception ex)
                    {
                        Logger.LogError("Unable to Parse Image: #" + i + " - " + node.ChildNodes[i].Name);
                    }
                }
                //Paragraphs are special too
                else if (node.ChildNodes[i].Name == "p")
                {
                    bodyText += string.Format(EpubStructure.COMMONPARAGRAPH, node.ChildNodes[i].InnerText);
                }
                //Otherwise just slap the html into the body
                else
                {
                    bodyText += "\n" + node.ChildNodes[i].OuterHtml;
                }
            }
            return bodyText;
        }
    }

    public class WizardParser : EpubParser
    {
        public override string Parse(string url, Epub epub)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            string bodyText = "";
            HtmlNode node = doc.DocumentNode.SelectNodes("//*[@id=\"content-detail-page-of-an-article\"]/html/body")[0];
            for(int i = 0; i < node.ChildNodes.Count; i++)
            {
                if(node.ChildNodes[i].Name == "#text"
                    || node.ChildNodes[i].Name == "aside"
                    || node.ChildNodes[i].Name == "hr"
                    || node.ChildNodes[i].HasClass("module_inline-promo"))
                {
                }
                else if(node.ChildNodes[i].Name == "p")
                {
                    if(node.ChildNodes[i].InnerHtml.Contains("img"))
                    {
                        string imgText = "";
                        foreach(HtmlNode child in node.ChildNodes[i].ChildNodes)
                        {
                            if (child.Name == "img")
                            {
                                imgText += string.Format(EpubStructure.COMMONIMAGE, 
                                    SaveImage(child.Attributes.Where(x => x.Name == "src")
                                    .FirstOrDefault().Value, epub), "");
                            }
                        }
                        bodyText += string.Format(EpubStructure.COMMONSMALLIMAGECONTAINER, imgText);
                    }
                    else
                    {
                        bodyText += string.Format(EpubStructure.COMMONPARAGRAPH, node.ChildNodes[i].InnerHtml.Replace("&mdash;", " - "));
                    }
                }
                else
                {
                    bodyText += "\n" + node.ChildNodes[i].OuterHtml;
                }
            }
            return bodyText;
        }
    }


} //END OF NAMESPACE
