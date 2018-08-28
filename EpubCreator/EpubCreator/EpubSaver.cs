using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace EpubCreator
{
    class EpubSaver
    {
        public const string EPUBEXTENSION = ".epub";
        public const string MOBIEXTENSION = ".mobi";
        public readonly string CONVERTER = @"..\..\..\..\Calibre Portable\Calibre\ebook-convert.exe";
        public Epub epub;

        /// <summary>
        /// Constructor: Setup the epub
        /// </summary>
        public EpubSaver(Epub epub)
        {
            this.epub = epub;
        }

        /// <summary>
        /// Create the Epub Zip Fiel
        /// </summary>
        public void CreateEpubFile()
        {
            Logger.LogInfo("CreateEpubFile");
            ZipFile.CreateFromDirectory(epub.location + EpubStructure.EPUBLOCATION, epub.location + epub.title.Replace(" ", "") + EPUBEXTENSION);
        }

        /// <summary>
        /// Create the MOBI file
        ///       we use the Calibre command line program
        /// </summary>
        public void CreateMobiFile()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = CONVERTER;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = epub.location + epub.title.Replace(" ", "") + EPUBEXTENSION + " " + epub.location + epub.title.Replace(" ", "") + MOBIEXTENSION;
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Unable to create .MOBI: " + ex.Message);
                throw new Exception("Unable to create .MOBI", ex);
            }
        }

        /// <summary>
        /// Populate the Epub Structure
        /// Create all the Folders
        /// And Create all the assests
        /// </summary>
        public void PopulateStructure()
        {
            Logger.LogInfo("PopulateStructure");
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
        /// Clean up the old structure
        /// We dont' care if this fails we'll just overwrite everything
        /// </summary>
        public void CleanUpOldStructure()
        {
            Logger.LogInfo("CleanUpOldStructure");
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
        /// Make all the directories
        /// </summary>
        public void CreateDirectories()
        {
            Logger.LogInfo("CreateDirectories");
            Directory.CreateDirectory(epub.location);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.METAINFLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.CSSLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION);
        }

        /// <summary>
        /// Create the Container file and the mimetype file
        /// </summary>
        public void CreateContainerAndMimetype()
        {
            Logger.LogInfo("CreateContainerAndMimetype");
            StreamWriter writer = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.METAINFLOCATION + EpubStructure.CONTAINERLOCATION));
            writer.WriteLine(EpubStructure.COMMONCONTAINER);
            writer.Close();

            writer = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.MIMETYPELOCATION));
            writer.Write(EpubStructure.COMMONMIMETYPE);
            writer.Close();
        }

        /// <summary>
        /// Create the cover if we need to
        /// </summary>
        public void CreateCover()
        {
            Logger.LogInfo("CreateCover");
            string imgFileName = epub.cover.Split('\\')[epub.cover.Split('\\').Length - 1];
            StreamWriter writer = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.COVERLOCATION));
            writer.WriteLine(
                string.Format(EpubStructure.COMMONCOVER, epub.title, (EpubStructure.IMAGELOCATION + imgFileName).Replace("\\", "/")));
            writer.Close();
            File.Copy(epub.cover, epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION + imgFileName, true);
            epub.AddToSpine(EpubStructure.COVERLOCATION);
            epub.AddToManifest(EpubStructure.COVERLOCATION, EpubStructure.COVERLOCATION);
            epub.AddToManifest(imgFileName, EpubStructure.IMAGELOCATION + imgFileName);
        }

        /// <summary>
        /// Create the title page
        /// </summary>
        public void CreateTitlePage()
        {
            Logger.LogInfo("CreateTitlePage");
            StreamWriter writer = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.TITLEPAGELOCATION));
            writer.WriteLine(string.Format(EpubStructure.COMMONTITLEPAGE, epub.title, epub.subtitle, epub.author));
            writer.Close();
            epub.AddToSpine(EpubStructure.TITLEPAGELOCATION);
            epub.AddToManifest(EpubStructure.TITLEPAGELOCATION, EpubStructure.TITLEPAGELOCATION);
        }

        /// <summary>
        /// Create and add a css asset
        /// </summary>
        /// <param name="css"></param>
        public void CreateCSS(string css)
        {
            Logger.LogInfo("CreateCSS");
            string cssFileName = css.Split('\\')[css.Split('\\').Length - 1];
            File.Copy(css, epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.CSSLOCATION + cssFileName, true);
            epub.AddToManifest(cssFileName, EpubStructure.CSSLOCATION + cssFileName);
        }

        /// <summary>
        /// Create and add an image asset
        /// </summary>
        /// <param name="img"></param>
        public void CreateImage(string img)
        {
            Logger.LogInfo("CreateImage");
            string imgFileName = img.Split('\\')[img.Split('\\').Length - 1];
            File.Copy(img, epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.IMAGELOCATION + imgFileName, true);
            epub.AddToManifest(imgFileName, EpubStructure.IMAGELOCATION + imgFileName);
        }

        /// <summary>
        /// Get the title name that we can use to save the file to and in the xhtml file
        /// </summary>
        /// <param name="title">Full title name</param>
        /// <returns>sanitized file name</returns>
        public string GetPageTitleNoSpaces(string title)
        {
            return title
                .Replace(" ", "")
                .Replace(":", "")
                .Replace("'", "")
                .Replace("&", "")
                .Replace("?", "")
                + ".xhtml";
        }

        /// <summary>
        /// Create the page
        /// Parse throught the website using the specified parser
        /// Save it tot he file structure and add it to the manifest and spine
        /// </summary>
        /// <param name="page">Page to Parse</param>
        public void CreatePage(Page page)
        {
            Logger.LogInfo("CreatePage");
            Logger.LogInfo("Parsing " + page.title + " with " + page.parser + "Parser");

            EpubParser parser = (EpubParser)Activator.CreateInstance(Type.GetType("EpubCreator." + page.parser + "Parser"));
            string bodyText = parser.Parse(page.url, epub);

            string pageTitleNoSpaces = GetPageTitleNoSpaces(page.title);

            StreamWriter writer = new StreamWriter(File.Create(epub.location + EpubStructure.EPUBLOCATION +
                EpubStructure.CONTENTLOCATION + pageTitleNoSpaces));
            writer.WriteLine(
            string.Format(EpubStructure.COMMONPAGE,
                string.Format(EpubStructure.COMMONHEADER, epub.title),
                string.Format(EpubStructure.COMMONBODY, pageTitleNoSpaces, page.title, bodyText, page.author)
            ));
            writer.Close();

            epub.AddToManifest(pageTitleNoSpaces, pageTitleNoSpaces);
            epub.AddToSpine(pageTitleNoSpaces);
        }

        /// <summary>
        /// Create the table of contents using the given page structure in the json
        /// </summary>
        public void CreateTOC()
        {
            Logger.LogInfo("CreateTOC");
            StreamWriter writer = new StreamWriter(File.Create(epub.location + EpubStructure.EPUBLOCATION +
                EpubStructure.CONTENTLOCATION + EpubStructure.TOCLOCATION));

            string bodyText = "";
            int i = 1;
            foreach(Page page in epub.pages)
            {
                string pageTitleNoSpaces = GetPageTitleNoSpaces(page.title);
                bodyText += string.Format(EpubStructure.COMMONTOCITEM, pageTitleNoSpaces.Replace('.', '-'), i, pageTitleNoSpaces, page.title);
                i++;
            }
            writer.WriteLine(
            string.Format(EpubStructure.COMMONPAGE,
                string.Format(EpubStructure.COMMONHEADER, "Table of Contents"),
                string.Format(EpubStructure.COMMONTOCBODY, bodyText)
            ));
            writer.Close();
            epub.AddToManifest(EpubStructure.TOCLOCATION, EpubStructure.TOCLOCATION, "nav");
            epub.AddToSpine(EpubStructure.TOCLOCATION);
        }

        #region Package

        /// <summary>
        /// Create the package manifest
        /// </summary>
        public void CreatePackageManifest()
        {
            Logger.LogInfo("CreatePackageManifest");
            epub.package.Metadata.Identifier = new Identifier() { Id = "uid", Text = "67a3bf52-c649-4394-833c-f77f0a054aa2" };
            epub.package.Metadata.Language = "en-US";
            epub.package.Metadata.Title = epub.title;
            epub.package.Metadata.Creator = new Creator() { Id = "creator", Text = epub.author };
            epub.package.Metadata.Meta = new List<Meta>();
            epub.package.Metadata.Meta.Add(new Meta() { Id = "role", Property = "role", Refines = "#creator", Text = "aut" });
            epub.package.Metadata.Meta.Add(new Meta() { Property = "dcterms:modified", Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") });
        }

        /// <summary>
        /// Create the Package
        /// </summary>
        public void CreatePackage()
        {
            Logger.LogInfo("CreatePackage");
            CreatePackageManifest();
            epub.package.Version = "3.0";
            epub.package.Uniqueidentifier = "uid";
            XmlSerializer xml = new XmlSerializer(typeof(Package));
            TextWriter textWriter = new StreamWriter(
                File.Create(epub.location + EpubStructure.EPUBLOCATION + EpubStructure.CONTENTLOCATION + EpubStructure.PACKAGELOCATION));
            xml.Serialize(textWriter, epub.package);
            textWriter.Close();
        }

        #endregion
    }
}//END OF NAMESPACE
