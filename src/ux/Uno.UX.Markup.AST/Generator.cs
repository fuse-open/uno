using System;

namespace Uno.UX.Markup.AST
{
    public abstract class Generator
    {
        public static Generator Resolve(FileSourceInfo source, AST.Element e, ContentMode contentMode, InstanceType instanceType, Common.IMarkupErrorLog log)
        {
            var g = e.Generator;

            if (g is UnspecifiedGenerator)
            {
                var ug = (UnspecifiedGenerator)g;
                if (contentMode == ContentMode.Default || e.UXKey != null)
                {
                    if (instanceType == InstanceType.Local)
                    {
                        return new InstanceGenerator();
                    }
                    else if (instanceType == InstanceType.Global)
                    {
                        return new GlobalInstanceGenerator(null);
                    }
                    else
                    {
                        throw new Exception("Unhandled instance type: " + instanceType);
                    }
                }
                else if (contentMode == ContentMode.Template)
                {
                    var t = (UnspecifiedGenerator)g;
                    return new TemplateGenerator(null, t.Case, t.IsDefaultCase);
                }
                else
                {
                    log.ReportError(source.FileName, source.LineNumber, "Unknown content mode: " + contentMode);
                    return new InstanceGenerator();
                }
            }
            else
            {
                return g;
            }
        }
    }

    public sealed class UnspecifiedGenerator: Generator
    {
        public string Case { get; }
        public bool IsDefaultCase { get; }

        public UnspecifiedGenerator(string caseMatch, bool isDefaultCase)
        {
            Case = caseMatch;
            IsDefaultCase = isDefaultCase;
        }
    }

    public class InstanceGenerator : Generator
    {
    }

    public sealed class GlobalInstanceGenerator : InstanceGenerator
    {
        public string Name { get; private set; }

        internal GlobalInstanceGenerator(string name)
        {
            Name = name;
        }
    }

    public sealed class ClassGenerator : Generator
    {
        public bool IsInnerClass { get; private set; }
        public bool AutoCtor { get; private set; }
        public bool Simulate { get; private set; }
        public string ClassName { get; private set; }

        internal ClassGenerator(bool isInnerClass, string className, bool autoCtor, bool simulate)
        {
            IsInnerClass = isInnerClass;
            AutoCtor = autoCtor;
            Simulate = simulate;
            ClassName = className;
        }
    }

    public sealed class TemplateGenerator : InstanceGenerator
    {
        public string ClassName { get; private set; }
        public string Case { get; private set; }
        public bool IsDefaultCase { get; private set; }

        internal TemplateGenerator(string className, string caseMatch, bool isDefaultCase)
        {
            ClassName = className;
            Case = caseMatch;
            IsDefaultCase = isDefaultCase;
        }
    }

    /// <summary>
    /// Generates an UnoTest Test
    /// </summary>
    public sealed class TestGenerator: Generator
    {
        public string TestName { get; }
        internal TestGenerator(string testName)
        {
            TestName = testName;
        }

    }

    

    

}
