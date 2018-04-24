using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace EpubCreator
{
    class EpubSaver
    {
        public const string  EXTENSION = ".epub";
        public Epub epub;

        /// <summary>
        /// 
        /// </summary>
        public EpubSaver(Epub epub)
        {
            this.epub = epub;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        public void CreateEpubFile()
        {
            File.Delete(epub.location + epub.title.Replace(" ", "") + EXTENSION);
            ZipFile.CreateFromDirectory(epub.location + EpubStructure.EPUBLOCATION, epub.location + epub.title.Replace(" ", "") + EXTENSION);
        }

        public void PopulateStructure()
        {
            CleanUpOldStructure();
            CreateDirectories();
            CreateContainerAndMimetype();
            if(!string.IsNullOrEmpty(epub.cover))
            {
                CreateCover();
            }
            CreateTitlePage();
            CreateTOC();
            foreach(string asset in epub.assets)
            {
                switch(asset.Split('.')[asset.Split('.').Length - 1])
                {
                    case "jpg":
                    case "jpeg":
                    case "png":
                    case "svg":
                        CreateImage(asset);
                        break;
                    case "css":
                        CreateCSS(asset);
                        break;
                    default:
                        break;
                }
            }
            foreach(Page page in epub.pages)
            {
                CreatePage(page);
            }
            CreatePackage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        public void CleanUpOldStructure()
        {
            try
            {
                Directory.Delete(epub.location, true);

            }
            catch (Exception ex)
            {
                Logger.LogError("Unable to delete " + epub.location);
                Logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        public void CreateDirectories()
        {
            Directory.CreateDirectory(epub.location);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.METAINFLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.CSSLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        public void CreateContainerAndMimetype()
        {
            StreamWriter writer = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.METAINFLOCATION + EpubStructure.CONTAINERLOCATION));
            writer.WriteLine(EpubStructure.COMMONCONTAINER);
            writer.Close();

            writer = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.MIMETYPELOCATION));
            writer.WriteLine(EpubStructure.COMMONMIMETYPE);
            writer.Close();
        }

        public void CreateCover()
        {
            StreamWriter writer = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.COVERLOCATION));
            writer.WriteLine(
                string.Format(EpubStructure.COMMONCOVER, epub.title, EpubStructure.IMAGELOCATION + EpubStructure.COVERLOCATION));
            writer.Close();
            File.Copy(epub.cover, epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION + EpubStructure.COVERLOCATION, true);
            AddToSpine(EpubStructure.COVERLOCATION);
            AddToManifest(EpubStructure.COVERLOCATION, EpubStructure.COVERLOCATION);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        public void CreateTitlePage()
        {
            StreamWriter writer = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.TITLEPAGELOCATION));
            writer.WriteLine(string.Format(EpubStructure.COMMONTITLEPAGE, epub.title, epub.subtitle, epub.author));
            writer.Close();
            AddToSpine(EpubStructure.TITLEPAGELOCATION);
            AddToManifest(EpubStructure.TITLEPAGELOCATION, EpubStructure.TITLEPAGELOCATION);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        /// <param name="css"></param>
        public void CreateCSS(string css)
        {
            string cssFileName = css.Split('\\')[css.Split('\\').Length - 1];
            File.Copy(css, epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.CSSLOCATION + cssFileName, true);
            AddToManifest(cssFileName, EpubStructure.CSSLOCATION + cssFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="img"></param>
        public void CreateImage(string img)
        {
            string imgFileName = img.Split('\\')[img.Split('\\').Length - 1];
            File.Copy(img, epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION + imgFileName, true);
            AddToManifest(imgFileName, EpubStructure.IMAGELOCATION + imgFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        /// <param name="page"></param>
        public void CreatePage(Page page)
        {
            Logger.LogInfo("Parsing " + page.title + " with " + page.parser + "Parser");

            EpubParser parser = (EpubParser)Activator.CreateInstance(Type.GetType("EpubCreator." + page.parser + "Parser"));
            string bodyText = parser.Parse(page.url, epub);

            string pageTitleNoSpaces = page.title.Replace(" ", "") + ".xhtml";

            StreamWriter writer = new StreamWriter(File.Create(epub.location + EpubStructure.EPUBLOCATION +
                EpubStructure.CONTENTLOCATION + pageTitleNoSpaces));
            writer.WriteLine(
            string.Format(EpubStructure.COMMONPAGE,
                string.Format(EpubStructure.COMMONHEADER, epub.title),
                string.Format(EpubStructure.COMMONBODY, pageTitleNoSpaces, page.title, bodyText, page.author)
            ));
            writer.Close();

            AddToManifest(pageTitleNoSpaces, pageTitleNoSpaces);
            AddToSpine(pageTitleNoSpaces);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateTOC()
        {
            StreamWriter writer = new StreamWriter(File.Create(epub.location + EpubStructure.EPUBLOCATION +
                EpubStructure.CONTENTLOCATION + EpubStructure.TOCLOCATION));

            string bodyText = "";
            int i = 1; 
            foreach(Page page in epub.pages)
            {
                string pageTitleNoSpaces = page.title.Replace(" ", "") + ".xhtml";
                bodyText += string.Format(EpubStructure.COMMONTOCITEM, pageTitleNoSpaces.Replace('.', '-'), i, pageTitleNoSpaces, page.title);
                i++;
            }
            writer.WriteLine(
            string.Format(EpubStructure.COMMONPAGE,
                string.Format(EpubStructure.COMMONHEADER, "Table of Contents"),
                string.Format(EpubStructure.COMMONTOCBODY, bodyText)
            ));
            writer.Close();
            AddToManifest(EpubStructure.TOCLOCATION, EpubStructure.TOCLOCATION);
            AddToSpine(EpubStructure.TOCLOCATION);
        }

        #region Package

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        /// <param name="idref"></param>
        public void AddToSpine(string idref)
        {
            epub.package.Spine.Itemref.Add(new Itemref()
            {
                Idref = idref.Replace('.', '-')
            });
            epub.toc.Add(idref);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epub"></param>
        /// <param name="id"></param>
        /// <param name="href"></param>
        public void AddToManifest(string id, string href)
        {
            epub.package.Manifest.Item.Add(new Item()
            {
                Id = id.Replace('.', '-'),
                Href = href,
                Mediatype = id
            });
        }

        public void CreatePackageManifest()
        {
            epub.package.Metadata.Identifier = new Identifier() { Id = "uid", Text = "67a3bf52-c649-4394-833c-f77f0a054aa2" };
            epub.package.Metadata.Language = "en-US";
            epub.package.Metadata.Title = epub.title;
            epub.package.Metadata.Creator = new Creator() { Id = "creator", Text = epub.author };
            epub.package.Metadata.Meta = new List<Meta>();
            epub.package.Metadata.Meta.Add(new Meta() { Id = "role", Property = "role", Refines = "#creator", Text = "aut" });
            epub.package.Metadata.Meta.Add(new Meta() { Property = "dcterms:modified", Text = DateTime.Now.ToLongTimeString() });
        }

        public void CreatePackage()
        {
            CreatePackageManifest();
            XmlSerializer xml = new XmlSerializer(typeof(Package));
            TextWriter textWriter = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.PACKAGELOCATION));
            xml.Serialize(textWriter, epub.package);
            textWriter.Close();
        }

        #endregion
    }
}//END OF NAMESPACE
