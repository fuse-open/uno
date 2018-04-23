using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Uno.Build;
using Uno.Build.Targets.Android;
using Uno.Build.Targets.Xcode;
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
        private readonly CommandLineOptions _options;
        private readonly ITestResultLogger _logger;
        private TestRun _testRun;

        public TestProjectRunner(string unoProj, CommandLineOptions options, ITestResultLogger logger)
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
                _testRun = new TestRun(_logger, _options.TestTimeout, _options.StartupTimeout);

                HttpTestCommunicator communicator = null;
                if (!_options.RunLocal)
                    communicator = new HttpTestCommunicator(_logger, _testRun, NeedsPublicIp());

                var cts = new CancellationTokenSource();
                bool runFinished = false;
                try
                {
                    var log = Log.Default;
                    var target = _options.Target;
                    var proj = Project.Load(_unoProj);
                    var outputDirectory = _options.OutputDirectory ?? Path.GetFullPath(Path.Combine(proj.BuildDirectory, "Test", target.Identifier));

                    // YUCK: need to start the communicator before building, to get the Prefix/TestServerUrl
                    if (communicator != null)
                        communicator.Start();

                    var options = new BuildOptions {
                        Test = true,
                        Force = true,
                        TestFilter = _options.Filter,
                        TestServerUrl = _options.RunLocal ? string.Empty : communicator.Prefix,
                        OutputDirectory = outputDirectory,
                        WarningLevel = 1,
                        Library = _options.Library,
                        PackageTarget = Build.Targets.BuildTargets.Package
                    };

                    options.Defines.AddRange(_options.Defines);
                    options.Undefines.AddRange(_options.Undefines);

                    if (_options.OpenDebugger)
                    {
                        options.RunArguments = "debug";
                        options.Native = false; // disable native build
                        options.Defines.Add("DEBUG_NATIVE"); // disable native optimizations
                    }

                    var builder = new ProjectBuilder(log, target, options);
                    var result = builder.Build(proj);
                    if (result.ErrorCount != 0)
                        throw new Exception("Build failed.");

                    Log targetLog = null;
                    if (_options.RunLocal)
                    {
                        targetLog = new Log(new DebugLogTestFilter(Console.Out, _testRun), Console.Error);
                    }
                    Task runTask = null;
                    try
                    {
                        if (!_options.AllowDebugger)
                            runTask = Task.Run(() => result.RunAsync(targetLog, cts.Token), cts.Token);
                        _testRun.Start();


                        tests = _testRun.WaitUntilFinished();
                        if (runTask != null)
                            runFinished = runTask.Wait(100);

                    }
                    finally
                    {
                        if ((target is AndroidBuild || target is iOSBuild) &&
                            !_options.NoUninstall &&
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
                    if (communicator != null)
                        communicator.Stop();

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

        private bool NeedsPublicIp()
        {
            return _options.Target is AndroidBuild;
        }
    }
}
