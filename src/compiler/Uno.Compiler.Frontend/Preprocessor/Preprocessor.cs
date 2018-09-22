using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.Frontend.Analysis;
using Uno.Logging;
using Uno.Macros;

namespace Uno.Compiler.Frontend.Preprocessor
{
    public static class Preprocessor
    {
        public static string Process(Source src, string text, bool escape, object context, Func<Source, string, bool, object, string> expandMacros, bool acceptErrors = false)
        {
            return expandMacros(src, ExpandDirectives(src, text, escape, context, expandMacros, acceptErrors), escape, context);
        }

        public static string ExpandDirectives(Source src, string text, bool escape, object context, Func<Source, string, bool, object, string> expandMacros, bool acceptErrors)
        {
            var stack = new List<TestPart>();

            for (int i = 0;;)
            {
                string ppValue;
                int ppStart, ppEnd;

                if (!TryFindPreprocessorLine(text, i, out ppStart, out ppEnd, out ppValue))
                {
                    if (stack.Count > 0)
                        throw new SourceException(text.CreateSource(src, 0, -1), "No '#endif' found while parsing '#if' at " + text.CreateSource(src, 0, stack.First().LineStart));

                    return text;
                }

                if (ppValue == "endif")
                {
                    var parts = new List<TestPart>();

                    while (stack.Count > 0 && stack.Last().Type != TestType.If)
                        parts.Add(stack.RemoveLast());

                    if (stack.Count == 0 || stack.Last().Type != TestType.If)
                        throw new SourceException(text.CreateSource(src, 0, ppStart), "Invalid '#endif' found without an '#if <condition>'");

                    parts.Add(stack.RemoveLast());

                    for (int j = parts.Count - 1; j >= 0; j--)
                    {
                        var test = parts[j];

                        // Ignore this, must be acceptErrors
                        if (!test.Condition.HasValue)
                            break;

                        if (test.Condition.Value || j == 0)
                        {
                            var sb = new StringBuilder();

                            var ifTest = parts.Last();
                            sb.Append(text, 0, ifTest.LineStart);

                            if (test.Condition.Value)
                            {
                                var nextStart = j > 0 ? parts[j - 1].LineStart : ppStart;
                                sb.Append(text.CreateComment(src, ifTest.LineStart, test.LineEnd));
                                sb.Append(text, test.LineEnd, nextStart - test.LineEnd);
                                sb.Append(text.CreateComment(src, nextStart, ppEnd));
                            }
                            else
                                sb.Append(text.CreateComment(src, ifTest.LineStart, ppEnd));

                            ppStart = sb.Length;
                            sb.Append(text, ppEnd, text.Length - ppEnd);
                            text = sb.ToString();
                            ppEnd = ppStart;

                            break;
                        }
                    }
                }
                else if (ppValue == "else")
                {
                    if (stack.Count == 0)
                        throw new SourceException(text.CreateSource(src, 0, ppStart), "Invalid '#else' found without an '#if <condition>'");
                    if (stack.Last().Type == TestType.Else)
                        throw new SourceException(text.CreateSource(src, 0, ppStart), "Invalid '#else' found inside '#else', expecting '#endif'");

                    stack.Add(new TestPart(TestType.Else, true, ppStart, ppEnd));
                }
                else if (ppValue.StartsWith("elif ", StringComparison.InvariantCulture))
                {
                    if (stack.Count == 0)
                        throw new SourceException(text.CreateSource(src, 0, ppStart), "Invalid '#elif' found without an '#if <condition>'");
                    if (stack.Last().Type == TestType.Else)
                        throw new SourceException(text.CreateSource(src, 0, ppStart), "Invalid '#elif' found inside '#else', expecting '#endif'");

                    stack.Add(new TestPart(TestType.ElseIf, ParseCondition(ppValue.Substring(5), text.CreateSource(src, 0, ppStart), escape, context, expandMacros, acceptErrors), ppStart, ppEnd));
                }
                else if (ppValue.StartsWith("if ", StringComparison.InvariantCulture))
                {
                    stack.Add(new TestPart(TestType.If, ParseCondition(ppValue.Substring(3), text.CreateSource(src, 0, ppStart), escape, context, expandMacros, acceptErrors), ppStart, ppEnd));
                }
                else if (!acceptErrors)
                {
                    throw new SourceException(text.CreateSource(src, 0, ppStart), "Syntax error in " + ppValue.Quote());
                }
                else if (ppValue.StartsWith("ifdef ", StringComparison.InvariantCulture) || 
                         ppValue.StartsWith("ifndef ", StringComparison.InvariantCulture))
                {
                    stack.Add(new TestPart(TestType.If, null, ppStart, ppEnd));
                }

                i = ppEnd;
            }
        }

        static bool? ParseCondition(string value, Source src, bool escape, object context, Func<Source, string, bool, object, string> expandMacros, bool acceptErrors)
        {
            bool cond;
            if (TryParseCondition(expandMacros(src, value, escape, context), out cond))
                return cond;

            if (acceptErrors)
                return null;

            throw new SourceException(src, "Syntax error in conditional expression " + value.Quote());
        }

        public static bool TryParseCondition(string value, out bool result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = false;
                return false;
            }

            var log = new Log(TextWriter.Null);
            var e = Parser.ParseExpression(log, Source.Unknown, value.ToLower());

            if (!log.HasErrors && TryEvaluateConditionRecursive(e, out result))
                return true;

            result = false;
            return false;
        }

        static bool TryEvaluateConditionRecursive(AstExpression e, out bool result)
        {
            switch (e.ExpressionType)
            {
                case AstExpressionType.Int:
                    result = ((AstInt) e).Value != 0;
                    return true;
                case AstExpressionType.True:
                    result = true;
                    return true;
                case AstExpressionType.Zero:
                case AstExpressionType.False:
                    result = false;
                    return true;
                case AstExpressionType.LogNot:
                {
                    if (TryEvaluateConditionRecursive(((AstUnary) e).Operand, out result))
                    {
                        result = !result;
                        return true;
                    }
                    break;
                }
                case AstExpressionType.LogOr:
                {
                    if (TryEvaluateConditionRecursive(((AstBinary) e).Left, out result))
                    {
                        if (result)
                            return true;
                        if (TryEvaluateConditionRecursive(((AstBinary) e).Right, out result))
                            return true;
                    }
                    break;
                }
                case AstExpressionType.LogAnd:
                {
                    if (TryEvaluateConditionRecursive(((AstBinary) e).Left, out result))
                    {
                        if (!result)
                            return true;
                        if (TryEvaluateConditionRecursive(((AstBinary) e).Right, out result))
                            return true;
                    }
                    break;
                }
                case AstExpressionType.Equal:
                {
                    int left, right;
                    if (TryGetInt(((AstBinary) e).Left, out left) &&
                        TryGetInt(((AstBinary) e).Right, out right))
                    {
                        result = left == right;
                        return true;
                    }
                    return result = false;
                }
                case AstExpressionType.NotEqual:
                {
                    int left, right;
                    if (TryGetInt(((AstBinary) e).Left, out left) &&
                        TryGetInt(((AstBinary) e).Right, out right))
                    {
                        result = left != right;
                        return true;
                    }
                    return result = false;
                }
            }

            result = false;
            return false;
        }

        static bool TryGetInt(AstExpression e, out int result)
        {
            switch (e.ExpressionType)
            {
                case AstExpressionType.Zero:
                    result = 0;
                    return true;
                case AstExpressionType.Int:
                    result = ((AstInt) e).Value;
                    return true;
                default:
                    result = 0;
                    return false;
            }
        }

        /// <summary>
        /// Finds a preprocessor line in text.
        /// </summary>
        ///
        /// This function understands single- and double quoted strings, C-style
        /// in-line comments (also including multi-line comments) and C++-style
        /// end-of-line comments.
        ///
        /// An arbitrary sequence of comments and characters on which
        /// char.IsWhiteSpace returns true is treated as whitespace (WS, below).
        ///
        /// A preprocessor is then defined by the occurrence of the (pseudo)
        /// regular expression outside comments and quoted strings:
        ///
        ///     ^ WS '#' WS {ppValue} WS ('\n' | $)
        ///
        /// ppValue is an arbitrary string with no leading or trailing
        /// whitespace, and no line-breaks outside C-style multi-line comments
        /// or quoted strings.
        ///
        /// <param name="text">Source string to search in</param>
        /// <param name="start">Index of text where search will start</param>
        /// <param name="ppStart">If a preprocessor line is found, set to index
        /// of line start</param>
        /// <param name="ppEnd">If a preprocessor line is found, set to index
        /// of first character after line. This can be used to start a new
        /// search.</param>
        /// <param name="ppValue">If preprocessor line is found, set to the
        /// preprocessor directive, i.e., text after the '#'. Leading and
        /// trailing whitespace, including comments is stripped. Other
        /// whitespace, including comments, is left as is. </param>
        ///
        /// <returns>True if a preprocessor line was found, in which case,
        /// ppStart, ppEnd and ppValue will be set. When no preprocessor line is
        /// found, the contents of ppStart and ppEnd are undefined, ppValue is
        /// set to null.</returns>
        static bool TryFindPreprocessorLine(string text, int start, out int ppStart, out int ppEnd, out string ppValue)
        {
            //  Variable        Value   Invariant
            //  ----------------------------------------------------------------
            //  ppStart         -1      '#' on line will not start directive
            //                  index   start of line for directive,
            //
            //  ppValueStart    -1      no directive found
            //                  index   directive found, first non-whitespace
            //                          after '#'
            //  ppValueEnd              valid in directive, ppValueStart != -1
            //                  -1      empty directive (whitespace, so far)
            //                  index   whitespace (e.g. end-of-line) at end of
            //                          directive or text.Length.

            int ppValueStart = -1;
            int ppValueEnd = -1;

            ppStart = start;
            ppEnd = text.Length;

            for (int i = start; i < ppEnd; ++i) {
                char ch = text[i];

                // No state change on whitespace (including comments)

                if (char.IsWhiteSpace(ch)) {
                    if (ch == '\n') {
                        if (ppValueStart != -1) {
                            // Found directive
                            ppEnd = i + 1;
                            break;
                        }

                        ppStart = i + 1;
                    }
                    continue;
                }

                if (ch == '/' && (i < ppEnd - 1)
                        && (text[i + 1] == '*' || text[i + 1] == '/')) {
                    // Comment, still whitespace

                    if (text[i] == '*') {
                        // C-style inline comment
                        for (i += 2; i < ppEnd - 1; ++i) {
                            if (text[i] == '*' && text[i + 1] == '/') {
                                ++i;
                                break;
                            }
                        }
                        continue;
                    }

                    // C++-style end-of-line comment
                    // Leave '\n' to be handled further down.
                    for (++i; i < ppEnd - 1; ++i) {
                        if (text[i + 1] == '\n')
                            break;
                    }
                    continue;
                }

                // Non-whitespace, state change allowed.

                if (ppValueStart == -1) {
                    if (ch == '#') {
                        if (ppStart != -1) {
                            // Directive start
                            ppValueStart = i + 1;
                            ppValueEnd = -1;
                        }
                        continue;
                    }

                    // No directive in this line
                    ppStart = -1;
                } else if (ppValueEnd == -1) {
                    // First non-whitespace character in directive
                    ppValueStart = i;
                }

                // Skip to (potential) whitespace
                for (;; ++i) {
                    if (ch == '"' || ch == '\'') {
                        // Skip to end of quoted string
                        for (++i; i < ppEnd - 1; ++i) {
                            if (text[i] == ch)
                                break;
                            if (text[i] == '\\')
                                ++i;
                        }
                    }

                    if (i + 1 >= ppEnd) {
                        ppValueEnd = ppEnd;
                        break;
                    }

                    // Check for whitespace ahead
                    ch = text[i + 1];
                    if (char.IsWhiteSpace(ch) || ch == '/') {
                        ppValueEnd = i + 1;
                        break;
                    }
                }
            }

            if (ppValueStart == -1) {
                ppValue = null;
                return false;
            }

            ppValue = ppValueEnd == -1
                ? ""
                : text.Substring(ppValueStart, ppValueEnd - ppValueStart);

            return true;
        }

        static string CreateComment(this string text, Source src, int start, int end)
        {
            string prefix, suffix;
            return end > start && GetCommentDecoration(src, out prefix, out suffix)
                ? prefix + text.Substring(start, end - start - 1).EscapeComment(suffix).Replace("\n", suffix + "\n" + prefix) + suffix + "\n"
                : new string('\n', text.CountNewlines(start, end));
        }

        static string EscapeComment(this string text, string suffix)
        {
            text = text.Replace('@', '#'); // Obfuscate macros to avoid parsing
            return !string.IsNullOrEmpty(suffix)
                ? text.Replace(suffix, "") // Strip comment suffix to avoid syntax errors
                : text;
        }

        static bool GetCommentDecoration(Source src, out string prefix, out string suffix)
        {
            prefix = "";
            suffix = "";

            switch (Path.GetExtension(src.FullPath ?? "").ToUpperInvariant())
            {
                case ".C":
                case ".CC":
                case ".CPP":
                case ".CS":
                case ".GRADLE":
                case ".H":
                case ".JAVA":
                case ".JS":
                case ".M":
                case ".MM":
                case ".PBXPROJ":
                case ".UNO":
                case ".UXL":
                    prefix = "//";
                    return true;
                case ".BASH":
                case ".PROPERTIES":
                case ".SH":
                case ".TXT":
                    prefix = "#";
                    return true;
                case ".BAT":
                case ".CMD":
                    prefix = ":: ";
                    return true;
                case ".MD":
                case ".PLIST":
                    prefix = "<!--";
                    suffix = "-->";
                    return true;
            }

            switch (Path.GetFileName(src.FullPath))
            {
                case "Makefile":
                    prefix = "#";
                    return true;
            }

            return false;
        }
    }
}
