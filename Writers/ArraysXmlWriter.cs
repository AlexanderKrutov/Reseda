using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Reseda
{
    public static class ArraysXmlWriter
    {
        private const string NEWLINE = "\n";

        public static void Write(string file, ResourceSet arrayItems, string indent)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement resources = doc.CreateElement(string.Empty, "resources", string.Empty);
            doc.AppendChild(resources);
            resources.AppendChild(doc.CreateTextNode(NEWLINE));

            var groups = arrayItems.Where(a => a.IsArrayItem).GroupBy(a => a.Name).ToList();

            foreach (var group in groups)
            {
                // comment
                if (group.First().IsComment)
                {
                    resources.AppendChild(doc.CreateTextNode(NEWLINE));
                    resources.AppendChild(doc.CreateWhitespace(indent));
                    resources.AppendChild(doc.CreateComment($" {group.Key} "));
                    resources.AppendChild(doc.CreateTextNode(NEWLINE));
                }
                // array item resource
                else
                {
                    bool translatable = group.All(itm => itm.IsTranslatable);

                    // skip non-translatable arrays for all locales except default
                    if (!translatable && !string.IsNullOrEmpty(arrayItems.Locale))
                    {
                        continue;
                    }

                    // skip arrays with no items
                    if (group.All(itm => string.IsNullOrEmpty(itm.Value)))
                    {
                        continue;
                    }

                    XmlElement stringArray = doc.CreateElement("string-array");
                    stringArray.SetAttribute("name", group.Key);

                    if (!translatable)
                    {
                        stringArray.SetAttribute("translatable", "false");
                    }

                    stringArray.AppendChild(doc.CreateTextNode(NEWLINE));
                    foreach (var item in group)
                    {
                        XmlElement stringArrayItem = doc.CreateElement("item");
                        stringArrayItem.InnerXml = item.Value;
                        stringArray.AppendChild(doc.CreateWhitespace(indent + indent));
                        stringArray.AppendChild(stringArrayItem);
                        stringArray.AppendChild(doc.CreateTextNode(NEWLINE));
                    }

                    stringArray.AppendChild(doc.CreateTextNode(indent));
                    resources.AppendChild(doc.CreateWhitespace(indent));
                    resources.AppendChild(stringArray);
                    resources.AppendChild(doc.CreateWhitespace(indent));
                    resources.AppendChild(doc.CreateTextNode(NEWLINE));
                }
            }

            resources.AppendChild(doc.CreateTextNode(NEWLINE));

            try
            {
                doc.Save(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to save xml in file `{file}`. Reason: " + ex.Message);
                Program.Exit(-1);
            }
        }
    }
}
