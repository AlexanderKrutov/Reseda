using Reseda.Readers;
using Reseda.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("");
            WriteLine("╔════════╤══════════════════════════════════════════╗", ConsoleColor.DarkCyan);
            WriteLine("║ Reseda │ http://github.com/AlexanderKrutov/Reseda ║", ConsoleColor.DarkCyan);
            WriteLine("╚════════╧══════════════════════════════════════════╝", ConsoleColor.DarkCyan);
            WriteLine("");

            Config.Parse(args);

            // CSV => XML
            if (!string.IsNullOrEmpty(Config.OutResFolder))
            {
                Program.WriteLine("Task: CSV to XML", ConsoleColor.DarkGray);
                XmlWriter.Write(CsvReader.Read());
            }

            // XML => CSV
            if (!string.IsNullOrEmpty(Config.InResFolder))
            {
                CsvWriter.Write(XmlReader.Read());
            }

            WriteLineAndExit("SUCCESS", 0, ConsoleColor.Green);
        }

        public static void Exit(int code)
        {
            if (Config.DontExit)
            {
                WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            Environment.Exit(code);
        }


        public static void Write(string text, ConsoleColor? color = null)
        {
            Write(text, false, color, null);
        }

        public static void WriteLine(string text, ConsoleColor? color = null)
        {
            Write(text, true, color, null);
        }

        public static void WriteAndExit(string text, int exitCode, ConsoleColor? color = null)
        {
            Write(text, false, color, exitCode);
        }

        public static void WriteLineAndExit(string text, int exitCode, ConsoleColor? color = null)
        {
            Write(text, true, color, exitCode);
        }

        private static void Write(string text, bool terminateLine, ConsoleColor? color, int? exitCode)
        {
            ConsoleColor savedColor = Console.ForegroundColor;
            if (color != null)
            {
                Console.ForegroundColor = color.Value;
            }

            if (terminateLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }

            if (color != null)
            {
                Console.ForegroundColor = savedColor;
            }

            if (exitCode != null)
            {
                Exit(exitCode.Value);
            }
        }
    }
}
