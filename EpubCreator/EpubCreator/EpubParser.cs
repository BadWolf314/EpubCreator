using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace EpubCreator
{
    public abstract class EpubParser
    {
        public abstract string Parse(string url, Epub epub);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryTimes"></param>
        /// <param name="retryMillisInterval"></param>
        /// <param name="retryAction"></param>
        public static void Retry(int retryTimes, TimeSpan retryMillisInterval, Action retryAction)
        {
            bool actionDone = false;
            for (int i = 0; i < retryTimes && !actionDone; ++i)
            {
                try
                {
                    retryAction();
                    actionDone = true;
                }
                catch (Exception e)
                {
                    if (i == retryTimes - 1)
                    {
                        Logger.LogError("Retries failed, now letting throw out");
                        throw e;
                    }
                    else
                    {
                        Logger.LogInfo("Will retry, here's the stack trace for the fail");
                        Logger.LogInfo(e.Message);
                        Thread.Sleep(retryMillisInterval);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="epub"></param>
        /// <returns></returns>
        public string SaveImage(string url, Epub epub)
        {
            WebClient webClient = new WebClient();
            string imgName = "";
            try
            {
                imgName = url.Split('/')[url.Split('/').Length - 1].Split('?')[0].Replace("%", "");
                if(imgName.EndsWith(".ashx"))
                {
                    imgName = url.Split('?')[url.Split('?').Length - 1];
                    imgName = imgName.Split('&').Where(x => x.StartsWith("name=")).FirstOrDefault().Substring(5) + ".jpg";
                }
                epub.package.Manifest.Item.Add(new Item()
                {
                    Id = imgName, 
                    Href = EpubStructure.IMAGELOCATION + imgName,
                    Mediatype = EpubStructure.GetMediaType(imgName)
                });

                EpubParser.Retry(3, TimeSpan.FromSeconds(60), () =>
                {
                    webClient.DownloadFile(url,
                      epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION + imgName);
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to download: " + url + " - " + imgName);
                Logger.LogError(ex.Message);
            }
            finally
            {
                webClient.Dispose();
            }
            return EpubStructure.IMAGELOCATION + imgName;
        }

    } //END OF CLASS

    /// <summary>
    /// 
    /// </summary>
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
                else if(node.ChildNodes[i].HasClass("crystal-catalog-helper-list"))
                {
                    foreach(HtmlNode subList in node.ChildNodes[i].SelectNodes(".//div[contains(@class, 'crystal-catalog-helper-sublist')]"))
                    {
                        bodyText += "<h3>" + subList.SelectNodes(".//span[contains(@class, 'crystal-catalog-helper-subtitle')]")[0].InnerText + "</h3><ul>";
                        foreach(HtmlNode listItem in subList.SelectNodes(".//a[contains(@class, 'crystal-catalog-helper-list-item')]"))
                        {
                            bodyText += "<li>" + listItem.InnerText + "</li>";
                        }
                        bodyText += "</ul>";
                    }
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

    /// <summary>
    /// 
    /// </summary>
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
                else if(node.ChildNodes[i].InnerHtml.Contains("img"))
                {
                    string imgText = "";
                    foreach (HtmlNode child in node.ChildNodes[i].SelectNodes(".//img"))
                    {
                        imgText += string.Format(EpubStructure.COMMONIMAGE,
                            SaveImage(child.Attributes.Where(x => x.Name == "src")
                            .FirstOrDefault().Value.Replace("&amp;", "&"), epub), "");
                    }
                    bodyText += string.Format(EpubStructure.COMMONSMALLIMAGECONTAINER, imgText);
                }
                else if(node.ChildNodes[i].Name == "p")
                {
                    bodyText += string.Format(EpubStructure.COMMONPARAGRAPH, node.ChildNodes[i].InnerHtml.Replace("&mdash;", " - "));
                }
                else
                {
                    bodyText += "\n" + node.ChildNodes[i].OuterHtml;
                }
            }
            return bodyText;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MagicStoryParser : EpubParser
    {
        public override string Parse(string url, Epub epub)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            string bodyText = "";
            HtmlNode node = doc.DocumentNode.SelectNodes("//*[@id=\"content-detail-page-of-an-article\"]/html/body")[0];
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Name == "#text"
                    || node.ChildNodes[i].Name == "aside"
                    || node.ChildNodes[i].HasClass("module_inline-promo")
                    || node.ChildNodes[i].InnerHtml.Contains("module_inline-promo"))
                {
                }
                else if (node.ChildNodes[i].HasClass("figure-wrapper"))
                {
                    string imgText = "";
                    foreach (HtmlNode child in node.ChildNodes[i].SelectNodes(".//img"))
                    {
                        imgText += string.Format(EpubStructure.COMMONIMAGE,
                            SaveImage(child.Attributes.Where(x => x.Name == "src")
                            .FirstOrDefault().Value.Replace("&amp;", "&"), epub), "");
                    }
                    string caption = node.ChildNodes[i].SelectNodes(".//figcaption")[0].InnerText;
                    bodyText += string.Format(EpubStructure.COMMONFULLIMAGEWITHCAPTIONCONTAINER, imgText, caption);
                }
                else if (node.ChildNodes[i].Name == "p")
                {
                    if(node.ChildNodes[i].Attributes["align"] != null && node.ChildNodes[i].Attributes["align"].Value == "center")
                    {
                        bodyText += string.Format(EpubStructure.COMMONCHAPTERNUMBER, 
                            node.ChildNodes[i].InnerText.Substring(0, node.ChildNodes[i].InnerText.Length - 1).ToUpper());
                    }
                    else if (!node.ChildNodes[i].InnerHtml.Contains("target=\"_blank\""))
                    {
                        bodyText += string.Format(EpubStructure.COMMONPARAGRAPH, node.ChildNodes[i].InnerHtml
                            .Replace("&mdash;", " - ").Replace("&nbsp;", " ")
                            .Replace("<br>", ""));
                    }
                }
                else if(node.ChildNodes[i].Name == "hr")
                {
                    bodyText += EpubStructure.COMMONHR;
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
