using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Generators
{
    struct StageValue
    {
        public Expression Value;
        public MetaStage MinStage;
        public MetaStage MaxStage;

        public StageValue(Expression value, MetaStage minStage, MetaStage maxStage = MetaStage.Max)
        {
            Value = value;
            MinStage = minStage;
            MaxStage = maxStage;
        }

        public static StageValue Default => new StageValue(null, MetaStage.Const);
    }
}
