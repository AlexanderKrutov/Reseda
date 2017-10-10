using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace Reseda
{
    public static class Config
    {
        public static string[] Locales { get; private set; }
        public static string InCsv { get; private set; }
        public static string OutResFolder { get; private set; }
        public static string InResFolder { get; private set; }
        public static string OutCsv { get; private set; }
        public static string Separator { get; private set; }
        public static string Indent { get; private set; }
        public static bool DontExit { get; private set; }

        public static void Parse(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            for (int i=0; i<args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    switch (args[i])
                    {
                        case "-in-csv":
                            InCsv = GetParameterValue(args, i);
                            break;

                        case "-out-res":
                            OutResFolder = GetParameterValue(args, i);
                            break;

                        case "-in-res":
                            InResFolder = GetParameterValue(args, i);
                            break;

                        case "-out-csv":
                            OutCsv = GetParameterValue(args, i);
                            break;

                        case "-locales":
                            string locales = GetParameterValue(args, i);
                            Locales = ParseLocales(locales);
                            break;

                        case "-separator":
                            Separator = GetParameterValue(args, i);
                            break;

                        case "-indent":
                            Indent = Regex.Unescape(GetParameterValue(args, i) ?? "");
                            break;

                        case "-dontexit":
                            DontExit = true;
                            break;

                        case "-help":
                        case "-h":
                            PrintHelp();
                            break;

                        default:
                            Console.WriteLine($"Unknown parameter `{args[i]}`.");
                            Console.WriteLine($"Type `-help` to get command line parameters reference.");
                            Program.Exit(-1);
                            break;
                    }
                }
            }

            // two-directional

            if (!string.IsNullOrEmpty(InCsv) && !string.IsNullOrEmpty(InResFolder))
            {
                Console.WriteLine($"Only one `-in-...` parameter can be applied at the same time. Use `-in-res` OR `-in-csv` depending on what you need.");
                Program.Exit(-1);
            }

            if (!string.IsNullOrEmpty(OutCsv) && !string.IsNullOrEmpty(OutResFolder))
            {
                Console.WriteLine($"Only one `-out-...` parameter can be applied at the same time. Use `-out-res` OR `-out-csv` depending on what you need.");
                Program.Exit(-1);
            }

            // missed parameter pairs

            if (string.IsNullOrEmpty(InResFolder) && !string.IsNullOrEmpty(OutCsv))
            {
                Console.WriteLine($"You specified `-out-csv` parameter but missed `-in-res` parameter.");
                Program.Exit(-1);
            }

            if (!string.IsNullOrEmpty(InResFolder) && string.IsNullOrEmpty(OutCsv))
            {
                Console.WriteLine($"You specified `-in-res` parameter but missed `-out-csv` parameter.");
                Program.Exit(-1);
            }

            if (string.IsNullOrEmpty(InCsv) && !string.IsNullOrEmpty(OutResFolder))
            {
                Console.WriteLine($"You specified `-out-res` parameter but missed `-in-csv` parameter.");
                Program.Exit(-1);
            }

            if (!string.IsNullOrEmpty(InCsv) && string.IsNullOrEmpty(OutResFolder))
            {
                Console.WriteLine($"You specified `-in-csv` parameter but missed `-out-res` parameter.");
                Program.Exit(-1);
            }

            // locales
            if (Locales == null || !Locales.Any())
            {
                Console.WriteLine($"`-locales` parameter value is missed.");
                Program.Exit(-1);
            }

            // separator
            if (string.IsNullOrEmpty(Separator))
            {
                Separator = ",";
            }

            // indent
            if (string.IsNullOrEmpty(Indent))
            {
                Indent = "  ";
            }
            else if (!string.IsNullOrWhiteSpace(Indent))
            {
                Console.WriteLine($"`-indent` parameter can have only white-space characters.");
                Program.Exit(-1);
            }

            // check paths

            if (!string.IsNullOrEmpty(InCsv) && !File.Exists(InCsv))
            {
                Console.WriteLine($"`{InCsv}` is not exist or not a valid file name/path.");
                Program.Exit(-1);
            }

            if (!string.IsNullOrEmpty(InResFolder) && !Directory.Exists(InResFolder))
            {
                Console.WriteLine($"`{InResFolder}` folder is not exist or not a valid directory path.");
                Program.Exit(-1);
            }
        }

        private static string GetParameterValue(string[] args, int index)
        {
            if (index + 1 >= args.Length)
            {
                Console.WriteLine($"Command line parameter `{args[index]}` requires value.");
                Console.WriteLine($"Type `-help` to get command line parameters reference.");
                Program.Exit(-1);
                return null;
            }
            else
            {
                return args[index + 1];
            }
        }

        private static string[] ParseLocales(string locales)
        {
            var localesList = locales.Split(',').Select(loc => loc.Trim()).ToList();

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var loc in localesList)
            {
                if (String.IsNullOrEmpty(loc))
                {
                    continue;
                }

                if (cultures.FirstOrDefault(c => c.Name == loc || c.TwoLetterISOLanguageName == loc) == null)
                {
                    Console.WriteLine($"Locale name `{loc}` is incorrect.");
                    Program.Exit(-1);
                    return null;
                }
            }

            return localesList.ToArray();
        }

        private static void PrintHelp()
        {

            string help = @"
Welcome to RESEDA ver 0.1
=========================

Command line reference:

-in-csv <filepath>  - path to input CSV file with localization resources.

-out-res <dirpath>  - path to output folder where the generated XML 
                      resource files will be placed.

-in-res <dirpath>   - path to input folder with XML resource files 
                      that will be converted to CSV.

-out-csv <filepath> - path to output CSV file with localization resources.

-locales <locales>  - list of comma-separated locales. Use it to filter 
                      unused localizations. 
                      Example:
                      -locales "" , ru, en, uk"" 
                      means use Default, Russian and Ukrainian locales.

-separator <char>   - CSV separator symbol. Usually "","" or "";"".
                      Default is "","".

-indent <string>    - XML tree indent string. Only white-space chars allowed.
                      Default is ""  "" (2 spaces).

-dontexit           - flag indicating that application will not exit 
                      until any key will be pressed.

-h | -help          - prints this help.";

            Console.WriteLine(help);
            Program.Exit(0);
        }
    }
}
