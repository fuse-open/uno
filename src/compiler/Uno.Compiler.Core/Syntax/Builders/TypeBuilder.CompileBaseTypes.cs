using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        void CompileBaseTypes(DataType def, IReadOnlyList<AstExpression> baseTypes)
        {
            var parameterizedType = Parameterize(def);

            // Compile base type and interfaces
            DataType baseType = null;
            var interfaces = new List<InterfaceType>();
            var blocks = new List<Block>();

            if (baseTypes != null && baseTypes.Count > 0)
            {
                foreach (var bt in baseTypes)
                {
                    var p = _resolver.ResolveExpression(parameterizedType, bt, null);

                    if (p.IsInvalid)
                        continue;

                    switch (p.ExpressionType)
                    {
                        case PartialExpressionType.Block:
                            {
                                var block = _resolver.GetBlock(p, bt);

                                if (!parameterizedType.IsClass)
                                    Log.Error(bt.Source, ErrorCode.E0000, parameterizedType.Quote() + " cannot use a block because it is not a class");
                                else if (parameterizedType.IsStatic)
                                    Log.Error(baseTypes[0].Source, ErrorCode.E0000, parameterizedType.Quote() + " cannot use a block because it is a static class");
                                else if (def != parameterizedType)
                                    Log.Error(baseTypes[0].Source, ErrorCode.E0000, parameterizedType.Quote() + " cannot use a block because it is a generic class");
                                else
                                    blocks.Add(block);
                            }
                            break;

                        case PartialExpressionType.Type:
                            {
                                var dt = _resolver.GetType(p, bt);

                                switch (dt.TypeType)
                                {
                                    case TypeType.Class:
                                        if (!(parameterizedType.IsClass || parameterizedType.IsGenericParameter))
                                            Log.Error(bt.Source, ErrorCode.E3016, parameterizedType.Quote() + " cannot inherit a base class because it is not a class");
                                        else if (parameterizedType.IsStatic)
                                            Log.Error(baseTypes[0].Source, ErrorCode.E3015, parameterizedType.Quote() + " cannot inherit a base type because it is a static class");
                                        else if (!(baseType == null || def.IsPartial && dt == baseType))
                                            Log.Error(bt.Source, ErrorCode.E3017, parameterizedType.Quote() + " can only inherit one base class");
                                        else if ((interfaces.Count > 0 || blocks.Count > 0) && !def.IsPartial)
                                            Log.Error(bt.Source, ErrorCode.E0000, "Base type must be specified before any interfaces or blocks");
                                        else
                                            baseType = dt;

                                        break;

                                    case TypeType.Interface:
                                        if (parameterizedType.IsStatic)
                                            Log.Error(baseTypes[0].Source, ErrorCode.E3015, parameterizedType.Quote() + " cannot implement a interface because it is a static class");
                                        else
                                        {
                                            var interfaceType = (InterfaceType) dt;
                                            if (!interfaces.Contains(interfaceType)) // Avoid implementing same interface twice
                                            {
                                                interfaces.Add(interfaceType);
                                            }
                                        }

                                        break;

                                    case TypeType.Invalid:
                                        break;

                                    default:
                                        Log.Error(bt.Source, ErrorCode.E3018, dt.Quote() + " is not a class or interface");
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

            if ((baseType != null || interfaces.Count > 0 || parameterizedType.Constructors.Count > 0) && 
                    parameterizedType.IsGenericParameter && 
                    (parameterizedType as GenericParameterType).ConstraintType == 0)
                (parameterizedType as GenericParameterType).SetConstraintType(GenericConstraintType.Class);

            if (baseType != null)
                parameterizedType.SetBase(baseType);
            else if (parameterizedType.IsStruct)
                parameterizedType.SetBase(_ilf.Essentials.ValueType);
            else if (parameterizedType != _ilf.Essentials.Object && !parameterizedType.IsStatic)
                parameterizedType.SetBase(_ilf.Essentials.Object);

            if (interfaces.Count > 0)
            {
                // Flatten subinterfaces (this is order sensitive, hence this do..while loop)
                int i = 0;

                do
                {
                    for (int c = interfaces.Count; i < c; i++)
                    {
                        var it = interfaces[i];
                        it.AssignBaseType();

                        foreach (var st in it.Interfaces)
                            if (!interfaces.Contains(st))
                                interfaces.Add(st);
                    }
                }
                while (interfaces.Count > i);

                if (parameterizedType is InterfaceType && interfaces.Remove(parameterizedType as InterfaceType))
                    Log.Error(parameterizedType.Source, ErrorCode.E0000, parameterizedType.Quote() + " can't implement itself (circular reference)");

                parameterizedType.SetInterfaces(interfaces.ToArray());
            }

            if (blocks.Count > 0)
                def.Block.SetUsingBlocks(blocks.ToArray());

            if (DetectCircularInheritance(parameterizedType))
            {
                parameterizedType.SetBase(DataType.Invalid);
                parameterizedType.SetInterfaces();
            }

            if (def != parameterizedType)
            {
                def.SetBase(parameterizedType.Base);
                def.SetInterfaces(parameterizedType.Interfaces);
            }
        }

        bool DetectCircularInheritance(DataType dt)
        {
            // Look for circular inheritance (must be done here to prevent stack overflows later)
            var baseTypes = new HashSet<DataType>() { dt.MasterDefinition };

            var bt = dt.Base;
            while (bt != null)
            {
                if (baseTypes.Contains(bt.MasterDefinition))
                {
                    Log.Error(dt.Source, ErrorCode.E3048, "Circular base class\n  * " + string.Join("\n  * ", baseTypes));
                    return true;
                }

                baseTypes.Add(bt.MasterDefinition);
                bt = bt.Base;
            }

            // TODO: Detect circular blocks
            // TODO: Detect circular interfaces

            return false;
        }
    }
}
