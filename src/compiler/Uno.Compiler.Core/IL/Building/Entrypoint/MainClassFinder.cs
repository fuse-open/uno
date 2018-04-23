using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.IL.Building.Entrypoint
{
    class MainClassFinder : CompilerPass
    {
        readonly List<DataType> FoundMainClasses = new List<DataType>();

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
                    var extraMsg = Package.Name.EndsWith("Test") ? ". If this is a test project, it cannot be started directly, but must be run with a test runner." : "";
                    Log.Error(Package.Source, ErrorCode.E3503, "No non-abstract application classes found in project" + extraMsg);
                    break;
                default:
                    var mainClass = BestMainClassSelector.GetBestMainClass(FoundMainClasses, Log, Package, Essentials.MainClassAttribute);
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
