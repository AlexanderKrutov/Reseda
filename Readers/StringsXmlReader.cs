using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

namespace Reseda
{
    public static class StringsXmlReader
    {
        public static List<ResourceItem> Read(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode resources = doc.ChildNodes.OfType<XmlElement>().FirstOrDefault(e => e.Name == "resources");

            if (resources == null)
            {
                throw new InvalidDataException("Node <resources> not found.");
            }

            List<ResourceItem> items = new List<ResourceItem>();

            if (resources.HasChildNodes)
            {
                for (int i = 0; i < resources.ChildNodes.Count; i++)
                {
                    var node = resources.ChildNodes[i];

                    if (node is XmlComment)
                    {
                        var xmlComment = (node as XmlComment);
                        items.Add(new ResourceItem()
                        {
                            IsComment = true,
                            Value = xmlComment.Value.Trim()
                        });
                    }
                    else if (node is XmlElement)
                    {
                        var xmlString = (node as XmlElement);
                        if (xmlString.Name == "string" && xmlString.HasAttribute("name"))
                        {
                            items.Add(new ResourceItem()
                            {
                                Name = xmlString.GetAttribute("name"),
                                Value = xmlString.InnerXml,
                                IsFormatted = xmlString.HasAttribute("formatted") ? Boolean.Parse(xmlString.GetAttribute("formatted")) : true,
                                IsTranslatable = xmlString.HasAttribute("translatable") ? Boolean.Parse(xmlString.GetAttribute("translatable")) : true,
                                Documentation = xmlString.HasAttribute("documentation") ? xmlString.GetAttribute("documentation") : null,
                            });
                        }
                    }
                }
            }

            return items;
        }

    }
}
