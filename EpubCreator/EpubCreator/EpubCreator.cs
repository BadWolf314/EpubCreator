using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Directory.CreateDirectory(epub.location);
            Directory.CreateDirectory(epub.location + EpubStructure.METAINFLOCATION);
            StreamWriter writer = new StreamWriter(File.Create(epub.location + EpubStructure.CONTAINERLOCATION));
            writer.WriteLine(EpubStructure.CONTAINER);
            writer.Close();
            Directory.CreateDirectory(epub.location + EpubStructure.CONTENTLOCATION);
            writer = new StreamWriter(File.Create(epub.location + EpubStructure.MIMETYPELOCATION));
            writer.WriteLine(EpubStructure.MIMETYPE);
            writer.Close();
            Directory.CreateDirectory(epub.location + EpubStructure.CSSLOCATION);
            Directory.CreateDirectory(epub.location + EpubStructure.IMAGELOCATION);
            //will want to wait to generate package.opf till we have all the files and everything we need
            writer = new StreamWriter(File.Create(epub.location + EpubStructure.PACKAGELOCATION));
            writer.Close();
            foreach(Page page in epub.pages)
            {
                EpubParser parser = (EpubParser)Activator.CreateInstance(Type.GetType("EpubCreator." + page.parser + "Parser"));
                string bodyText = parser.Parse(page.url);
                string pageTitleNoSpaces = page.title.Replace(" ", "");
                writer = new StreamWriter(File.Create(epub.location + EpubStructure.CONTENTLOCATION + "\\" + pageTitleNoSpaces + ".xhtml"));
                writer.WriteLine(
                string.Format(EpubStructure.COMMONPAGE,
                    string.Format(EpubStructure.COMMONHEADER, epub.title),
                    string.Format(EpubStructure.COMMONBODY, pageTitleNoSpaces, page.title, bodyText)
                ));
                writer.Close();
            }

        }
    }
}
