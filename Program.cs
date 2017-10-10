using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Reseda
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Parse(args);
           
            // XML => CSV
            if (!string.IsNullOrEmpty(Config.InResFolder))
            {
                List<ResourceSet> resourceSets = new List<ResourceSet>();

                for (int i = 0; i < Config.Locales.Length; i++)
                {
                    var strings = StringsXmlReader.Read(Path.Combine(Config.InResFolder, "values" + (string.IsNullOrEmpty(Config.Locales[i]) ? "" : "-" + Config.Locales[i]), "strings.xml"));
                    var arrays = ArraysXmlReader.Read(Path.Combine(Config.InResFolder, "values" + (string.IsNullOrEmpty(Config.Locales[i]) ? "" : "-" + Config.Locales[i]), "arrays.xml"));

                    ResourceSet resourceSet = new ResourceSet(Config.Locales[i]);
                    resourceSet.AddRange(strings);
                    resourceSet.AddRange(arrays);

                    Console.WriteLine($"Loaded {resourceSet.StringsCount} strings for locale `{resourceSet.Locale}`");

                    resourceSets.Add(resourceSet);
                }

                CsvWriter.Write(resourceSets, Config.OutCsv, Config.Separator);
            }

            // CSV => XML
            if (!string.IsNullOrEmpty(Config.OutResFolder))
            {
                var resourceSets = CsvReader.Read(Config.InCsv, Config.Locales, Config.Separator);
                foreach (var resourceSet in resourceSets)
                {
                    string stringsFile = Path.Combine(Config.OutResFolder, resourceSet.ValuesFolder, "strings.xml");
                    string arraysFile = Path.Combine(Config.OutResFolder, resourceSet.ValuesFolder, "arrays.xml");

                    string dir = "";
                    try
                    {
                        dir = Path.GetDirectoryName(stringsFile);
                        Directory.CreateDirectory(dir);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to create out directory `{dir}`. Reason: " + ex.Message);
                        Program.Exit(-1);
                        return;
                    }

                    try
                    {
                        dir = Path.GetDirectoryName(arraysFile);
                        Directory.CreateDirectory(dir);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to create out directory `{dir}`. Reason: " + ex.Message);
                        Program.Exit(-1);
                        return;
                    }

                    Console.Write($"Writing resources for locale `{resourceSet.LocaleName}`... ");

                    StringsXmlWriter.Write(stringsFile, resourceSet, Config.Indent);
                    ArraysXmlWriter.Write(arraysFile, resourceSet, Config.Indent);

                    Console.WriteLine($"Done (arrays: {resourceSet.ArraysCount}, strings: {resourceSet.StringsCount}).");
                }
            }

            Console.WriteLine("Done.");
            Exit(0);
        }

        public static void Exit(int code)
        {
            if (Config.DontExit)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            Environment.Exit(code);
        }
    }
}
