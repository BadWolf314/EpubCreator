using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EpubCreator
{
    class EpubCreator
    {
        static void Main(string[] args)
        {
            EpubCreator creator = new EpubCreator();
            try
            {
                creator.ParseArgs(args);
            }
            catch(Exception ex)
            {
                Logger.LogError("ERROR: " + ex.Message);
                creator.Usage();
                return;
            }

            try
            {
                creator.BuildEpub();
            }
            catch(Exception ex)
            {
                Logger.LogError("ERROR: " + ex.Message);
                return;
            }
        }

        #region Setup

        Epub epub = new Epub();


        public EpubCreator()
        {
            string logFile = "epubcreator.log";
            string logPath = Path.Combine(Path.GetTempPath(), logFile);
            Logger.LogRollover(logPath, logFile);
            Logger.LoggerSetup(logPath);
            Logger.LogInfo("Log is Setup");
        }

        public void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Logger.LogInfo("args[" + i + "]: " + args[i]);
                switch (args[i])
                {
                    case "--file": 
                    case "-f":
                        i++;
                        GetEpubInfoFromFile(args[i]);
                        break;
                    default:
                        throw new Exception("Unknown Parameter: " + args[i]);
                }
            }
        }

        public void Usage()
        {
            Console.WriteLine("Usage: EpubCreator.exe");
            Console.WriteLine("This program is used to create an .epub");
        }

        #endregion

        public void GetEpubInfoFromFile(string file)
        {
            epub = JsonConvert.DeserializeObject<Epub>(File.ReadAllText(file));
        }

        public void BuildEpub()
        {
            try
            {
                Directory.Delete(epub.location, true);
            }
            catch(Exception ex)
            {
                Logger.LogInfo("Unable to delete " + epub.location + ": " + ex.Message);
            }

            Directory.CreateDirectory(epub.location);
            Directory.CreateDirectory(epub.location + EpubStructure.METAINFLOCATION);
            StreamWriter writer = new StreamWriter(File.Create(epub.location + EpubStructure.CONTAINERLOCATION));
            writer.WriteLine(EpubStructure.CONTAINER);
            writer.Close();
            Directory.CreateDirectory(epub.location + EpubStructure.CONTENTLOCATION);
            writer = new StreamWriter(File.Create(epub.location + EpubStructure.MIMETYPELOCATION));
            writer.WriteLine(EpubStructure.MIMETYPE);
            writer.Close();

            string titlePage = "title-page.xhtml";
            writer = new StreamWriter(File.Create(epub.location + EpubStructure.CONTENTLOCATION + "\\" + titlePage));
            writer.WriteLine(string.Format(EpubStructure.COMMONTITLEPAGE, epub.title, epub.subtitle, epub.author));
            writer.Close();
            epub.package.Manifest.Item.Add(new Item()
            {
                Id = titlePage.Replace('.', '-'),
                Href = titlePage,
                Mediatype = EpubStructure.GetMediaType(titlePage)
            });
            epub.package.Spine.Itemref.Add(new Itemref() { Idref = titlePage.Replace('.', '-') });

            Directory.CreateDirectory(epub.location + EpubStructure.CSSLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.IMAGELOCATION);
            foreach(string css in epub.css)
            {
                string cssFileName = css.Split('\\')[css.Split('\\').Length - 1];
                string finalLocation = EpubStructure.CSSLOCATION.Split('\\')[1] + "\\" + cssFileName;
                File.Copy(css, epub.location + EpubStructure.CSSLOCATION + "\\" + cssFileName, true);
                epub.package.Manifest.Item.Add(new Item()
                {
                    Id = cssFileName.Replace('.','-'),
                    Href = finalLocation,
                    Mediatype = EpubStructure.GetMediaType(cssFileName)
                });
            }
            
            foreach(Page page in epub.pages)
            {
                Logger.LogInfo("Parsing " + page.title + " with " + page.parser + "Parser");
                EpubParser parser = (EpubParser)Activator.CreateInstance(Type.GetType("EpubCreator." + page.parser + "Parser"));
                string bodyText = parser.Parse(page.url, epub);
                string pageTitleNoSpaces = page.title.Replace(" ", "") + ".xhtml";
                string finalLocation = EpubStructure.CONTENTLOCATION + "\\" + pageTitleNoSpaces;
                writer = new StreamWriter(File.Create(epub.location + finalLocation));
                writer.WriteLine(
                string.Format(EpubStructure.COMMONPAGE,
                    string.Format(EpubStructure.COMMONHEADER, epub.title),
                    string.Format(EpubStructure.COMMONBODY, pageTitleNoSpaces, page.title, bodyText)
                ));
                writer.Close();
                epub.package.Manifest.Item.Add(new Item()
                {
                    Id = pageTitleNoSpaces.Replace('.', '-'),
                    Href = finalLocation.Split('\\')[1],
                    Mediatype = EpubStructure.GetMediaType(pageTitleNoSpaces)
                });
                epub.package.Spine.Itemref.Add(new Itemref()
                {
                    Idref = pageTitleNoSpaces.Replace('.', '-')
                });
            }

            epub.package.Metadata.Identifier = new Identifier() { Id = "uid", Text = "67a3bf52-c649-4394-833c-f77f0a054aa2" };
            epub.package.Metadata.Language = "en-US";
            epub.package.Metadata.Title = epub.title;
            epub.package.Metadata.Creator = new Creator() { Id = "creator", Text = epub.author };
            epub.package.Metadata.Meta = new List<Meta>();
            epub.package.Metadata.Meta.Add(new Meta() { Id = "role", Property = "role", Refines = "#creator", Text = "aut" });
            epub.package.Metadata.Meta.Add(new Meta() { Property = "dcterms:modified", Text = DateTime.Now.ToLongTimeString() });

            XmlSerializer xml = new XmlSerializer(typeof(Package));
            TextWriter textWriter = new StreamWriter(File.Create(epub.location + EpubStructure.PACKAGELOCATION));
            xml.Serialize(textWriter, epub.package);
            textWriter.Close();
        }
    }
}
