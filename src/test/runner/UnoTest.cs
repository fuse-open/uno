using System.Collections.Generic;
using System.Linq;
using Uno.TestRunner.BasicTypes;
using Uno.TestRunner.Loggers;

namespace Uno.TestRunner
{
    public class UnoTest
    {
        public static int DiscoverAndRun(TestOptions options)
        {
            var logger = LoggerFactory.CreateLogger(options);
            var discoveredProjects = ProjectDiscoverer.Discover(options.Paths, logger);

            if (discoveredProjects.Length > 1)
            {
                logger.Log("Discovered {0} projects:", discoveredProjects.Length);

                foreach (var discoveredProject in discoveredProjects)
                    logger.Log(" " + discoveredProject);

                logger.Log("");
            }

            var tests = new List<Test>();

            foreach (var project in discoveredProjects)
                tests.AddRange(new TestProjectRunner(project, options, logger).RunTests());

            var failedCount = tests.Count(t => t.Failed);

            if (discoveredProjects.Length > 1 && tests.Count > 0)
            {
                logger.Log("Since you ran multiple projects, here is a summary:");
                logger.Log("From {0} projects, ran {1} tests, {2} failed.", discoveredProjects.Length, tests.Count, failedCount);
                logger.Log("");

                if (failedCount > 0)
                {
                    foreach (var test in tests.Where(t => t.Failed))
                        logger.Log("  Failed:  {0}", test.Name);

                    logger.Log("");
                }
            }

            return failedCount;
        }
    }
}
