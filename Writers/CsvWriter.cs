using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Reseda
{
    /// <summary>
    /// Writes CSV file with resources.
    /// </summary>
    public static class CsvWriter
    {
        public static void Write(ICollection<ResourceSet> locales, string path, string separator)
        {
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!String.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to create directories for path `{path}`. Reason: " + ex.Message);
                Program.Exit(-1);
                return;
            }

            StreamWriter fileWriter = null;
            try
            {
                fileWriter = new StreamWriter(path, false, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to create file `{path}`. Reason: " + ex.Message);
                Program.Exit(-1);
                return;
            }

            string quote = "\"";

            ResourceSet baseLocale = locales.First();

            StringBuilder line = new StringBuilder();

            // header line
            line.Append($"{quote}META{quote}{separator}");
            line.Append($"{quote}NAME{quote}{separator}");
            for (int i = 0; i < locales.Count; i++)
            {
                line.Append($"{quote}{locales.ElementAt(i).Locale}{quote}{separator}");
            }
            line.Append($"{quote}COMMENTS{quote}");
            fileWriter.WriteLine(line.ToString());

            foreach (var item in baseLocale)
            { 
                line.Clear();

                StringBuilder meta = new StringBuilder();
                if (item.IsArrayItem)
                {
                    meta.Append(Meta.ARRAY);
                }

                // comment
                if (item.IsComment)
                {
                    meta.Append(Meta.COMMENT);

                    // add empty row before comment
                    fileWriter.WriteLine($"{quote}{Meta.UNUSED}{quote}{separator}");

                    // column #1 - meta (comment sign)
                    line.Append($"{quote}{meta.ToString()}{quote}{separator}");

                    // column #2 - comment text
                    line.Append($"{quote}{item.Value.CsvEscape()}{quote}{separator}");
                }
                // resource value
                else
                {
                    if (!item.IsFormatted)
                    {
                        meta.Append(Meta.FORMATTED);
                    }
                    if (!item.IsTranslatable)
                    {
                        meta.Append(Meta.TRANSLATABLE);
                    }

                    // column #1 - meta
                    line.Append($"{quote}{meta.ToString()}{quote}{separator}");

                    // column #2 - resource name
                    line.Append($"{quote}{item.Name.CsvEscape()}{quote}{separator}");

                    // column #3 - base locale resource value
                    line.Append($"{quote}{item.Value.CsvEscape()}{quote}{separator}");

                    // column #4 ... #N - other locales
                    for (int i = 1; i < locales.Count; i++)
                    {
                        var secondLocaleStringItems = locales.ElementAt(i).Where(itm => itm.Name == item.Name).ToList();
                        if (secondLocaleStringItems.Any())
                        {
                            string value = null;
                            // array item
                            if (item.IsArrayItem)
                            {
                                var baseLocaleStringItems = baseLocale.Where(itm => itm.Name == item.Name).ToList();
                                var arrayIndex = baseLocaleStringItems.FindIndex(itm => itm == item);
                                if (arrayIndex < secondLocaleStringItems.Count)
                                {
                                    value = secondLocaleStringItems.ElementAt(arrayIndex).Value;
                                }             
                            }
                            // string item
                            else
                            {
                                value = secondLocaleStringItems.First().Value;
                            }
                            line.Append($"{quote}{value.CsvEscape()}{quote}{separator}");
                        }                        
                    }

                    // column #N+1 - documentation
                    line.Append($"{quote}{item.Documentation.CsvEscape()}{quote}{separator}");
                }

                fileWriter.WriteLine(line.ToString());
            }

            fileWriter.Flush();
            fileWriter.Close();
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
