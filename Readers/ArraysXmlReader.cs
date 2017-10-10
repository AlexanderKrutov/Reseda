using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Reseda
{
    public static class ArraysXmlReader
    {
        public static List<ResourceItem> Read(string path)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to read file `{path}`. Reason: " + ex.Message);
                Program.Exit(-1);
                return null;
            }

            XmlNode resources = doc.ChildNodes.OfType<XmlElement>().FirstOrDefault(e => e.Name == "resources");
            if (resources == null)
            {
                Console.WriteLine($"Node <resources> not found in the file `{path}`.");
                Program.Exit(-1);
                return null;
            }

            if (!resources.HasChildNodes)
            {
                Console.WriteLine($"Node <resources> in the file `{path}` has no child items.");
                Program.Exit(-1);
                return null;
            }

            List<ResourceItem> items = new List<ResourceItem>();
            for (int i = 0; i < resources.ChildNodes.Count; i++)
            {
                var node = resources.ChildNodes[i];

                if (node is XmlComment)
                {
                    var xmlComment = (node as XmlComment);

                    items.Add(new ResourceItem()
                    {
                        IsComment = true,
                        IsArrayItem = true,
                        Value = xmlComment.Value.Trim()
                    });
                }
                else if (node is XmlElement)
                {
                    var xmlStringArray = (node as XmlElement);
                    if (xmlStringArray.Name == "string-array" && xmlStringArray.HasAttribute("name"))
                    {
                        var xmlStringArrayItems = xmlStringArray.ChildNodes;

                        foreach (var xmlStringArrayItem in xmlStringArrayItems.OfType<XmlElement>())
                        {
                            if (xmlStringArrayItem.Name == "item")
                            {
                                bool translatable = xmlStringArray.HasAttribute("translatable") ? Boolean.Parse(xmlStringArray.GetAttribute("translatable")) : true;

                                var resourceItem = new ResourceItem()
                                {
                                    Name = xmlStringArray.GetAttribute("name"),
                                    Value = xmlStringArrayItem.InnerXml,
                                    IsArrayItem = true,
                                    IsFormatted = true,
                                    IsTranslatable = translatable,
                                };

                                items.Add(resourceItem);
                            }
                        }
                    }
                }
            }
            
            return items;
        }
    }
}
