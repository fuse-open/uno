using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class CommentParser : ICommentParser
    {
        private static readonly HashSet<string> SupportedMacros = new HashSet<string>(new[]
        {
            "advanced",
            "scriptmodule",
            "scriptproperty",
            "scriptevent",
            "scriptmethod",
            "return",
            "published",
            "param",
            "default",
            "seealso",
            "see",
            "mount",
            "experimental",
            "readonly",
            "include",
            "ux",
            "remarks",
            "examples",
            "ux-property",
            "topic",
            "deprecated"
        });

        private const string DefaultIndentation = "    ";
        private static readonly Regex LeadingLineBreaksPattern = new Regex(@"^[\r\n]+", RegexOptions.Compiled);
        private static readonly Regex TrailingLineBreaksPattern = new Regex(@"[\r\n]+$", RegexOptions.Compiled);
        private static readonly Regex MultipleLineBreaksPattern = new Regex(@"\n{2,}", RegexOptions.Compiled);
        private static readonly Regex TabsPattern = new Regex(@"\t", RegexOptions.Compiled);

        private readonly IDictionary<string, SourceComment> _rawCache = new Dictionary<string, SourceComment>();

        public SourceComment Read(SourceObject entity)
        {
            var ctor = entity as Constructor;
            if (ctor != null && ctor.IsGenerated)
            {
                return GetGeneratedConstructorDefaultComment(ctor);
            }

            var method = entity as Method;
            if (method?.IsGenerated ?? false) // Ignore generated methods
            {
                return new SourceComment();
            }
            if (method?.OverriddenMethod != null)
            {
                return ReadInheritedMethodComment(method);
            }

            // Special case for partial classes. The Source property here points to the first file found
            // by the compiler, which is not neccesarily what we want. In these cases, as a hack, we load
            // all the files, find a line containing "class <data type name>", and pick the comment ending
            // on the previous line, if any.
            var dataType = entity as DataType;
            if (dataType?.IsGenerated ?? false) // Ignore generated data types
            {
                return new SourceComment();
            }

            // Try to read the comment on the elment itself
            var entityInstance = entity as IEntity;
            if (entityInstance != null)
            {
                var comment = ReadFromEntity(entityInstance);
                if (comment != null && comment.HasValue)
                {
                    return comment;
                }
            }

            // If we couldn't find anything, return an empty comment
            return new SourceComment();
        }

        private SourceComment ReadInheritedMethodComment(Method method)
        {
            var comment = Read(method.OverriddenMethod); // This will read up the hierarchy automatically
            return comment;
        }

        private SourceComment GetGeneratedConstructorDefaultComment(Constructor ctor)
        {
            var declaringType = ctor.DeclaringType;
            var typeName = new EntityNaming().GetIndexTitle(declaringType);
            var comment = $"Creates a new {typeName}";
            return new SourceComment(comment, comment, comment, null, null, null, new List<Tuple<string, StringBuilder>>());
        }

        private SourceComment ReadFromEntity(IEntity entity)
        {
            // Skip empty comments
            if (string.IsNullOrWhiteSpace(entity.DocComment))
            {
                return new SourceComment();
            }

            // If the comment does not contain a comment opening and closing block, skip it
            if (entity.DocComment.Contains("/**") == false || entity.DocComment.Contains("*/") == false)
            {
                return new SourceComment();
            }

            var commentText = entity.DocComment.Trim();

            // If the comment text doesn't start with /**, we might have multiple comments - trim junk off the start of the string
            if (!commentText.StartsWith("/**"))
            {
                commentText = commentText.Substring(commentText.IndexOf("/**", StringComparison.Ordinal));
            }

            commentText = commentText.Substring(0, commentText.IndexOf("*/", StringComparison.Ordinal) + 2);

            if (_rawCache.ContainsKey(commentText))
            {
                return _rawCache[commentText];
            }

            var comment = BuildComment(entity, commentText);
            _rawCache[commentText] = comment;
            return comment;
        }

        private static SourceComment BuildComment(IEntity entity, string commentText)
        {
            var lines = commentText.Trim().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
            var processedLines = ProcessLines(lines);

            var text = new StringBuilder();
            var remarks = new StringBuilder();
            var examples = new StringBuilder();
            var ux = new StringBuilder();

            var macros = new List<Tuple<string, StringBuilder>>();

            StringBuilder lastMacroBuilder = null;
            foreach (var line in processedLines)
            {
                if (LineStartsKnownMacro(line))
                {
                    var macro = ProcessMacro(line.Trim(), text, remarks, examples, ux, entity.Source.File.FullPath, entity.Source.Package.SourceDirectory);
                    macros.Add(macro);
                    lastMacroBuilder = macro.Item2;
                    continue;
                }

                // If one of the previous non-broken lines of text were a macro, and this line has content
                // and is indented, take it as a part of the macro text.
                if (lastMacroBuilder != null && line.Trim().Length > 0 && MeasureIndentation(line) > 0)
                {
                    lastMacroBuilder.AppendLine(line.TrimStart());
                    continue;
                }

                lastMacroBuilder = null;
                text.Append(line.TrimEnd() + "\n");
            }

            var body = text.ToString();

            // Replace instances of more than 2 line breaks with a regular double line break
            // to avoid returning too heavily split up text.
            body = MultipleLineBreaksPattern.Replace(body, "\n\n");

            // Trim unneccesary fuzz
            body = LeadingLineBreaksPattern.Replace(body, "");
            body = TrailingLineBreaksPattern.Replace(body, "");

            var firstParagraph = body.Contains("\n\n")
                                         ? body.Split(new[] { "\n\n" }, StringSplitOptions.None).First()
                                         : body;

            return new SourceComment(string.Join("\n", lines),
                                     firstParagraph,
                                     body.TrimEnd(),
                                     remarks.ToString().Trim().Replace("\r\n", "\n"),
                                     examples.ToString().Trim().Replace("\r\n", "\n"),
                                     ux.ToString().Trim().Replace("\r\n", "\n"),
                                     macros);
        }

        private static bool LineStartsKnownMacro(string text)
        {
            var macro = text.Trim();
            if (!macro.StartsWith("@"))
            {
                return false;
            }

            macro = macro.Substring(1);
            if (macro.Contains(" "))
            {
                macro = macro.Substring(0, macro.IndexOf(" ", StringComparison.Ordinal));
            }

            var result = SupportedMacros.Contains(macro.ToLowerInvariant());
            return result;
        }

        private static Tuple<string, StringBuilder> ProcessMacro(string text,
                                                                 StringBuilder currentComment,
                                                                 StringBuilder currentRemarks,
                                                                 StringBuilder currentExamples,
                                                                 StringBuilder currentUx,
                                                                 string sourcePath,
                                                                 string packageSourceDirectory)
        {
            if (text.Contains(" "))
            {
                var name = text.Substring(0, text.IndexOf(' '));
                var value = text.Substring(name.Length + 1).Trim();

                switch (name)
                {
                    case "@include":
                        var included = ReadFile(value, sourcePath, packageSourceDirectory);
                        currentComment.AppendLine(included);
                        break;

                    case "@remarks":
                        var remarks = ReadFile(value, sourcePath, packageSourceDirectory);
                        currentRemarks.AppendLine(remarks);
                        break;

                    case "@examples":
                        var example = ReadFile(value, sourcePath, packageSourceDirectory);
                        currentExamples.AppendLine(example);
                        break;

                    case "@ux":
                        var code = $"```ux\n{ReadFile(value, sourcePath, packageSourceDirectory)}\n```\n\n";
                        currentUx.AppendLine(code);
                        break;
                }

                return new Tuple<string, StringBuilder>(name, new StringBuilder(value));
            }

            return new Tuple<string, StringBuilder>(text, new StringBuilder());
        }

        private static string ReadFile(string filename, string path, string packageSourceDirectory)
        {
            var filePath = Path.Combine(packageSourceDirectory, filename);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Included file {filename} in {path} was not found in {packageSourceDirectory}");
            }

            return File.ReadAllText(filePath).Replace("\r\n", "\n");
        }

        private static List<string> ProcessLines(List<string> lines)
        {
            var result = new List<string>();

            var textStartsOnCommentStart = false;
            var baseIndentation = -1;

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                // Strip leading comment symbols
                if (i == 0)
                {
                    if (line.Trim().StartsWith("/**")) line = line.TrimStart().Substring(3);
                    if (line.Trim().StartsWith("/*")) line = line.TrimStart().Substring(2);

                    // Lets us know if the text starts on the same line as the comment got opened
                    // For example:
                    //
                    // /** Hello
                    //   World
                    // */
                    //
                    // vs:
                    //
                    // /**
                    //   Hello
                    // */
                    textStartsOnCommentStart = line.Trim().Length > 0;
                }

                // Skip trailing comment symbols
                if (i == lines.Count - 1 && line.TrimEnd().EndsWith("*/"))
                {
                    line = line.TrimEnd();
                    line = line.Substring(0, line.Length - 2);
                }

                if (baseIndentation == -1 && line.Trim().Length > 0)
                {
                    // Try to identify the base indentation of the comment so we can shift things left
                    // and get text appropriately indented the way it's expected to be.
                    //
                    // For example, like this:
                    // /**
                    //    This is some example text
                    //      With multiple lines, some of which are intended.
                    //    Hello.
                    // */
                    //
                    // This should become the string:
                    // "This is some example text\n  With multiple lines, some of which are indented.\nHello."
                    //
                    // Similarly, if the comment look like this:
                    // /** This is some example text
                    //   With multiple lines,
                    //     some of which are intended.
                    //   Hello.
                    // */
                    //
                    // This should become the string:
                    // "This is some example text\nWith multiple lines,\n  some of which are indented.\nHello."

                    // If we're on line 0, and we have text in the same line that the comment got started in,
                    // don't do anything.
                    if (!(i == 0 && textStartsOnCommentStart))
                    {
                        baseIndentation = MeasureIndentation(line);
                    }
                }

                // If it's an empty line, just add it as-is
                if (line.Trim().Length == 0)
                {
                    result.Add("");
                    continue;
                }

                // If the line is indented as the base indentation, or more, strip off the base indentation length.
                // If it's indented less than the base indentation, just trim it and ignore the problem.
                var lineWithNormalizedIndentation = NormalizeIndentation(line);
                var lineIndentation = MeasureIndentation(line);
                if (baseIndentation == -1 || lineIndentation < baseIndentation || baseIndentation > lineWithNormalizedIndentation.Length - 1)
                {
                    line = lineWithNormalizedIndentation.Trim();
                }
                else
                {
                    line = lineWithNormalizedIndentation.Substring(baseIndentation).TrimEnd();
                }
                result.Add(line);
            }

            return result;
        }

        private static string NormalizeIndentation(string line)
        {
            return string.IsNullOrWhiteSpace(line) ? "" : TabsPattern.Replace(line, DefaultIndentation);
        }

        private static int MeasureIndentation(string line)
        {
            var indent = 0;
            foreach (var b in line)
            {
                if (b == ' ') indent++;
                else if (b == '\t') indent += DefaultIndentation.Length;
                else break;
            }
            return indent;
        }
    }
}
