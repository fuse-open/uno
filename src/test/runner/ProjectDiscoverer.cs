using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.IO;
using Uno.ProjectFormat;
using Uno.TestRunner.Loggers;

namespace Uno.TestRunner
{
    public static class ProjectDiscoverer
    {
        public static Project[] Discover(List<string> searchPaths, ITestResultLogger logger)
        {
            if (searchPaths.Count == 0)
            {
                searchPaths.Add(Directory.GetCurrentDirectory());
            }

            var searchDirectories = searchPaths.Where(p => !p.EndsWith(".unoproj")).ToArray();
            var explicitProjects = searchPaths.Where(p => p.EndsWith(".unoproj")).ToArray();
            var tests = new List<Project>();

            foreach (var file in explicitProjects)
            {
                try
                {
                    tests.Add(Project.Load(file));
                }
                catch
                {
                    logger.Log("Failed to load " + file.ToRelativePath());
                }
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
                    foreach (var file in Directory
                                            .GetFiles(searchDirectory, "*.unoproj", SearchOption.AllDirectories)
                                            .Select(Path.GetFullPath))
                    {
                        try
                        {
                            var project = Project.Load(file);

                            if (IsTestProject(project, logger))
                                tests.Add(project);
                        }
                        catch
                        {
                            logger.Log("Failed to load " + file.ToRelativePath());
                        }
                    }
                }
            }

            return tests.ToArray();
        }

        public static bool IsTestProject(Project project, ITestResultLogger logger)
        {
            var outputType = project.OutputType;

            switch (outputType)
            {
                case OutputType.Test:
                    return true;

                case OutputType.Undefined:
                    if (project.Name.EndsWith("Test", "test"))
                    {
                        logger.Log(project.Name + ": Missing \"outputType\" property in project file (assuming \"test\")");
                        return true;
                    }

                    return false;

                default:
                    return false;
            }
        }
    }
}
