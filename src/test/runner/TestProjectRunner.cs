using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly string _unoProj;
        private readonly TestOptions _options;
        private readonly ITestResultLogger _logger;
        private TestRun _testRun;

        public TestProjectRunner(string unoProj, TestOptions options, ITestResultLogger logger)
        {
            _unoProj = unoProj;
            _options = options;
            _logger = logger;
        }

        public List<Test> RunTests()
        {
            var project = Path.GetFileNameWithoutExtension(_unoProj);
            _logger.ProjectStarting(project, _options.Target.ToString());
            List<Test> tests = new List<Test>();
            try
            {
                _testRun = new TestRun(_logger);

                var cts = new CancellationTokenSource();
                bool runFinished = false;
                try
                {
                    var log = Log.Default;
                    var target = _options.Target;
                    var proj = Project.Load(_unoProj);
                    var outputDirectory = _options.OutputDirectory ?? proj.GetOutputDirectory("Test", target);

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

                    if (_options.Target is iOSBuild && !proj.MutableProperties.ContainsKey("iOS.BundleIdentifier"))
                        proj.MutableProperties["iOS.BundleIdentifier"] = "dev.testprojects." + proj.Name.ToIdentifier(true).ToLower();

                    if (_options.OnlyGenerate)
                        options.NativeBuild = false;

                    var builder = new ProjectBuilder(log, target, options);
                    var result = builder.Build(proj);

                    if (result.ErrorCount != 0)
                        throw new Exception("Build failed.");

                    if (_options.OnlyBuild || _options.OnlyGenerate)
                        return tests;

                    // We don't need a window when running tests.
                    Environment.SetEnvironmentVariable("UNO_WINDOW_HIDDEN", "1");

                    var targetLog = new Log(new DebugLogTestFilter(Console.Out, _testRun), Console.Error);

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
