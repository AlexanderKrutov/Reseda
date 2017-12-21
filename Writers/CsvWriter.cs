using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda.Writers
{
    /// <summary>
    /// Writes CSV file with resources.
    /// </summary>
    public static class CsvWriter
    {
        public static void Write(List<DataItem> items)
        {
            string path = Config.OutCsv;

            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                Program.WriteAndExit($"Unable to create directories for path `{path}`. Reason: {ex.Message}", -1);
            }

            StreamWriter fileWriter = null;
            try
            {
                fileWriter = new StreamWriter(path, false, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Program.WriteLineAndExit($"Unable to create file `{path}`. Reason: {ex.Message}", -1, ConsoleColor.Red);
            }

            string quote = "\"";
            string separator = Config.Separator;

            StringBuilder line = new StringBuilder();

            // header line
            line.Append($"{quote}META{quote}{separator}");
            line.Append($"{quote}NAME{quote}{separator}");
            line.Append($"{quote}{quote}{separator}");
            foreach (string locale in Config.Locales)
            {
                line.Append($"{quote}{locale}{quote}{separator}");
            }

            line.Append($"{quote}DOCS{quote}");
            fileWriter.WriteLine(line.ToString());

            Program.Write($"Writing resources to `{Path.GetFileName(path)}` file ... ");

            foreach (var item in items)
            {
                line.Clear();

                // column #1 - meta 
                line.Append($"{quote}{item.Meta}{quote}{separator}");
   
                if (item.IsComment)
                {
                    // column #2 - comment text
                    line.Append($"{quote}{item.Values[""].CsvEscape()}{quote}{separator}");
                }
                // resource value
                else
                {
                    // column #2 - resource name
                    line.Append($"{quote}{item.Name.CsvEscape()}{quote}{separator}");

                    // column #3 - base locale resource value
                    line.Append($"{quote}{item.Values[""].CsvEscape()}{quote}{separator}");

                    // column #4 ... #N - other locales
                    foreach (string locale in Config.Locales)
                    { 
                        line.Append($"{quote}{(item.Values.ContainsKey(locale) ? item.Values[locale].CsvEscape() : "")}{quote}{separator}");
                    }

                    // column #N+1 - documentation
                    line.Append($"{quote}{item.Documentation.CsvEscape()}{quote}{separator}");
                }

                fileWriter.WriteLine(line.ToString());
            }

            fileWriter.Flush();
            fileWriter.Close();

            Program.WriteLine("Done.", ConsoleColor.DarkGreen);
            Program.WriteLine("");
        }

        public static string CsvEscape(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            else
            {
                return str.Replace("\r\n", "").Replace("\"", "\"\"");
            }
        }
    }
}
