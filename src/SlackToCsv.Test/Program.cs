using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackToCsv.Test
{
    class Program
    {
        static Action<object> w = o => Console.WriteLine(o.ToString());
        static Func<string> r = () => Console.ReadLine();
        static Func<object, string> wr = o => { w(o); return r(); };

        static void Main(string[] args)
        {
            try
            {
                string resultFile = "export.csv";
                var path = string.Empty;

                while (path == string.Empty || !Directory.Exists(path))
                    path = wr("Please enter folder path of the unzipped Slack export:");

                var importer = new ImportSlack(path);
                var data = importer.Import();
                File.WriteAllText(Path.Combine(path, resultFile), data.ToString());
                w($"Data exported successfulle to {path}, file name: {resultFile}");
                r();
            }
            catch(Exception ex)
            {
                w(ex.Message);
                r();
            }
        }
    }
}
