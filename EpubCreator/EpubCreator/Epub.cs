using System.Collections.Generic;
using System.Xml.Serialization;

namespace EpubCreator
{
    public class EpubStructure
    {
        #region Location Strings

        public static string EPUBLOCATION = "EPUB\\";
        public static string CONTENTLOCATION = "CONTENT\\";
        public static string IMAGELOCATION = "images\\";
        public static string CSSLOCATION = "css\\";
        public static string METAINFLOCATION = "META-INF\\";
        public static string CONTAINERLOCATION = "container.xml";
        public static string MIMETYPELOCATION = "mimetype";
        public static string PACKAGELOCATION = "package.opf";
        public static string TITLEPAGELOCATION = "title-page.xhtml";
        public static string TOCLOCATION = "toc.xhtml";
        public static string COVERLOCATION = "cover.xhtml";

        #endregion

        #region Common Templates

        public static string COMMONMIMETYPE = "application/epub+zip";
        public static string COMMONCONTAINER = "<?xml version=\"1.0\"?>"
                                            + "\n<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">"
                                            + "\n\t<rootfiles>"
                                            + "\n\t\t<rootfile full-path=\"CONTENT/package.opf\""
                                            + "\n\t\t\tmedia-type=\"application/oebps-package+xml\" />"
                                            + "\n\t</rootfiles>"
                                            + "\n</container>";
        public static string COMMONTITLEPAGE = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                            + "\n<html xmlns = \"http://www.w3.org/1999/xhtml\" xmlns:epub=\"http://www.idpf.org/2007/ops\">"
                                            + "\n<head>"
                                            + "\n<title></title>"
                                            + "\n<link rel=\"stylesheet\""
                                            + "\ntype=\"text/css\""
                                            + "\nhref=\"css/style.css\" />"
                                            + "\n<link rel = \"stylesheet\""
                                            + "\ntype=\"text/css\""
                                            + "\nhref=\"css/media.css\" />"
                                            + "\n</head>"
                                            + "\n<body>"
                                            + "\n<section id = \"title-page\" class=\"element titlepage\" epub:type=\"titlepage\">"
                                            + "\n<div class=\"title-page-title-subtitle-block\">"
                                            + "\n<h1 class=\"title-page-title\">{0}</h1>"
                                            + "\n<h3 class=\"title-page-subtitle\">{1}</h3>"
                                            + "\n</div>"
                                            + "\n<div class=\"title-page-author-block\">"
                                            + "\n<h2 class=\"title-page-author title-page-author-1\">{2}</h2>"
                                            + "\n</div>"
                                            + "\n</section>"
                                            + "\n</body>"
                                            + "</html>";
        public static string COMMONPAGE = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                            + "\n<html xmlns = \"http://www.w3.org/1999/xhtml\" xmlns:epub=\"http://www.idpf.org/2007/ops\">"
                                            + "\n<head>"
                                            + "\n\t{0}"
                                            + "\n</head> "
                                            + "\n<body>"
                                            + "\n\t{1}"
                                            + "\n</body>"
                                            + "</html>";
        public static string COMMONHEADER = "\n<title>{0}</title>"
                                            + "\n<link rel = \"stylesheet\""
                                            + "\n\ttype= \"text/css\""
                                            + "\n\thref= \"css/style.css\" />"
                                            + "\n\t<link rel= \"stylesheet\""
                                            + "\n\ttype= \"text/css\""
                                            + "\n\thref= \"css/media.css\" />";
        public static string COMMONBODY = "\n<section id=\"{0}\" class=\"element chapter\" epub:type=\"chapter\">"
                                            + "\n<header class=\"heading heading-with-title heading-without-image\" id=\"{0}-heading\">"
                                            + "\n\t<div class=\"chapter-title-subtitle-block chapter-title-block-with-chapter-number\">"
                                            + "\n\t<div class=\"title-block\">"
                                            + "\n\t\t<h1 class=\"title\">{1}</h1>"
                                            + "\n\t</div>"
                                            + "\n\t<h3 class=\"subtitle\">By {3}</h3>"
                                            + "\n\t</div>"
                                            + "\n</header>"
                                            + "\n<div class=\"text text-main\" id=\"{0}-i-text\">"
                                            + "{2}"
                                            + "\n</div>"
                                            + "\n</section>";
        public static string COMMONPARAGRAPH = "\n<p class=\"subsq\">{0}</p>";
        public static string COMMONSMALLIMAGECONTAINER = "\n<figure class=\"inline-image inline-image-size-small inline-image-flow-center\">{0}\n</figure>";
        public static string COMMONIMAGE = "\n<div class=\"inline-image-container\">"
                                            + "\n<img src=\"{0}\" alt=\"{1}\" />"
                                            + "\n</div>";
        public static string COMMONFULLIMAGEWITHCAPTIONCONTAINER = "\n<figure class=\"inline-image inline-image-kind-photograph inline-image-size-full inline-image-flow-center inline-image-aspect-wide inline-image-with-caption\">"
                                            + "{0}"
                                            + "\n<figcaption class=\"inline-image-caption\">{1}</figcaption>"
                                            + "\n</figure>";
        public static string COMMONTOCITEM = "<li id=\"{0}\" class=\"element-title has-no-children\">"
                                            + "\n<a href=\"{2}\">{3}</a>"
                                            + "\n</li>";
        public static string COMMONTOCBODY = "<section id=\"toc\" epub:type=\"toc\">"
                                            + "\n<h3 class=\"toc-title\">Contents</h3>"
                                            + "\n<nav epub:type=\"toc\">"
                                            + "\n<ol id=\"contents\">"
                                            + "\n{0}"
                                            + "\n</ol>"
                                            + "\n</nav>"
                                            + "\n</section>";
        public static string COMMONHR = "<figure class=\"inline-image inline-image-kind-photograph\">"
                                            + "<div class=\"inline-image-container\">"
                                            + "<img src=\"images/break-section-side.png\" alt=\"\" />"
                                            + "</div>"
                                            + "</figure>";
        public static string COMMONCHAPTERNUMBER = "<header class=\"heading heading-with-title heading-without-image\">"
                                            + "<div class=\"chapter-title-subtitle-block chapter-title-block-with-chapter-number\">"
                                            + "<div class=\"chapter-number-block\">"
                                            + "<h2 class=\"chapter-number\">{0}</h2>"
                                            + "</div>"
                                            + "</div>"
                                            + "</header>";
        public static string COMMONCOVER = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                            + "\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:epub=\"http://www.idpf.org/2007/ops\">"
                                            + "\n<head>"
                                            + "\n<title>{0}</title>"
                                            + "\n<style type=\"text/css\">"
                                            + "\nimg.cover-image {{"
                                            + "\nmax-width: 100%;"
                                            + "\nmax-height: 100%;"
                                            + "\ndisplay: block;"
                                            + "\nmargin-left: auto;"
                                            + "\nmargin-right: auto;"
                                            + "\n}}"
                                            + "\n</style>"
                                            + "\n</head>"
                                            + "\n<body class=\"cover\">"
                                            + "\n<section id = \"cover-image\" epub:type=\"cover\">"
                                            + "\n<img src=\"{1}\""
                                            + "\nalt=\"{0}\""
                                            + "\nclass=\"cover-image\" />"
                                            + "\n</section>"
                                            + "\n</body>"
                                            + "\n</html>";

        #endregion

        /// <summary>
        /// Gets the media type for the requested file
        /// </summary>
        /// <param name="file">file to determine the media type of</param>
        /// <returns>the media type as a string</returns>
        public static string GetMediaType(string file)
        {
            string mediaType = "";

            switch(file.Split('.')[file.Split('.').Length - 1])
            {
                case "jpg":
                case "jpeg":
                    mediaType = "image/jpeg";
                    break;
                case "png":
                    mediaType = "image/png";
                    break;
                case "svg":
                    mediaType = "image/svg+xml";
                    break;
                case "css":
                    mediaType = "text/css";
                    break;
                default:
                    mediaType = "application/xhtml+xml";
                    break;
            }

            return mediaType;
        }
    }

    /// <summary>
    /// Epub class structure (inputs from JSON format)
    /// </summary>
    public class Epub
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string author { get; set; }
        public string cover { get; set; }
        public List<Page> pages { get; set; }
        public string location { get; set; }
        public List<string> assets { get; set; }
        public List<string> toc { get; set; }
        public Package package { get; set; }

        public Epub() {
            package = new Package();
            toc = new List<string>();
            pages = new List<Page>();
            assets = new List<string>();
        }

        #region Helper Functions

        /// <summary>
        /// Sanatize the id
        /// </summary>
        /// <param name="id">id with potentially bad characters</param>
        /// <returns>sanatized id</returns>
        private string SanitizeId(string id)
        {
            if (char.IsDigit(id[0]))
            {
                id = "zz" + id;
            }
            return id
                .Replace('.', '-')
                .Replace(',', '-')
                .Replace('+', '-')
                ;
        }


        /// <summary>
        /// Add an object to the spine
        /// </summary>
        /// <param name="idref">idref to use</param>
        public void AddToSpine(string idref)
        {
            package.Spine.Itemref.Add(new Itemref()
            {
                Idref = SanitizeId(idref)
            });
            toc.Add(idref);
        }

        /// <summary>
        /// Add an object to the Mainifest
        /// </summary>
        /// <param name="id">id to use</param>
        /// <param name="href">link to use</param>
        public void AddToManifest(string id, string href, string properties = "")
        {
            Item item = new Item()
            {
                Id = SanitizeId(id),
                Href = href.Replace("\\", "/"),
                Mediatype = EpubStructure.GetMediaType(id)
            };

            if(!string.IsNullOrEmpty(properties))
            {
                item.Properties = properties;
            }

            if(!package.Manifest.Item.Exists(x => x.Id == item.Id && x.Href == item.Href && x.Mediatype == item.Mediatype))
            {
                package.Manifest.Item.Add(item);
            }
        }

        #endregion
    }

    public class Page
    {
        public string url { get; set; }
        public string title { get; set; }
        public string parser { get; set; }
        public string author { get; set; }
        public Page() { }
    }

    #region PackageXML

    [XmlRoot(ElementName = "identifier", Namespace = "http://purl.org/dc/elements/1.1/")]
    public class Identifier
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
    public class Creator
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "meta", Namespace = "http://www.idpf.org/2007/opf")]
    public class Meta
    {
        [XmlAttribute(AttributeName = "refines")]
        public string Refines { get; set; }
        [XmlAttribute(AttributeName = "property")]
        public string Property { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "metadata", Namespace = "http://www.idpf.org/2007/opf")]
    public class Metadata
    {
        [XmlElement(ElementName = "identifier", Namespace = "http://purl.org/dc/elements/1.1/")]
        public Identifier Identifier { get; set; }
        [XmlElement(ElementName = "language", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Language { get; set; }
        [XmlElement(ElementName = "title", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Title { get; set; }
        [XmlElement(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
        public Creator Creator { get; set; }
        [XmlElement(ElementName = "meta", Namespace = "http://www.idpf.org/2007/opf")]
        public List<Meta> Meta { get; set; }
        [XmlAttribute(AttributeName = "dc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dc { get; set; }
    }

    [XmlRoot(ElementName = "item", Namespace = "http://www.idpf.org/2007/opf")]
    public class Item
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "properties")]
        public string Properties { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "media-type")]
        public string Mediatype { get; set; }
    }

    [XmlRoot(ElementName = "manifest", Namespace = "http://www.idpf.org/2007/opf")]
    public class Manifest
    {
        [XmlElement(ElementName = "item", Namespace = "http://www.idpf.org/2007/opf")]
        public List<Item> Item { get; set; }
        public Manifest()
        {
            Item = new List<Item>();
        }
    }

    [XmlRoot(ElementName = "itemref", Namespace = "http://www.idpf.org/2007/opf")]
    public class Itemref
    {
        [XmlAttribute(AttributeName = "idref")]
        public string Idref { get; set; }
    }

    [XmlRoot(ElementName = "spine", Namespace = "http://www.idpf.org/2007/opf")]
    public class Spine
    {
        [XmlElement(ElementName = "itemref", Namespace = "http://www.idpf.org/2007/opf")]
        public List<Itemref> Itemref { get; set; }
        public Spine()
        {
            Itemref = new List<Itemref>();
        }
    }

    [XmlRoot(ElementName = "package", Namespace = "http://www.idpf.org/2007/opf")]
    public class Package
    {
        [XmlElement(ElementName = "metadata", Namespace = "http://www.idpf.org/2007/opf")]
        public Metadata Metadata { get; set; }
        [XmlElement(ElementName = "manifest", Namespace = "http://www.idpf.org/2007/opf")]
        public Manifest Manifest { get; set; }
        [XmlElement(ElementName = "spine", Namespace = "http://www.idpf.org/2007/opf")]
        public Spine Spine { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "unique-identifier")]
        public string Uniqueidentifier { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        public Package()
        {
            Metadata = new Metadata();
            Manifest = new Manifest();
            Spine = new Spine();
        }
    }

    #endregion

}//END Of NAMESPACE
