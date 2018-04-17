using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubCreator
{
    class Logger
    {
        /// <summary>
        /// Setup the Log
        /// </summary>
        /// <param name="logPath">Path to the Log</param>
        public static void LoggerSetup(string logPath)
        {
            Trace.Listeners.Clear();
            TextWriterTraceListener traceListener = new TextWriterTraceListener(logPath);
            //TextWriterTraceListener traceListener = new TextWriterTraceListener(Path.Combine(Path.GetTempPath(), AppDomain.CurrentDomain.FriendlyName+".log"));
            Trace.Listeners.Add(traceListener);
            Trace.Listeners.Add(new ConsoleTraceListener());   //need turn this off when release      
            Trace.AutoFlush = true;

        }

        /// <summary>
        /// Add some info to the log complete with datetime
        /// </summary>
        /// <param name="info">info to be added</param>
        public static void LogInfo(string info)
        {
            Trace.WriteLine(string.Format("{0}    Info: {1}", DateTime.Now, info));
        }

        /// <summary>
        /// Add some error to the log complete with date time and the Stack info
        /// </summary>
        /// <param name="errors">Error to be added</param>
        public static void LogError(string errors)
        {
            StackFrame caller = new StackFrame(1);
            Trace.WriteLine(string.Format("{0}    Error:{1}\n\t{2}", DateTime.Now, caller.GetMethod().DeclaringType + " " + caller.GetMethod().Name, errors));
        }

        /// <summary>
        /// Convert a list of strings to a single string
        /// </summary>
        /// <param name="list">List of strings</param>
        /// <returns>Single string</returns>
        public static string ListToString(List<string> list)
        {
            string str = string.Empty;
            foreach (string line in list)
            {
                str += line + "\n";
            }
            return str;
        }

        /// <summary>
        /// We don't want a ridiculously huge logFile so when it gets to big copy it to a different file and then delete the original
        /// </summary>
        /// <param name="logPath">Path to Log File</param>
        /// <param name="logFile">Log File Name</param>
        public static void LogRollover(string logPath, string logFile)
        {
            long fileSize = 100000;
            if (File.Exists(logPath))
            {
                if (new FileInfo(logPath).Length > fileSize)
                {
                    File.Copy(logPath, logPath.Replace(logFile, logFile + ".old"), true);
                    File.Delete(logPath);
                }
            }
        }
    }
}
