using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MSAL
{
    public class Logging
    {
        private static string _path { get; } =  Directory.GetCurrentDirectory()
                                                + "\\logging\\"
                                                + DateTime.Now.ToString("yyyy-MM-dd HHmmss fff");

        public static void HandleException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Looks like something went wrong. Here's the error message:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            WriteLogFile(ex);
        }

        public static void WriteLogFile()
        {
            if(Program.Json != null)
            {
                UseStreamWriterAsync(_path + ".json", Program.Json).Wait();
            }
        }

        public static void WriteLogFile(Exception ex)
        {
            string data =   "Error message received was:\n\n\""
                            + ex.Message
                            + "\"\n\nAnd here's the stack trace:\n\n\""
                            + ex.StackTrace
                            + "\"";
            
            UseStreamWriterAsync(_path + " (EXCEPTION OCCURRED).txt",data).Wait();
        }

        private async static Task UseStreamWriterAsync(string path, string data)
        {
            using StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
            await writer.WriteAsync(data);
        }
    }
}