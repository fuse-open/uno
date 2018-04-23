using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        StageValue ProcessFixedArrayDeclaration(FixedArrayDeclaration s)
        {
            Expression[] initializer = null;

            var minStage = MetaStage.Const;
            var maxStage = MetaStage.Max;

            if (s.OptionalInitializer != null)
            {
                var stageValues = new StageValue[s.OptionalInitializer.Length];

                for (int i = 0; i < stageValues.Length; i++)
                {
                    var v = stageValues[i] = ProcessValue(s.OptionalInitializer[i]);

                    if (v.MinStage > minStage)
                        minStage = v.MinStage;

                    if (v.MaxStage < maxStage)
                        maxStage = v.MaxStage;
                }

                initializer = new Expression[stageValues.Length];

                if (maxStage < minStage)
                    maxStage = minStage;

                for (int i = 0; i < stageValues.Length; i++)
                    initializer[i] = ProcessStage(stageValues[i], minStage, maxStage).Value;
            }

            return new StageValue(new PlaceholderArray(s.Source, s.Variable.ValueType, initializer), minStage, maxStage);
        }
    }
}
