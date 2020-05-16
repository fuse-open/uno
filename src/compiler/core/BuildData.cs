using Uno.Compiler.API;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL;
using Uno.Compiler.Core.IL.Building.Entrypoint;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Logging;

namespace Uno.Compiler.Core
{
    public class BuildData : LogObject, IBuildData
    {
        readonly ILFactory _ilf;

        public ExtensionRoot Extensions { get; }
        public Namespace IL { get; }
        public DataType MainClass { get; private set; }
        public Method Entrypoint { get; private set; }
        public Scope StartupCode { get; }

        internal BuildData(
            Namespace il,
            ExtensionRoot extensions,
            ILFactory ilf)
            : base(ilf)
        {
            _ilf = ilf;
            IL = il;
            Extensions = extensions;
            MainClass = DataType.Invalid;
            StartupCode = new Scope();
        }

        internal void SetMainClass(DataType dt)
        {
            var ctor = dt.TryGetDefaultConstructor();

            if (!dt.IsSubclassOf(_ilf.Essentials.CoreApp))
                Log.Error(dt.Source, ErrorCode.E0000, "Main class must be a sub class of " + _ilf.Essentials.Application.Quote());
            else if (dt.IsAbstract)
                Log.Error(dt.Source, ErrorCode.E0000, "Main class cannot be declared 'abstract'");
            else if (dt.IsGenericType)
                Log.Error(dt.Source, ErrorCode.E0000, "Main class cannot be a generic class");
            else if (ctor == null || !ctor.IsPublic || !ctor.IsConstructor)
                Log.Error(dt.Source, ErrorCode.E3501, "Main class requires a parameterless public constructor");
            else
                StartupCode.Statements.Add(new NewObject(Source.Unknown, (Constructor) ctor));

            MainClass = dt;
        }

        internal void ResolveMainClass(CompilerPass parent, BuildEnvironment environment)
        {
            if (MainClass != null && MainClass != DataType.Invalid)
                return;
            if (!string.IsNullOrEmpty(environment.Options.MainClass))
                SetMainClass(_ilf.GetType(environment.Options.MainClass));
            else
                new MainClassFinder(parent).Run();
        }

        internal void CreateEntrypoint()
        {
            Entrypoint = new Method(MainClass.Source,
                new ClassType(MainClass.Source, IL, null, Modifiers.Public | Modifiers.Static, ".program"),
                null, Modifiers.Public | Modifiers.Static | Modifiers.Generated,
                ".main", DataType.Void, ParameterList.Empty, StartupCode);
        }
    }
}
