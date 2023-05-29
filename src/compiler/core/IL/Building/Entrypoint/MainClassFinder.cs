using System;
using System.Collections.Generic;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Building.Entrypoint
{
    class MainClassFinder : CompilerPass
    {
        readonly List<DataType> FoundMainClasses = new();

        public MainClassFinder(CompilerPass parent)
            : base(parent)
        {
        }

        public override void End()
        {
            switch (FoundMainClasses.Count)
            {
                case 1:
                    Data.SetMainClass(FoundMainClasses[0]);
                    break;
                case 0:
                    // Auto-generate main-class when building a library.
                    if (Environment.IsLibrary || Backend.BuildType == BuildType.Library)
                    {
                        var type = new ClassType(Bundle.Source, Data.IL, null, Modifiers.Generated | Modifiers.Public, Bundle.Name.ToIdentifier() + "_app");
                        type.SetBase(Essentials.Application);
                        type.Constructors.Add(new Constructor(Bundle.Source, type, null, Modifiers.Generated | Modifiers.Public, Array.Empty<Parameter>(), new Scope()));
                        Data.IL.Types.Add(type);
                        Data.SetMainClass(type);
                        break;
                    }

                    var extraMsg = Bundle.Name.EndsWith("Test") || Environment.IsTest
                        ? ". If this is a test project, it cannot be started directly, but must be run with a test runner."
                        : "";
                    Log.Error(Bundle.Source, ErrorCode.E3503, "No non-abstract application classes found in project" + extraMsg);
                    break;
                default:
                    var mainClass = BestMainClassSelector.GetBestMainClass(FoundMainClasses, Log, Bundle, Essentials.MainClassAttribute);
                    if (mainClass != null)
                        Data.SetMainClass(mainClass);
                    break;
            }
        }

        public override bool Begin(DataType dt)
        {
            if (dt.IsClass &&
                !dt.IsAbstract &&
                !dt.HasAttribute(Essentials.IgnoreMainClassAttribute) &&
                dt.IsSubclassOf(Essentials.Application))
                FoundMainClasses.Add(dt);

            return true;
        }

        public override bool Begin(Function f)
        {
            return false;
        }
    }
}
