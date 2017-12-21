using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Reseda.Writers
{
    public class XmlWriter
    {
        private const string NEWLINE = "\r\n";

        public static void Write(List<DataItem> items)
        {
            var locales = new List<string>();

            // there are no locales in config, use all existing
            if (!Config.Locales.Any())
            {
                locales.AddRange(items.SelectMany(it => it.Values.Keys).Distinct());
            }
            // use locales defined in config only
            else
            {
                locales.Add("");
                locales.AddRange(Config.Locales);
            }

            foreach (var locale in locales)
            {
                string stringsFile = Path.Combine(Config.OutResFolder, "values" + (string.IsNullOrWhiteSpace(locale) ? "" : "-" + locale), "strings.xml");
                string dir = "";
                try
                {
                    dir = Path.GetDirectoryName(stringsFile);
                    Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    Program.WriteLineAndExit($"Unable to create output directory `{dir}`. Reason: " + ex.Message, -1);
                }

                Write(stringsFile, locale, items);
            }
        }

        public static void Write(string file, string locale, List<DataItem> items)
        {
            Program.Write($"Writing resources for locale `{(string.IsNullOrEmpty(locale) ? "Default" : locale)}`... ");

            string indent = Config.Indent;
            int stringsCount = 0;

            XmlDocument doc = new XmlDocument();

            // xml declaration
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement resources = doc.CreateElement(string.Empty, "resources", string.Empty);
            doc.AppendChild(resources);
            resources.AppendChild(doc.CreateTextNode(NEWLINE));

            foreach (var item in items)
            {
                // comment
                if (item.IsComment)
                {
                    resources.AppendChild(doc.CreateWhitespace(indent));
                    resources.AppendChild(doc.CreateComment($" {item.Name} "));
                    resources.AppendChild(doc.CreateTextNode(NEWLINE));
                }
                // string resource
                else
                {
                    // write empty rows if required
                    if (Config.KeepEmptyRows && item.IsEmptyRow)
                    {
                        resources.AppendChild(doc.CreateTextNode(NEWLINE));
                        continue;
                    }

                    // skip items without text
                    string value = item.Values[locale];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    // skip non-translatable strings for all locales except default
                    if (!item.IsTranslatable && !String.IsNullOrEmpty(locale))
                    {
                        continue;
                    }

                    XmlElement resourceString = doc.CreateElement("string");
                    resourceString.SetAttribute("name", item.Name);

                    if (!item.IsFormatted)
                    {
                        resourceString.SetAttribute("formatted", "false");
                    }

                    if (!item.IsTranslatable)
                    {
                        resourceString.SetAttribute("translatable", "false");
                    }

                    if (!string.IsNullOrEmpty(item.Documentation))
                    {
                        resourceString.SetAttribute("documentation", item.Documentation);
                    }

                    resourceString.InnerXml = value;
                    resources.AppendChild(doc.CreateWhitespace(indent));
                    resources.AppendChild(resourceString);
                    resources.AppendChild(doc.CreateTextNode(NEWLINE));

                    stringsCount++;
                }
            }

            Program.WriteLine($"Done.", ConsoleColor.DarkGreen);
            Program.WriteLine($"({stringsCount} strings added.)\n");

            try
            {
                doc.Save(file);
            }
            catch (Exception ex)
            {
                Program.WriteLineAndExit($"Unable to save xml in file `{file}`. Reason: {ex.Message}", -1, ConsoleColor.Red);
            }
        }
    }
}
