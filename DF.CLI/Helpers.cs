using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DF.CLI
{
    public static class Helpers
    {
        /// <summary>
        /// Register a msbuild version if it is available.
        /// Todo: Might want to force this to a specific version depending on where it's loaded.
        /// From the context of a VSIX, we should be able to pass the version of VS used.
        /// </summary>
        public static void SetMSBuildVersion(string version)
        {
            if (!MSBuildLocator.IsRegistered)
            {
                // Attempt to set the version of MSBuild.
                var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
                VisualStudioInstance instance = null;

                if (visualStudioInstances.Length == 1)
                {
                    instance = visualStudioInstances[0];
                }
                else if (!string.IsNullOrEmpty(version))
                {
                    // Defensive programming, version might be wrong.
                    instance = visualStudioInstances.SingleOrDefault(v => version.Equals($"{v.Version.Major}.{v.Version.MajorRevision}"));
                }

                if (instance == null)
                {
                    instance = visualStudioInstances.First(v => v.Version == visualStudioInstances.Max(mv => mv.Version));
                }

                MSBuildLocator.RegisterInstance(instance);
            }
        }

        /// <summary>
        /// Gets the configuration files.
        /// </summary>
        /// <param name="configurationPath">The configuration path.</param>
        /// <returns></returns>
        public static List<string> GetConfigurationFiles(string configurationPath)
        {
            var dirInfo = new DirectoryInfo(configurationPath);

            var configFiles = dirInfo.GetFiles("*.xml");

            return configFiles.Select(c => c.FullName).ToList();
        }
    }
}
