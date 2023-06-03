using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uno.Build;
using Uno.Build.Targets;
using Uno.Diagnostics;
using Uno.Logging;
using Uno.ProjectFormat;
using Uno.TestRunner.BasicTypes;
using Uno.TestRunner.Loggers;

namespace Uno.TestRunner
{
    public class TestProjectRunner
    {
        private readonly Project _project;
        private readonly TestOptions _options;
        private readonly ITestResultLogger _logger;
        private TestRun _testRun;

        public TestProjectRunner(Project unoProj, TestOptions options, ITestResultLogger logger)
        {
            _project = unoProj;
            _options = options;
            _logger = logger;
        }

        public List<Test> RunTests()
        {
            _logger.ProjectStarting(_project.Name, _options.Target.ToString());
            List<Test> tests = new();

            try
            {
                _testRun = new TestRun(_logger);

                var cts = new CancellationTokenSource();
                bool runFinished = false;

                try
                {
                    var log = Log.Default;
                    var target = _options.Target;
                    var outputDirectory = _options.OutputDirectory ?? _project.GetOutputDirectory("test", target);

                    var options = new BuildOptions {
                        Test = true,
                        Force = true,
                        TestFilter = _options.Filter,
                        OutputDirectory = outputDirectory,
                        WarningLevel = 1,
                        UpdateLibrary = _options.UpdateLibrary
                    };

                    options.Defines.AddRange(_options.Defines);
                    options.Undefines.AddRange(_options.Undefines);

                    if (_options.Target is iOSBuild && !_project.MutableProperties.ContainsKey("ios.bundleIdentifier"))
                        _project.MutableProperties["ios.bundleIdentifier"] = "dev.testprojects." + _project.Name.ToIdentifier(true).ToLower();

                    if (_options.OnlyGenerate)
                        options.NativeBuild = false;

                    var builder = new ProjectBuilder(log, target, options);
                    var result = builder.Build(_project);

                    if (result.ErrorCount != 0)
                        throw new Exception("Build failed.");

                    if (_options.OnlyBuild || _options.OnlyGenerate)
                        return tests;

                    // We don't need a window when running tests.
                    Environment.SetEnvironmentVariable("UNO_WINDOW_HIDDEN", "1");

                    var targetLog = new Log(new DebugLogTestFilter(Console.Out, _testRun), Console.Error);
                    targetLog.WriteLine();

                    Task runTask = null;
                    try
                    {
                        runTask = Task.Run(() => result.RunAsync(targetLog, cts.Token), cts.Token);
                        _testRun.Start();

                        tests = _testRun.WaitUntilFinished();
                        runFinished = runTask.Wait(100);
                    }
                    finally
                    {
                        if ((target is AndroidBuild || target is iOSBuild) &&
                            !_options.DontUninstall &&
                            runTask != null)
                        {
                            // Wait a little more for app to quit, after that we don't care
                            runTask.Wait(500);
                            Task.Run(() => target.Run(Shell.Default, result.File, "uninstall", cts.Token)).Wait();
                        }
                    }
                }
                finally
                {
                    _logger.ProjectEnded(tests);

                    if (!runFinished)
                        cts.Cancel();

                    cts.Dispose();
                }
            }
            finally
            {
                if (_testRun != null)
                {
                    _testRun.Dispose();
                    _testRun = null;
                }
            }

            return tests;
        }
    }
}
