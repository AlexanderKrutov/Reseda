using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Reseda
{
    public static class StringsXmlWriter
    {
        private const string NEWLINE = "\n";

        public static void Write(string file, ResourceSet strings, string indent)
        {
            XmlDocument doc = new XmlDocument();

            // xml declaration
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement resources = doc.CreateElement(string.Empty, "resources", string.Empty);
            doc.AppendChild(resources);
            resources.AppendChild(doc.CreateTextNode(NEWLINE));

            foreach (var item in strings)
            {
                // skip array items, will be written in separate file
                if (item.IsArrayItem)
                {
                    continue;
                }

                // comment
                if (item.IsComment)
                {
                    resources.AppendChild(doc.CreateTextNode(NEWLINE));
                    resources.AppendChild(doc.CreateWhitespace(indent));
                    resources.AppendChild(doc.CreateComment($" {item.Name} "));
                    resources.AppendChild(doc.CreateTextNode(NEWLINE));
                }
                // string resource
                else
                {
                    // skip items without text
                    if (string.IsNullOrEmpty(item.Value))
                    {
                        continue;
                    }

                    // skip non-translatable strings for all locales except default
                    if (!item.IsTranslatable && !String.IsNullOrEmpty(strings.Locale)) 
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
                    
                    resourceString.InnerXml = item.Value;
                    resources.AppendChild(doc.CreateWhitespace(indent));
                    resources.AppendChild(resourceString);
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
