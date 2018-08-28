using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace EpubCreator
{
    /// <summary>
    /// Basic Parser class
    /// </summary>
    public abstract class EpubParser
    {
        public string RootNode;
        public Epub epub;

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="epub"></param>
        /// <returns></returns>
        public string Parse(string url, Epub epub)
        {
            this.epub = epub;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            var bodyText = new StringBuilder();
            HtmlNode nodes = doc.DocumentNode.SelectNodes(RootNode)[0];

            foreach (HtmlNode node in nodes.ChildNodes)
            {
                if (IgnoreNode(node))
                {
                }
                else if(DecklistNode(node))
                {
                    bodyText.Append(DecklistParser(node));
                }
                else if (ImageNode(node))
                {
                    bodyText.Append(ImageParser(node));
                }
                else if (ParagraphNode(node))
                {
                    bodyText.Append(ParagraphParser(node));
                }
                else if (HrNode(node))
                {
                    bodyText.Append(HrParser(node));
                }
                else if(ListNode(node))
                {
                    bodyText.Append(ListParser(node));
                }
                else if(ScriptNode(node))
                {
                    bodyText.Append(ScriptParser(node));
                }
                else
                {
                    bodyText.Append(DefaultParser(node));
                }
            }

            return bodyText.ToString();
        }

        #region Node Types

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool DecklistNode(HtmlNode node)
        {
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool IgnoreNode(HtmlNode node)
        {
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool ImageNode(HtmlNode node)
        {
            return node.InnerHtml.Contains("img") || node.Name == "img";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool ParagraphNode(HtmlNode node)
        {
            return node.Name == "p";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool HrNode(HtmlNode node)
        {
            return node.Name == "hr";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool ListNode(HtmlNode node)
        {
            return node.Name == "ol" || node.Name == "ul";
        }

        public virtual bool ScriptNode(HtmlNode node)
        {
            return node.Name == "script";
        }

        #endregion

        #region Type Parsers

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual string ParagraphParser(HtmlNode node)
        {
            return string.Format(EpubStructure.COMMONPARAGRAPH, Sanitize(node.InnerText));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual string HrParser(HtmlNode node)
        {
            return EpubStructure.COMMONHR;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual string DecklistParser(HtmlNode node)
        {
            return "\n" + node.OuterHtml;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual string DefaultParser(HtmlNode node)
        {
            return "\n" + Sanitize(node.OuterHtml)
                    ;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual string ImageParser(HtmlNode node)
        {
            if (node.Name == "img")
            {
                return BuildImage(node);
            }
            else
            {
                var returnText = new StringBuilder();
                foreach (HtmlNode img in node.SelectNodes(".//img"))
                {
                    returnText.Append(BuildImage(img));
                }
                return returnText.ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual string ScriptParser(HtmlNode node)
        {
            return "";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual string ListParser(HtmlNode node)
        {
            string returnText = "";
            returnText += string.Format("<{0}>", node.Name);
            foreach(HtmlNode li in node.SelectNodes(".//li"))
            {
                returnText += "<li>" + Sanitize(li.InnerHtml) + "</li>";
            }
            returnText += string.Format("</{0}>", node.Name);
            return returnText;
        }

        public virtual string Sanitize(string html)
        {
            return html
                .Replace("&mdash;", " - ")
                .Replace("&ndash;", " - ")
                .Replace("&nbsp;", " ")
                .Replace("&eacute;", "é")
                .Replace("&agrave;", "à")
                .Replace("<br>", "")
                .Replace("<nbsp>", " ")
                .Replace("</nbsp>", " ")
                .Replace("&ccedil;", "ç")
                .Replace("&iuml;", "ï")
                .Replace("<p>", "")
                .Replace("</p>", "")
                .Replace("&AElig;", "Æ")
                .Replace("&hellip;", "…")
                .Replace("&rsquo;", "'")
                .Replace("&alpha;", "ALPHA")
                .Replace("&beta;", "BETA")
                .Replace("&omega;", "OMEGA")
                .Replace("&oacute;", "Ó")
                ;
        }

        #endregion

        #region Image Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public virtual string BuildImage(HtmlNode img)
        {
            string imgSrc = img.Attributes.FirstOrDefault(x => x.Name == "src").Value.Split('?')[0];

            string imgName = imgSrc.Split('/')[imgSrc.Split('/').Length - 1].Split('?')[0].Replace("%", "");

            return string.Format(EpubStructure.COMMONSMALLIMAGECONTAINER,
                string.Format(EpubStructure.COMMONIMAGE, SaveImage(epub, imgSrc, imgName), "")
                );
        }

        /// <summary>
        /// Save the image locally and then add to the epub manifest
        /// </summary>
        /// <param name="epub">The epub to add the image to</param>
        /// <param name="url">The address the image is located at</param>
        /// <param name="imgName"></param>
        /// <returns></returns>
        public static string SaveImage(Epub epub, string url, string imgName)
        {
            WebClient webClient = new WebClient();
            try
            {
                if (!File.Exists(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION + imgName))
                {
                    Retry(1, TimeSpan.FromSeconds(60), () =>
                    {
                        webClient.DownloadFile(url,
                          epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION + imgName);
                    });
                }
                epub.AddToManifest(imgName, EpubStructure.IMAGELOCATION + imgName);
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
            return (EpubStructure.IMAGELOCATION + imgName).Replace("\\", "/");
        }

        #endregion

        /// <summary>
        /// Simple Retry Logic
        /// </summary>
        /// <param name="retryTimes">Number of times to retry</param>
        /// <param name="retryMillisInterval">Milliseconds to wait</param>
        /// <param name="retryAction">What to do</param>
        public static void Retry(int retryTimes, TimeSpan retryMillisInterval, Action retryAction)
        {
            var actionDone = false;
            for (int i = 0; i < retryTimes && !actionDone; ++i)
            {
                try
                {
                    retryAction?.Invoke();
                    actionDone = true;
                }
                catch (Exception e)
                {
                    if (i == retryTimes - 1)
                    {
                        Logger.LogError("Retries failed, now letting throw out");
                        throw;
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

    } //END OF CLASS

    /// <summary>
    ///
    /// </summary>
    public class SCGParser : EpubParser
    {
        public SCGParser()
        {
            RootNode = "//*[@id='article_content']";
        }
    } //END OF CLASS

    /// <summary>
    ///
    /// </summary>
    public class CFBParser : EpubParser
    {
        public CFBParser()
        {
            RootNode = "//div[contains(@class, 'postContent')]";
        }

        #region Node Types

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool IgnoreNode(HtmlNode node)
        {
            return node.HasClass("spdiv")
                    || node.HasClass("sp-wrap")
                    || node.HasClass("sp-head")
                    || node.HasClass("sp-body")
                    || node.Name == "#text";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool DecklistNode(HtmlNode node)
        {
            return node.HasClass("crystal-catalog-helper-list");
        }

        #endregion

        #region Type Parsers

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override string ImageParser(HtmlNode node)
        {
            if(!(node.Name == "a" && node.Attributes.FirstOrDefault(x => x.Name == "href")!= null && node.Attributes.FirstOrDefault(x => x.Name == "href").Value.Contains("previews")))
            {
                return base.ImageParser(node);
            }

            return "";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override string DecklistParser(HtmlNode node)
        {
            var bodyText = new StringBuilder();
            foreach (HtmlNode subList in node.SelectNodes(".//div[contains(@class, 'crystal-catalog-helper-sublist')]"))
            {
                bodyText.Append("<h3>" + subList.SelectNodes(".//span[contains(@class, 'crystal-catalog-helper-subtitle')]")[0].InnerText + "</h3><ul>");
                foreach (HtmlNode listItem in subList.SelectNodes(".//a[contains(@class, 'crystal-catalog-helper-list-item')]"))
                {
                    bodyText.Append("<li>" + listItem.InnerText + "</li>");
                }
                bodyText.Append("</ul>");
            }
            return bodyText.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override string DefaultParser(HtmlNode node)
        {
            if (node.HasClass("twitter-tweet"))
                return HrParser(node) + ParagraphParser(node) + HrParser(node);
            return base.DefaultParser(node);
        }

        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public override string BuildImage(HtmlNode img)
        {
            string imgSrc = img.Attributes.FirstOrDefault(x => x.Name == "src")
                                          .Value.Split('?')[0];
            if (!imgSrc.StartsWith("https:") && !imgSrc.StartsWith("http:"))
            {
                imgSrc = "https:" + imgSrc;
            }
            string imgName = imgSrc.Split('/')[imgSrc.Split('/').Length - 1].Split('?')[0].Replace("%", "");
            return string.Format(EpubStructure.COMMONSMALLIMAGECONTAINER,
                string.Format(EpubStructure.COMMONIMAGE, SaveImage(epub, imgSrc, imgName), "")
                );
        }
    } //END OF CLASS

    /// <summary>
    ///
    /// </summary>
    public class WizardParser : EpubParser
    {
        public WizardParser()
        {
            RootNode = "//*[@id=\"content-detail-page-of-an-article\"]/html/body";
        }

        #region Node Types

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool IgnoreNode(HtmlNode node)
        {
            return node.Name == "#text"
                    || node.Name == "aside"
                    || node.HasClass("module_inline-promo")
                    || node.InnerHtml.Contains("module_inline-promo");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool HrNode(HtmlNode node)
        {
            return base.HrNode(node) || node.SelectNodes(".//hr") != null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool FullImageNode(HtmlNode node)
        {
            return node.HasClass("figure-wrapper");
        }

        public override bool DecklistNode(HtmlNode node)
        {
            return node.HasClass("bean_block_deck_list")
                || node.HasClass("bean--wiz-content-deck-list");
        }

        public override bool ParagraphNode(HtmlNode node)
        {
            return base.ParagraphNode(node) || (node.Name == "blockquote" && node.SelectNodes(".//p") != null);
        }

        #endregion

        #region Type Parsers

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override string ParagraphParser(HtmlNode node)
        {
            string bodyText = "";
            if (node.Attributes["align"] != null && node.Attributes["align"].Value == "center")
            {
                bodyText += string.Format(EpubStructure.COMMONCHAPTERNUMBER,
                    Sanitize(node.InnerText).ToUpper());
            }
            else if (!node.InnerHtml.Contains("target=\"_blank\""))
            {
                bodyText += base.ParagraphParser(node);
            }
            return bodyText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override string DefaultParser(HtmlNode node)
        {
            if (node.InnerHtml.Contains("<p>"))
            {
                var bodyText = new StringBuilder();
                foreach (HtmlNode p in node.SelectNodes(".//p"))
                {
                    bodyText.Append(ParagraphParser(p));
                }
                return bodyText.ToString();
            }
            else
            {
                return base.DefaultParser(node);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override string ImageParser(HtmlNode node)
        {
            if (FullImageNode(node))
            {
                string imgText;
                var builder = new StringBuilder();
                foreach (HtmlNode child in node.SelectNodes(".//img"))
                {
                    builder.Append(BuildImage(child));
                }
                imgText = builder.ToString();
                var caption = Sanitize(node.SelectNodes(".//figcaption")[0].InnerText);
                return string.Format(EpubStructure.COMMONFULLIMAGEWITHCAPTIONCONTAINER, imgText, caption);
            }
            else
            {
                return string.Format(EpubStructure.COMMONSMALLIMAGECONTAINER, base.ImageParser(node));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override string DecklistParser(HtmlNode node)
        {
            string bodyText = "";
            bodyText += base.HrParser(node);
            bodyText += "<h3>" + Sanitize(node.SelectNodes(".//span[contains(@class, 'deck-meta')]//h4").FirstOrDefault().InnerHtml) + "</h3>";

            foreach (HtmlNode sortedDiv in node.SelectNodes(".//div[contains(@class, 'sorted-by-overview-container')]//div[contains(@class, 'clearfix')]"))
            {
                bodyText += "<h4>" + Sanitize(sortedDiv.SelectNodes(".//h5").FirstOrDefault().InnerHtml) + "</h4>";
                foreach (HtmlNode row in sortedDiv.SelectNodes(".//span[contains(@class, 'row')]"))
                {
                    bodyText += base.ParagraphParser(row);
                }
            }
            bodyText += base.HrParser(node);
            return bodyText;
        }

        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public override string BuildImage(HtmlNode img)
        {
            string imgUrl = img.Attributes.FirstOrDefault(x => x.Name == "src")
                                          .Value.Replace("&amp;", "&");
            string imgName = imgName = imgUrl.Split('/')[imgUrl.Split('/').Length - 1].Split('?')[0].Replace("%", "");

            if (imgName.EndsWith(".ashx"))
            {
                imgName = imgUrl.Split('?')[imgUrl.Split('?').Length - 1];
                imgName = imgName.Split('&').FirstOrDefault(x => x.StartsWith("name=") || x.StartsWith("multiverseid=")).Replace("%", "").Substring(5) + ".png";
            }
            return string.Format(EpubStructure.COMMONIMAGE,
                SaveImage(epub, imgUrl, imgName), "");
        }
    }

} //END OF NAMESPACE
