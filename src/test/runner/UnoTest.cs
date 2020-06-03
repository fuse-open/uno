using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                logger.Log("UnoTest discovered {0} projects:", discoveredProjects.Length);
                foreach (var discoveredProject in discoveredProjects)
                {
                    logger.Log(" " + discoveredProject);
                }
            }
            var tests = new List<Test>();
            foreach (var unoproj in discoveredProjects)
            {
                tests.AddRange(new TestProjectRunner(unoproj, options, logger).RunTests());
            }
            if (discoveredProjects.Length > 1)
            {
                logger.Log("");
                logger.Log("Since you ran multiple projects, here is a summary:");
                logger.Log("From {0} projects, ran {1} tests, {2} failed.", discoveredProjects.Length, tests.Count, tests.Count(t => t.Failed));
                logger.Log("");
                foreach (var test in tests.Where(t => t.Failed))
                {
                    logger.Log("  Failed:  {0}", test.Name);
                }
            }
            return tests.Count(t => t.Failed);
        }
    }
}
