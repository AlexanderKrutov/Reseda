using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Reseda
{
    public static class CsvReader
    {
        public static List<ResourceSet> Read(string path, string[] locales, string separator)
        {
            TextFieldParser parser = null;

            try
            {
                parser = new TextFieldParser(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to read file `{path}`. Reason: " + ex.Message);
                Program.Exit(-1);
                return null;
            }

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(separator);

            List<ResourceSet> resourceSets = new List<ResourceSet>();

            string[] columns;
            while (!parser.EndOfData)
            { 
                // first line is a metadata
                if (parser.LineNumber == 1)
                {
                    columns = parser.ReadFields();

                    for (int i = 2; i < columns.Count() - 1; i++)
                    {
                        ResourceSet items = new ResourceSet(columns[i]);
                        resourceSets.Add(items);
                    }
                }
                else
                {
                    columns = parser.ReadFields();
                    var meta = columns[0];

                    // skip empty rows
                    if (columns.All(c => string.IsNullOrEmpty(c)))
                    {
                        continue;
                    }

                    // skip unused rows
                    if (meta.Contains(Meta.UNUSED))
                    {
                        continue;
                    }

                    for (int j = 0; j < resourceSets.Count; j++)
                    {
                        ResourceItem item = new ResourceItem();
                        item.Name = columns[1];
                        item.IsArrayItem = meta.Contains(Meta.ARRAY);
                        item.IsComment = meta.Contains(Meta.COMMENT);
                        if (!item.IsComment)
                        {
                            item.Value = columns.Length > 2 + j ? columns[2 + j] : null;
                            item.IsFormatted = !meta.Contains(Meta.FORMATTED);
                            item.IsTranslatable = !meta.Contains(Meta.TRANSLATABLE);
                        }
                        resourceSets[j].Add(item);
                    }
                }
            }

            parser.Close();

            // remove unlisted locales
            resourceSets = resourceSets.Where(loc => locales.Contains(loc.Locale)).ToList();

            return resourceSets;
        }
    }
}
