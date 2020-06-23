using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Uno.Build.Packages;
using Uno.Compiler.API.Backends;
using Uno.IO;
using Uno.Logging;
using Uno.ProjectFormat;

namespace Uno.Build
{
    public class ProjectBuilder : LogObject
    {
        readonly BuildTarget _target;
        readonly BuildOptions _options;

        public ProjectBuilder(Log log, BuildTarget target, BuildOptions options)
            : base(log)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public BuildResult Build(string project)
        {
            return Build(Project.Load(project));
        }

        public BuildResult Build(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            var config = project.Config;
            if (_options.UpdateLibrary ?? config.GetBool("IsSourceTree"))
                new LibraryBuilder(Log).Build(config);

            try
            {
                var startTime = Log.Time;
                using (var driver = new BuildDriver(Log, _target, _options, project, config))
                {
                    BackendResult result = null;
                    if (driver.IsUpToDate)
                    {
                        driver.StopAnim();
                        if (Log.IsVerbose)
                            Log.Skip();
                        Log.WriteLine("Target is up-to-date -- stopping build (pass --force to override)");
                    }
                    else
                        result = driver.Build();

                    if (!Log.HasErrors && driver.CanBuildNative)
                        driver.BuildNative();

                    var product = driver.ProductPath;
                    if (!Log.HasErrors && !string.IsNullOrEmpty(product) && (
                            File.Exists(product) || Directory.Exists(product)
                        ))
                    {
                        Log.Skip(true);
                        Log.WriteLine($"  {project.Name} -> {product.ToRelativePath()}");
                    }

                    Log.BuildCompleted(startTime);

                    if (_options.PrintInternals)
                        driver.PrintInternals();

                    return driver.GetResult(result);
                }
            }
            catch (ThreadAbortException e)
            {
                Log.Trace(e);
                Log.Skip();
                Log.WriteLine("Aborted!");
                return BuildResult.Error(Log, project, _target, true);
            }
            catch (MaxErrorException e)
            {
                Log.Trace(e);
                Log.WriteErrorLine("FATAL ERROR: Max error count (" + Log.ErrorCount + ") reached -- stopping build");
                return BuildResult.Error(Log, project, _target);
            }
            catch (FatalException e)
            {
                Log.Trace(e);
                Log.FatalError(e.Source, e.ErrorCode, e.Message);
                return BuildResult.Error(Log, project, _target);
            }
            catch (SourceException e)
            {
                Log.Trace(e);
                Log.FatalError(e.Source, null, "Exception: " + e.Message + " (pass --trace for stack trace)");
                return BuildResult.Error(Log, project, _target);
            }
            catch (TargetInvocationException e)
            {
                Log.Trace(e);
                Log.Skip();
                Log.WriteErrorLine("FATAL ERROR: " + e.InnerException.Message + " (pass --trace for stack trace)");
                return BuildResult.Error(Log, project, _target);
            }
            catch (Exception e)
            {
                Log.Trace(e);
                Log.Skip();
                Log.WriteErrorLine("FATAL ERROR: " + e.Message + " (pass --trace for stack trace)");
                return BuildResult.Error(Log, project, _target);
            }
        }
    }
}
