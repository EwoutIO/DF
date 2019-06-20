using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DF.Core.IO
{
    /// <summary>
    /// Responsible for the loading of a worspace, and providing a list of compilations of the contained files.
    /// </summary>
    public class WorkspaceLoader
    {
        // Hack: ensure these libraries/references are loaded.
        private readonly List<Type> dllLoader = new List<Type>() {
            typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions)
        };

        public WorkspaceLoader()
        {
            if (!MSBuildLocator.IsRegistered)
            {
                this.SetMSBuildVersion();
            }
        }

        /// <summary>
        /// Get a compiled project from the given project path
        /// </summary>
        /// <param name="projectPath">The path of the project</param>
        /// <returns>An in-memory compiled project</returns>
        public async Task<Compilation> GetCompilationFromProjectPath(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath) || !projectPath.EndsWith(".csproj") || !File.Exists(projectPath))
            {
                throw new InvalidDataException($"Could not load the project, invalid path provided: [{projectPath}]");
            }

            using (var workspace = MSBuildWorkspace.Create())
            {
                var project = await workspace.OpenProjectAsync(projectPath);

                return await project.GetCompilationAsync();
            }
        }

        /// <summary>
        /// Get a full list of compiled projects from the given project paths
        /// </summary>
        /// <param name="projectPaths">The paths of the included projects</param>
        /// <returns>A list of in-memory compiled projects</returns>
        public async Task<List<Compilation>> GetCompilationsFromProjectPaths(List<string> projectPaths)
        {
            var compilations = new List<Compilation>();
            using (var workspace = MSBuildWorkspace.Create())
            {
                foreach (var projectPath in projectPaths)
                {
                    var compilation = await this.GetCompilationFromProjectPath(projectPath);
                    compilations.Add(compilation);
                }

                return compilations;
            }
        }

        /// <summary>
        /// Get a full list of compiled projects from the solution located at the given path.
        /// </summary>
        /// <param name="solutionPath">The path to the solution.</param>
        /// <returns>A list of in-memory compiled projects</returns>
        public async Task<List<Compilation>> GetCompilationsFromSolutionPath(string solutionPath)
        {
            if (string.IsNullOrEmpty(solutionPath) || !solutionPath.EndsWith(".sln") || !File.Exists(solutionPath))
            {
                throw new InvalidDataException($"Could not load the solution, invalid path provided: [{solutionPath}]");
            }

            if (!MSBuildLocator.IsRegistered)
            {
                throw new Exception("No MSBuild version registered.");
            }

            using (var workspace = MSBuildWorkspace.Create())
            {
                var solution = await workspace.OpenSolutionAsync(solutionPath);
                var compilations = new List<Compilation>();

                foreach (var project in solution.Projects)
                {
                    var compilation = await project.GetCompilationAsync();
                    compilations.Add(compilation);
                }

                return compilations;
            }
        }

        /// <summary>
        /// Register a msbuild version if it is available.
        /// </summary>
        private void SetMSBuildVersion()
        {
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            VisualStudioInstance instance = visualStudioInstances.Length == 1 ? instance = visualStudioInstances[0] : visualStudioInstances.First(v => v.Version == visualStudioInstances.Max(mv => mv.Version));
            MSBuildLocator.RegisterInstance(instance);
        }
    }
}
