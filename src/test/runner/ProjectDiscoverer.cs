using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.TestRunner.Loggers;

namespace Uno.TestRunner
{
    public static class ProjectDiscoverer
    {
        public static string[] Discover(List<string> searchPaths, ITestResultLogger logger)
        {
            if (searchPaths.Count == 0)
            {
                searchPaths.Add(Directory.GetCurrentDirectory());
            }
            var searchDirectories = searchPaths.Where(p => !p.EndsWith(".unoproj")).ToArray();
            var explicitProjects = searchPaths.Where(p => p.EndsWith(".unoproj")).ToArray();
            var tests = explicitProjects.ToList();

            var notFoundProjects = explicitProjects.Where(p => !File.Exists(p)).ToList();
            if (notFoundProjects.Any())
            {
                var errorMessage = string.Format("Project{0} not found", notFoundProjects.Count() > 1 ? "s" : "");
                logger.Log(errorMessage + ": " + string.Join(", ", notFoundProjects));
                throw new Exception(errorMessage);
            }

            if (tests.Any())
            {
                logger.Log("Specified unoproj:\n " + string.Join("\n ", explicitProjects));
            }

            if (searchDirectories.Length > 0)
            {
                logger.Log("Searching for tests in:\n " + string.Join("\n ", searchDirectories));
                foreach (var searchDirectory in searchDirectories)
                {
                    tests.AddRange(Directory.GetFiles(searchDirectory, "*Test.unoproj", SearchOption.AllDirectories).Select(Path.GetFullPath));
                }
            }

            return tests.ToArray();
        }
    }
}
