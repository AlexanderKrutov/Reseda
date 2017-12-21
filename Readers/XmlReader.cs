using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Reseda.Readers
{
    /// <summary>
    /// Class for reading Android string resources.
    /// </summary>
    public static class XmlReader
    {
        /// <summary>
        /// Reads Android string resources.
        /// </summary>
        /// <returns>Returns list of DataItem objects.</returns>
        public static List<DataItem> Read()
        {
            string resFolder = Config.InResFolder;

            List<DataItem> items = new List<DataItem>();

            // read default locale
            Program.Write("Reading string resources from `values\\strings.xml` ... ");
            string path = Path.Combine(resFolder, "values", "strings.xml");
            ReadLocale(items, path);
            Program.WriteLine("Done.", ConsoleColor.DarkGreen);
            Program.WriteLine($"(Loaded {items.Count(it => it.IsResource)} resource items.)");
            Program.WriteLine("");

            // read other locales
            foreach (string locale in Config.Locales)
            {
                Program.Write($"Reading string resources from `values-{locale}\\strings.xml` ... ");
                path = Path.Combine(resFolder, $"values-{locale}", "strings.xml");
                ReadLocale(items, path, locale);
                Program.WriteLine("Done.", ConsoleColor.DarkGreen);
                Program.WriteLine($"(Loaded {items.Count(it => it.IsResource && it.Values.ContainsKey(locale))} resource items.)");
                Program.WriteLine("");
            }

            return items;
        }

        private static void ReadLocale(List<DataItem> items, string path, string locale = "")
        {
            if (!File.Exists(path))
            {
                Program.WriteLineAndExit($"Resource file `{path}` not found.", -1, ConsoleColor.Red);
            }

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = Config.KeepEmptyRows;
            doc.Load(path);
            XmlNode resources = doc.ChildNodes.OfType<XmlElement>().FirstOrDefault(e => e.Name == "resources");

            if (resources == null)
            {
                Program.WriteLineAndExit("Node <resources> not found.", -1, ConsoleColor.Red);
            }

            if (resources.HasChildNodes)
            {
                for (int i = 0; i < resources.ChildNodes.Count; i++)
                {
                    var node = resources.ChildNodes[i];

                    if (node is XmlWhitespace)
                    {
                        if (locale == "")
                        {
                            var xmlWhitespace = (node as XmlWhitespace);
                            int count = xmlWhitespace.Value.Count(c => c == '\n');
                            for (int c = 0; c < count - 1; c++)
                            {
                                var item = new DataItem();
                                item.Values.Add(locale, "");
                                items.Add(item);
                            }
                        }
                    }
                    else if (node is XmlComment)
                    {
                        // read comments from default locale only
                        if (locale == "")
                        {
                            var xmlComment = (node as XmlComment);
                            var item = new DataItem();
                            item.Meta += "#";
                            item.Values.Add(locale, xmlComment.Value.Trim());
                            items.Add(item);
                        }                        
                    }
                    else if (node is XmlElement)
                    {
                        var xmlString = (node as XmlElement);
                        if (xmlString.Name == "string" && xmlString.HasAttribute("name"))
                        {
                            DataItem item = null;
                            string name = xmlString.GetAttribute("name");

                            // create new resource item only if it's a default locale
                            if (locale == "")
                            {
                                item = new DataItem();
                                item.Name = xmlString.GetAttribute("name");
                                item.Meta += (xmlString.HasAttribute("formatted") ? Boolean.Parse(xmlString.GetAttribute("formatted")) : true) ? "" : "f";
                                item.Meta += (xmlString.HasAttribute("translatable") ? Boolean.Parse(xmlString.GetAttribute("translatable")) : true) ? "" : "t";
                                item.Documentation = xmlString.HasAttribute("documentation") ? xmlString.GetAttribute("documentation") : null;

                                items.Add(item);
                            }
                            // try to found resource item with specified name from default locale
                            else
                            {
                                item = items.FirstOrDefault(it => it.Name == name);

                                if (item == null)
                                {
                                    continue;
                                }
                            }
                            
                            item.Values.Add(locale, xmlString.InnerXml);
                        }
                    }
                }
            }
        }
    }
}
