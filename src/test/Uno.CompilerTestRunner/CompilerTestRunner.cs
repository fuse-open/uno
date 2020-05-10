using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Uno.Build;
using Uno.Build.Packages;
using Uno.Logging;
using Uno.ProjectFormat;
using Uno.TestRunner.BasicTypes;
using Uno.TestRunner.Loggers;

namespace Uno.CompilerTestRunner
{
    public class CompilerTestsRunner
    {
        protected string[] TestSuiteDirectories;

        public CompilerTestsRunner(string path, string filter=null)
        {
            TestSuiteDirectories = Directory.GetDirectories(path);
            TestSuiteDirectories = TestSuiteDirectories.Where(f =>
                {
                    var filename = Path.GetFileName(f);
                    return !filename.StartsWith("_") && !filename.StartsWith(".");
                })
                .ToArray();

            if (filter != null)
                TestSuiteDirectories = TestSuiteDirectories.Where(f => f.Contains(filter)).ToArray();
            if (!TestSuiteDirectories.Any())
                TestSuiteDirectories = new[] { path };
        }

        public void Run(ITestResultLogger logger)
        {
            foreach (var testSuiteDir in TestSuiteDirectories)
                RunTestSuite(logger, testSuiteDir);
        }

        private void RunTestSuite(ITestResultLogger logger, string directoryName)
        {
            var project = new Project(Path.Combine(directoryName, Path.GetFileName(directoryName) + ".unoproj"));
            project.MutablePackageReferences.Add(new PackageReference(project.Source, "Uno.Testing"));
            project.MutableProjectReferences.Add(new ProjectReference(project.Source, "../_Outracks.UnoTest.InternalHelpers/_Outracks.UnoTest.InternalHelpers.unoproj"));

            logger.ProjectStarting(project.Name, BuildTargets.Default.Identifier);
            var tests = new List<Test>();

            foreach (var file in Directory.GetFiles(directoryName, "*.uno").Where(x => x.EndsWith(".uno")))
            {
                var filePath = Path.GetFullPath(file);
                project.MutableIncludeItems.Clear();
                var fileName = Path.GetFileName(filePath);
                project.MutableIncludeItems.Add(new IncludeItem(project.Source, IncludeItemType.Source, fileName));

                var test = new Test(fileName);
                tests.Add(test);

                var expectations = ParseAssertComment(filePath).ToList();
                var ignores = expectations.Where(e => e.Type == ErrorType.Ignore).ToList();
                expectations = expectations.Except(ignores).ToList();

                foreach (var ignore in ignores)
                    logger.TestIgnored(new Test(ignore.ToString()));

                var output = RunBuild(project);
                var errors = ParseOutput(output);
                var unexpectedErrors = errors.Except(expectations, new ErrorItemComparer()).ToList();
                var notGivenErrors = expectations.Except(errors, new ErrorItemComparer()).ToList();

                foreach (var error in unexpectedErrors)
                {
                    test.Asserted(new Assertion(error.Source.FullPath, error.Source.Line, null, "(Got an unexpected error)", error.ToString(), error.Expression));
                    logger.TestAsserted(test);
                }

                foreach (var error in notGivenErrors)
                {
                    test.Asserted(new Assertion(error.Source.FullPath, error.Source.Line, null, error.ToString(), "(Missed an expected error)", error.Expression));
                    logger.TestAsserted(test);
                }

                if (!unexpectedErrors.Any() && !notGivenErrors.Any())
                {
                    test.Passed();
                    logger.TestPassed(test);
                }
            }

            logger.ProjectEnded(tests);
            logger.Log("");
        }

        private static string RunBuild(Project project)
        {
            var output = new StringBuilder();
            using (var w = new StringWriter(output))
                new ProjectBuilder(
                        new Log(w), 
                        BuildTargets.Default, 
                        new BuildOptions
                        {
                            Test = true,
                            PackageCache = new PackageCache(),
                            Force = true
                        })
                    .Build(project);

            return output.ToString();
        }

        private static IEnumerable<ErrorItem> ParseOutput(string buildOutput)
        {
            var result = new List<ErrorItem>();
            var regex = new Regex(@"^(?<source>(\w:)?[\w\\/.-]+?)\((?<line>\d+?)(.\d+)?\): (?<code>\w+?): (?<message>.*?)$");

            foreach (var line in buildOutput.Split('\r', '\n'))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (line.StartsWith("Build completed"))
                    break;

                var matches = regex.Matches(line);
                if (matches.Count == 1)
                {
                    var parts = matches[0].Groups;
                    var errorCode = ParseErrorCode(parts["code"].Value);
                    var source = Path.GetFullPath(parts["source"].Value);
                    var lineNumber = int.Parse(parts["line"].Value);
                    var message = parts["message"].Value;
                    result.Add(new ErrorItem(ErrorType.Actual, message, new Source(source, lineNumber), errorCode));
                }
            }
            return result;
        }

        private static IEnumerable<ErrorItem> ParseAssertComment(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var commentRegex = new Regex(@"^\s*(?<line>.*?)\s*//\s*(?<assert>\$.*?)\s*$");
            var expectationRegex = new Regex(@"\$(?<code>[^\s\$]*)\s*(?<ignore>\[Ignore\])?(?<message>[^\$]*)\s*");
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var match = commentRegex.Match(line);
                if (match.Success)
                {
                    var expectations = expectationRegex.Matches(match.Groups["assert"].Value);
                    foreach (Match expectation in expectations)
                    {
                        var code = ParseErrorCode(expectation.Groups["code"].Value);
                        var source = new Source(filePath, i + 1);
                        var message = expectation.Groups["message"].Value.Trim();
                        var type = string.IsNullOrEmpty(expectation.Groups["ignore"].Value) ? ErrorType.Expected : ErrorType.Ignore;
                        var expression = match.Groups["line"].Value;
                        yield return new ErrorItem(type, message, source, code, expression);
                    }
                }
            }
        }

        private static string ParseErrorCode(string macro)
        {
            return (macro.Length > 1 ? macro : macro + "0000").ToUpper();
        }
    }
}