using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace Reseda.Readers
{
    /// <summary>
    /// Class for reading resources from CSV file.
    /// </summary>
    public static class CsvReader
    {
        /// <summary>
        /// Reads items from CSV file.
        /// </summary>
        /// <returns>List of DataItem objects.</returns>
        public static List<DataItem> Read()
        {
            Program.Write($"Reading resources from `{Path.GetFileName(Config.InCsv)}` file ... ");

            TextFieldParser parser = null;

            var path = Config.InCsv;
            var separator = Config.Separator;
            var stringsCount = 0;
            var untranslatedCount = 0;

            try
            {
                parser = new TextFieldParser(path);
            }
            catch (Exception ex)
            {
                Program.WriteLineAndExit($"Unable to read file `{path}`. Reason: {ex.Message}" , -1, ConsoleColor.Red);
                return null;
            }

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(separator);

            List<DataItem> items = new List<DataItem>();
            Dictionary<string, int> headers = new Dictionary<string, int>();

            string[] columns;
            while (!parser.EndOfData)
            {
                // first line is a table header, collect columns positions
                if (parser.LineNumber == 1)
                {
                    columns = parser.ReadFields();

                    if (!columns.Contains("META", StringComparer.OrdinalIgnoreCase))
                    {
                        Program.WriteLineAndExit("There are no `META` column in the document header.", -1, ConsoleColor.Red);
                    }
                    else if (!columns.Contains("NAME", StringComparer.OrdinalIgnoreCase))
                    {
                        Program.WriteLineAndExit("There are no `NAME` column in the document header.", -1, ConsoleColor.Red);
                    }
                    else if (!columns.Contains("DOCS", StringComparer.OrdinalIgnoreCase))
                    {
                        Program.WriteLineAndExit("There are no `DOCS` column in the document header.", -1, ConsoleColor.Red);
                    }

                    for (int c = 0; c < columns.Count(); c++)
                    {
                        headers.Add(columns[c], c);
                    }
                }
                else
                {
                    columns = parser.ReadFields();

                    DataItem item = new DataItem();

                    for (int c = 0; c < Math.Min(headers.Count, columns.Count()); c++)
                    {
                        // META column
                        if (c == headers["META"])
                        {
                            item.Meta = columns[c];
                        }
                        // NAME column
                        else if (c == headers["NAME"])
                        {
                            item.Name = columns[c];
                        }
                        else if (c == headers["DOCS"])
                        {
                            item.Documentation = columns[c];
                        }
                        else
                        {
                            string localeCode = headers.FirstOrDefault(x => x.Value == c).Key;

                            // DEFAULT locale column
                            if (string.IsNullOrWhiteSpace(localeCode))
                            {
                                item.Values.Add("", columns[c]);
                            }
                            // other locale column
                            else
                            {
                                item.Values.Add(localeCode, columns[c]);
                            }
                        }
                    }

                    if (item.IsResource)
                    {
                        stringsCount++;
                    }

                    if (item.IsResource && item.IsTranslatable)
                    {
                        var untranslated = item.Values.Where(kv => !string.IsNullOrEmpty(kv.Key) && string.IsNullOrEmpty(kv.Value));
                        if (untranslated.Any())
                        {
                            var untranslatedLocales = untranslated.Select(kv => kv.Key).ToArray();
                            if (Config.ForceUntranslated)
                            {
                                // take value from default locale
                                foreach (string locale in untranslatedLocales)
                                {
                                    item.Values[locale] = item.Values[""];
                                }
                            }
                            else
                            {
                                if (untranslatedCount == 0)
                                {
                                    Program.WriteLine("");
                                }
                                untranslatedCount++;                                
                                Program.WriteLine($"Untranslated `{item.Name}` for locale(s): {string.Join(", ", untranslatedLocales)}", ConsoleColor.Yellow);
                            }
                        }
                    }

                    items.Add(item);
                }
            }

            parser.Close();

            Program.WriteLine("Done.", ConsoleColor.DarkGreen);
            Program.WriteLine($"({stringsCount} strings read.)\n");


            if (untranslatedCount > 0 && !Config.ForceUntranslated)
            {
                Program.WriteLineAndExit($"There are {untranslatedCount} untranslated strings found.\nUse `-force-untranslated` flag or translate them.", -1, ConsoleColor.Red);
            }

            return items;
        }
    }
}
