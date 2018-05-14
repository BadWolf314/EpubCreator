using Newtonsoft.Json;
using System;
using System.IO;

namespace EpubCreator
{
    class EpubCreator
    {
        /// <summary>
        /// Main Program
        /// Parse args and then try to create Epub
        /// </summary>
        /// <param name="args">parameters</param>
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
                creator.SaveEpub();
            }
            catch(Exception ex)
            {
                Logger.LogError("ERROR: " + ex.Message);
                return;
            }
        }

        #region Setup

        Epub epub = new Epub();
        EpubSaver saver;
        bool mobi = false;

        /// <summary>
        /// Constructor: Sets up the logging info
        /// </summary>
        public EpubCreator()
        {
            //Setup the log file
            string logFile = "epubcreator.log";
            string logPath = Path.Combine(Path.GetTempPath(), logFile);
            Logger.LogRollover(logPath, logFile);
            Logger.LoggerSetup(logPath);
            Logger.LogInfo("Log is Setup");
        }

        /// <summary>
        /// Parse the Args
        /// </summary>
        /// <param name="args">arguments from user</param>
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
                    case "-m":
                    case "--mobi":
                        mobi = true;
                        break;
                    default:
                        throw new Exception("Unknown Parameter: " + args[i]);
                }
            }
        }

        /// <summary>
        /// The usage of the program
        /// </summary>
        public void Usage()
        {
            Console.WriteLine("Usage: EpubCreator.exe [-f | --file] <fileName>");
            Console.WriteLine("This program is used to create an .epub");
            Console.WriteLine("\t--file: File that contains the json information for the epub");
        }

        #endregion

        /// <summary>
        /// Deserialize the file into the expected json object
        /// </summary>
        /// <param name="file">file name to build the epub from</param>
        public void GetEpubInfoFromFile(string file)
        {
            Logger.LogInfo("Reading file: " + file);
            saver = new EpubSaver(JsonConvert.DeserializeObject<Epub>(File.ReadAllText(file)));
        }

        /// <summary>
        /// Build the Epub using the Saver class
        /// </summary>
        public void BuildEpub()
        {
            saver.PopulateStructure();
        }

        /// <summary>
        /// Save the Epub into the proper file (.epub format)
        /// If mobi is specified save that file too
        /// </summary>
        public void SaveEpub()
        {
            saver.CreateEpubFile();
            if (mobi)
            {
                saver.CreateMobiFile();
            }
        }
    }
}
