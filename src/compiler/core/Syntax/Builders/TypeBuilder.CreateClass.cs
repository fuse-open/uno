using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        public void CreateClass(AstClass astClass, Namescope parent, IEnumerable<AstBlockMember> parentItems)
        {
            var sources = new List<string>();

            if (astClass.Modifiers.HasFlag(Modifiers.Partial))
                astClass = FlattenClass(astClass, sources, parentItems);

            DataType result;
            if (_cachedClasses.TryGetValue(astClass, out result))
                return;

            var src = astClass.Name.Source;
            var modifiers = GetTypeModifiers(parent, astClass.Modifiers);

            switch (astClass.Type)
            {
                case AstClassType.Struct:
                    result = new StructType(src, parent, astClass.DocComment, modifiers, astClass.Name.Symbol);
                    break;
                case AstClassType.Class:
                    result = new ClassType(src, parent, astClass.DocComment, modifiers, astClass.Name.Symbol);
                    break;

                case AstClassType.Interface:
                    if (modifiers.HasFlag(Modifiers.Abstract))
                        Log.Error(src, ErrorCode.E0000, "'abstract' is not valid for interface");

                    result = new InterfaceType(src, parent, astClass.DocComment, modifiers | Modifiers.Abstract, astClass.Name.Symbol);
                    break;

                default:
                    Log.Error(src, ErrorCode.I3045, "<" + astClass.Type + "> is not a class, struct or interface type");
                    return;
            }

            if (parent is DataType)
                (parent as DataType).NestedTypes.Add(result);
            else if (parent is Namespace)
                (parent as Namespace).Types.Add(result);
            else
                Log.Error(result.Source, ErrorCode.I3046, "<" + astClass.Type + "> is not allowed in this context");

            _cachedClasses.Add(astClass, result);

            if (astClass.OptionalGeneric != null)
                CreateGenericSignature(result, astClass.OptionalGeneric, false);

            result.SetBlock(new Block(src, result, null, result.Modifiers & Modifiers.ProtectionModifiers, ".block"));

            foreach (var s in sources)
                result.SourceFiles.Add(s);

            foreach (var b in astClass.Members)
                if (b is AstBlockBase)
                    _compiler.AstProcessor.CreateBlock(b as AstBlockBase, result, astClass.Members);

            if (astClass.Attributes.Count > 0)
                EnqueueAttributes(result, x =>
                {
                    result.SetAttributes(_compiler.CompileAttributes(result.Parent, astClass.Attributes));

                    // Remove default constructor if TargetSpecificType
                    if (result.HasAttribute(_ilf.Essentials.TargetSpecificTypeAttribute) &&
                        result.Constructors.Count == 1 && result.Constructors[0].IsGenerated)
                        result.Constructors.Clear();
                });

            EnqueueType(result,
                x => CompileBaseTypes(x, astClass.Bases),
                x => PopulateClass(astClass, x));
        }
    }
}
