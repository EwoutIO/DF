using DF.CLI.Results;
using DF.Finder;
using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DF.CLI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var solutionPath = string.Empty;
            var configurationFolderPath = string.Empty;

            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the following parameters:");
                Console.WriteLine("/solutionPath=");
                Console.WriteLine("/configurationFolderPath=");
                return;
            }

            foreach (var arg in args)
            {
                var name = arg;
                var value = string.Empty;

                if (arg.Contains('='))
                {
                    name = arg.Substring(0, arg.IndexOf('='));
                    value = arg.Substring(arg.IndexOf('=') + 1);

                    switch (name.ToLower())
                    {
                        case "/solutionpath":
                            solutionPath = value;
                            break;
                        case "/configurationfolderpath":
                            if (value.EndsWith("\\")) value = value.Substring(0, value.Length - 1);
                            configurationFolderPath = value;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (string.IsNullOrEmpty(solutionPath) || !File.Exists(solutionPath))
            {
                Console.WriteLine("Invalid solution path provided.");
                return;
            }

            if (string.IsNullOrEmpty(configurationFolderPath) || !Directory.Exists(configurationFolderPath))
            {
                Console.WriteLine("Invalid configuration folder path provided.");
                return;
            }

            Helpers.SetMSBuildVersion(null);

            var configurationFiles = Helpers.GetConfigurationFiles(configurationFolderPath);

            var finder = new PatternFinder();

            Console.WriteLine("Initializing configuration and loading solution.");
            await finder.Initialize(configurationFiles, new List<string> { solutionPath }, true);
            Console.WriteLine("Scanning solution.");
            var results = finder.FindPatterns();

            ResultFormatter.PrintResults(results);

            Console.ReadKey();
        }
    }
}
