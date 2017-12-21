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
    /// <summary>
    /// Contains application run config
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Array of resources' locales, like "ru", "en", "de" to process
        /// </summary>
        public static string[] Locales { get; private set; }

        /// <summary>
        /// Path to input CSV file with resources
        /// </summary>
        public static string InCsv { get; private set; }

        /// <summary>
        /// Path to output folder with Android resources
        /// </summary>
        public static string OutResFolder { get; private set; }

        /// <summary>
        /// Path to input folder with Android resources
        /// </summary>
        public static string InResFolder { get; private set; }

        /// <summary>
        /// Path to output CSV file 
        /// </summary>
        public static string OutCsv { get; private set; }

        /// <summary>
        /// CSV separator character 
        /// </summary>
        public static string Separator { get; private set; }

        /// <summary>
        /// Indentation symbol(s) to format output XML files; like "\t" or "  " (double space)
        /// </summary>
        public static string Indent { get; private set; }

        /// <summary>
        /// Keep empty rows on converting resources to CSV and vise versa
        /// </summary>
        public static bool KeepEmptyRows { get; private set; }

        /// <summary>
        /// Force untranslated strings
        /// </summary>
        public static bool ForceUntranslated { get; private set; }

        /// <summary>
        /// Flag indicating the Reseda tool should not exit after run and has to wait for user input
        /// </summary>
        public static bool DontExit { get; private set; }

        /// <summary>
        /// Parses application command line arguments
        /// </summary>
        /// <param name="args"></param>
        public static void Parse(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            for (int i = 0; i < args.Length; i++)
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

                        case "-keep-empty-rows":
                            KeepEmptyRows = true;
                            break;

                        case "-force-untranslated":
                            ForceUntranslated = true;
                            break;

                        case "-dontexit":
                            DontExit = true;
                            break;

                        case "-help":
                        case "-h":
                            PrintHelp();
                            break;

                        default:
                            Program.WriteLine($"Unknown parameter `{args[i]}`.", ConsoleColor.Red);
                            PrintTip();
                            break;
                    }
                }
            }

            // two-directional

            if (!string.IsNullOrEmpty(InCsv) && !string.IsNullOrEmpty(InResFolder))
            {
                Program.WriteLineAndExit($"Only one `-in-...` parameter can be applied at the same time. Use `-in-res` OR `-in-csv` depending on what you need.", -1);
            }

            if (!string.IsNullOrEmpty(OutCsv) && !string.IsNullOrEmpty(OutResFolder))
            {
                Program.WriteLineAndExit($"Only one `-out-...` parameter can be applied at the same time. Use `-out-res` OR `-out-csv` depending on what you need.", -1);
            }

            // missed parameter pairs

            if (string.IsNullOrEmpty(InResFolder) && !string.IsNullOrEmpty(OutCsv))
            {
                Program.WriteLineAndExit($"You've specified `-out-csv` parameter but have missed `-in-res` parameter.", -1);
            }

            if (!string.IsNullOrEmpty(InResFolder) && string.IsNullOrEmpty(OutCsv))
            {
                Program.WriteLineAndExit($"You've specified `-in-res` parameter but missed `-out-csv` parameter.", -1);
            }

            if (string.IsNullOrEmpty(InCsv) && !string.IsNullOrEmpty(OutResFolder))
            {
                Program.WriteLineAndExit($"You've specified `-out-res` parameter but missed `-in-csv` parameter.", -1);
            }

            if (!string.IsNullOrEmpty(InCsv) && string.IsNullOrEmpty(OutResFolder))
            {
                Program.WriteLineAndExit($"You've specified `-in-csv` parameter but missed `-out-res` parameter.", -1);
            }

            if (string.IsNullOrEmpty(InCsv) &&
                string.IsNullOrEmpty(InResFolder))
            {
                Program.WriteLine($"There are no input files/folders provided.");
                PrintTip();
            }

            // locales
            if (Locales == null || !Locales.Any())
            {
                if (!string.IsNullOrEmpty(InResFolder))
                {
                    Locales = Directory.GetDirectories(InResFolder, "values-*").Select(d => Path.GetFileName(d).Substring("values-".Length)).ToArray();
                }
                else
                {
                    Locales = new string[0];
                }
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
                Program.WriteLineAndExit($"`-indent` parameter can have only white-space characters.", -1);
            }

            // check paths
            if (!string.IsNullOrEmpty(InCsv) && !File.Exists(InCsv))
            {
                Program.WriteLineAndExit($"`{InCsv}` is not exist or not a valid file name/path.", -1);
            }

            if (!string.IsNullOrEmpty(InResFolder) && !Directory.Exists(InResFolder))
            {
                Program.WriteLineAndExit($"`{InResFolder}` folder is not exist or not a valid directory path.", -1);
            }
        }

        private static string GetParameterValue(string[] args, int index)
        {
            if (index + 1 >= args.Length)
            {
                Program.WriteLine($"Command line parameter `{args[index]}` requires value.");
                PrintTip();
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
                    Program.WriteLineAndExit($"Locale name `{loc}` is incorrect.", -1, ConsoleColor.Red);
                    return null;
                }
            }

            return localesList.ToArray();
        }

        private static void PrintTip()
        {
            Program.WriteLineAndExit($"Type `-help` to get command line parameters reference.", -1);
        }

        private static void PrintHelp()
        {
            string help = @"

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
                      -locales ""ru, en, uk"" 
                      means use Default, Russian and Ukrainian locales.

-separator <char>   - CSV separator symbol. Usually "","" or "";"".
                      Default is "","".

-indent <string>    - XML tree indent string. Only white-space chars allowed.
                      Default is ""  "" (2 spaces).

-keep-empty-rows    - keep empty rows on converting resources.

-force-untranslated - force untranslated strings.
                      If a string item is translatable but has 
                      missing translation, its value will be 
                      taken from default locale.

-dontexit           - flag indicating that application will not exit 
                      until any key will be pressed.

-h | -help          - prints this help.
";

            Program.WriteLineAndExit(help, 0);
        }
    }
}
