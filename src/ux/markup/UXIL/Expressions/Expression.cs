using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Uno.UX.Markup.Tests")]

namespace Uno.UX.Markup.UXIL.Expressions
{
    abstract class Expression
    {
        /// <summary>
        /// Returns true if this expression is a "classic" UX trivial expression that 
        /// can be parsed by the legacy parser. Such expressions do not generate a binding.
        /// </summary>
        public virtual bool IsTrivial { get { return false; } }

        /// <summary>
        /// Checks for value equality of two expressions. This is primarily used for testing,
        /// and kept separate from the normal Equals implementation, as several parts of the
        /// compiler currently assume Expressions have reference equality semantics.
        /// </summary>
        public abstract bool ValueEquals(Expression other);
    }

    class ExpressionValueEqualityComparer : IEqualityComparer<Expression>
    {
        public bool Equals(Expression x, Expression y)
        {
            return x?.ValueEquals(y) ?? false;
        }

        public int GetHashCode(Expression obj)
        {
            return obj.GetHashCode();
        }
    }

    class ThisExpression : Expression
    {
        public override string ToString()
        {
            return "this";
        }

        public override bool IsTrivial
        {
            get
            {
                return true;
            }
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as ThisExpression);
        }

        public bool ValueEquals(ThisExpression other)
        {
            return !ReferenceEquals(null, other);
        }
    }

    class Binding : Expression
    {
        public readonly Expression Key;
        public Binding(Expression key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return "{" + Key + "}";
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as Binding);
        }

        public bool ValueEquals(Binding other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key.ValueEquals(other.Key);
        }
    }

    [Flags]
    enum Modifier
    {
        Clear = 1,
        Read = 2,
        Write = 4,
        ReadClear = Read | Clear,
        WriteClear = Write | Clear,
        ReadWrite = Read | Write,
        ReadWriteClear = Read | Write | Clear,
        Default = ReadWrite
    }

    class ModeExpression: Expression
    {
        public readonly Expression Expression;
        public readonly Modifier Mode;
        public ModeExpression(Expression exp, Modifier mod)
        {
            Expression = exp;
            Mode = mod;
        }

        public override string ToString()
        {
            return Mode + " " + Expression;
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as ModeExpression);
        }

        public bool ValueEquals(ModeExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Expression.ValueEquals(other.Expression) && Mode == other.Mode;
        }
    }
    
    class RawExpression : Expression
    {
        public readonly Expression Expression;
        public RawExpression(Expression exp)
        {
            Expression = exp;
        }

        public override string ToString()
        {
            return "{= " + Expression + "}";
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as RawExpression);
        }

        public bool ValueEquals(RawExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Expression.ValueEquals(other.Expression);
        }
    }

    class UserDefinedUnaryOperator: Expression
    {
        public readonly string UxUnaryOperatorName;
        public readonly Expression Argument;
        public UserDefinedUnaryOperator(string uxUnaryOperatorName, Expression argument)
        {
            UxUnaryOperatorName = uxUnaryOperatorName;
            Argument = argument;
        }

        public override string ToString()
        {
            return UxUnaryOperatorName + " " + Argument;
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as UserDefinedUnaryOperator);
        }

        public bool ValueEquals(UserDefinedUnaryOperator other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UxUnaryOperatorName, other.UxUnaryOperatorName) && Argument.ValueEquals(other.Argument);
        }
    }

    class Literal : Expression
    {
        public readonly string Value;
        public Literal(string value)
        {
            Value = value;
        }
        public override string ToString()
        {
            return Value;
        }
        public override bool IsTrivial
        {
            get
            {
                return true;
            }
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as Literal);
        }

        public bool ValueEquals(Literal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }
    }

    class StringLiteral: Literal
    {
        public StringLiteral(string value) : base(value)
        {
        }

        public override string ToString()
        {
            return "\"" + base.ToString() + "\"";
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as StringLiteral);
        }
    }


    class Identifier : Expression
    {
        public readonly string Name;
        public Identifier(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool IsTrivial
        {
            get
            {
                return true;
            }
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as Identifier);
        }

        public bool ValueEquals(Identifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }
    }

    class MemberExpression : Expression
    {
        public readonly Expression Object;
        public readonly string Member;
        public MemberExpression(Expression obj, string member)
        {
            Object = obj;
            Member = member;
        }

        public override string ToString()
        {
            return Object.ToString() + "." + Member;
        }

        public override bool IsTrivial
        {
            get
            {
                return Object.IsTrivial;
            }
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as MemberExpression);
        }

        public bool ValueEquals(MemberExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return false;
            return Object.ValueEquals(other.Object) && string.Equals(Member, other.Member);
        }
    }

    class LookUpExpression : Expression
    {
        public readonly Expression Collection;
        public readonly Expression Index;
        public LookUpExpression(Expression obj, Expression index)
        {
            Collection = obj;
            Index = index;
        }

        public override string ToString()
        {
            return Collection.ToString() + "[" + Index.ToString() + "]";
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as LookUpExpression);
        }

        public bool ValueEquals(LookUpExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Collection.ValueEquals(other.Collection) && Index.ValueEquals(other.Index);
        }
    }

    class ConditionalExpression : Expression
    {
        public readonly Expression Condition, TrueCase, FalseCase;
        public ConditionalExpression(Expression cond, Expression trueCase, Expression falseCase)
        {
            Condition = cond;
            TrueCase = trueCase;
            FalseCase = falseCase;
        }

        public override string ToString()
        {
            return "(" + Condition.ToString() + " ? " + TrueCase.ToString() + " : " + FalseCase + ")";
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as ConditionalExpression);
        }

        public bool ValueEquals(ConditionalExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Condition.ValueEquals(other.Condition) && TrueCase.ValueEquals(other.TrueCase) && FalseCase.ValueEquals(other.FalseCase);
        }
    }

    abstract class BinaryExpression: Expression
    {
        public readonly Expression Left, Right;
        protected BinaryExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        protected abstract string Operator { get; }

        public override string ToString()
        {
            return "(" + Left.ToString() + Operator + Right.ToString() + ")";
        }

        public string Name
        {
            get { return GetType().Name.Substring(0, GetType().Name.Length - "Expression".Length); }
        }
    }

    class AddExpression : BinaryExpression
    {
        protected override string Operator { get { return "+"; } }
        public AddExpression(Expression left, Expression right) : base(left, right) { }

        // TODO: Add tests
        public static Expression Create(Expression[] parts, int from = 0)
        {
            if (parts.Length == from) return null;
            else if (parts.Length == from + 1) return parts[from];
            else return new AddExpression(parts[from], Create(parts, from + 1));
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as AddExpression);
        }

        public bool ValueEquals(AddExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class SubtractExpression : BinaryExpression
    {
        protected override string Operator { get { return "-"; } }
        public SubtractExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as SubtractExpression);
        }

        public bool ValueEquals(SubtractExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class MultiplyExpression : BinaryExpression
    {
        protected override string Operator { get { return "*"; } }
        public MultiplyExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as MultiplyExpression);
        }

        public bool ValueEquals(MultiplyExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class DivideExpression : BinaryExpression
    {
        protected override string Operator { get { return "/"; } }
        public DivideExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as DivideExpression);
        }

        public bool ValueEquals(DivideExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class LessThanExpression : BinaryExpression
    {
        protected override string Operator { get { return "<"; } }
        public LessThanExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as LessThanExpression);
        }

        public bool ValueEquals(LessThanExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class LessOrEqualExpression : BinaryExpression
    {
        protected override string Operator { get { return "<="; } }
        public LessOrEqualExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as LessOrEqualExpression);
        }

        public bool ValueEquals(LessOrEqualExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class GreaterThanExpression : BinaryExpression
    {
        protected override string Operator { get { return ">"; } }
        public GreaterThanExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as GreaterThanExpression);
        }

        public bool ValueEquals(GreaterThanExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class GreaterOrEqualExpression : BinaryExpression
    {
        protected override string Operator { get { return ">="; } }
        public GreaterOrEqualExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as GreaterOrEqualExpression);
        }

        public bool ValueEquals(GreaterOrEqualExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class EqualExpression : BinaryExpression
    {
        protected override string Operator { get { return "=="; } }
        public EqualExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as EqualExpression);
        }

        public bool ValueEquals(EqualExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class NotEqualExpression : BinaryExpression
    {
        protected override string Operator { get { return "!="; } }
        public NotEqualExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as NotEqualExpression);
        }

        public bool ValueEquals(NotEqualExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class LogicalOrExpression : BinaryExpression
    {
        protected override string Operator { get { return "||"; } }
        public LogicalOrExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as LogicalOrExpression);
        }

        public bool ValueEquals(LogicalOrExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class LogicalAndExpression : BinaryExpression
    {
        protected override string Operator { get { return "&&"; } }
        public LogicalAndExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as LogicalAndExpression);
        }

        public bool ValueEquals(LogicalAndExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class NullCoalesceExpression : BinaryExpression
    {
        protected override string Operator { get { return "??"; } }
        public NullCoalesceExpression(Expression left, Expression right) : base(left, right) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as NullCoalesceExpression);
        }

        public bool ValueEquals(NullCoalesceExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left.ValueEquals(other.Left) && Right.ValueEquals(other.Right);
        }
    }

    class VectorExpression : Expression
    {
        public Expression[] Comps { get; private set; }
        public VectorExpression(params Expression[] comps) { Comps = comps; }
        public override string ToString()
        {
            return "(" + Serialize() + ")";
        }

        string Serialize()
        {
            return Comps.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y);
        }

        public Expression TryFold()
        {
            if (Comps.All(x => x is Literal && !(x is StringLiteral))) return new Literal(Serialize());
            else return this;
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as VectorExpression);
        }

        public bool ValueEquals(VectorExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Comps.SequenceEqual(other.Comps, new ExpressionValueEqualityComparer());
        }
    }

    abstract class UnaryExpression: Expression
    {
        public readonly Expression Operand;
        protected UnaryExpression(Expression operand)
        {
            Operand = operand;
        }

        protected abstract string Operator { get; }

        public override string ToString()
        {
            return "(" + Operator + Operand.ToString() + ")";
        }

        public string Name
        {
            get { return GetType().Name.Substring(0, GetType().Name.Length - "Expression".Length); }
        }
    }

    class NameValuePairExpression : Expression
    {
        public readonly Expression Name;
        public readonly Expression Value;

        public NameValuePairExpression(Expression name, Expression value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return "(" + Name + ": " + Value.ToString() + ")";
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as NameValuePairExpression);
        }

        public bool ValueEquals(NameValuePairExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name.ValueEquals(other.Name) && Value.ValueEquals(other.Value);
        }
    }

    class LogicalNotExpression: UnaryExpression
    {
        protected override string Operator { get { return "!"; } }
        public LogicalNotExpression(Expression operand) : base(operand) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as LogicalNotExpression);
        }

        public bool ValueEquals(LogicalNotExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Operand.ValueEquals(other.Operand);
        }
    }

    class NegateExpression : UnaryExpression
    {
        protected override string Operator { get { return "-"; } }
        public NegateExpression(Expression operand) : base(operand) { }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as UnaryExpression);
        }

        public bool ValueEquals(UnaryExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Operand.ValueEquals(other.Operand);
        }
    }

    class FunctionCallExpression : Expression
    {
        public readonly string FuncName;
        public readonly Expression[] Args;
        public FunctionCallExpression(string funcName, Expression[] args)
        {
            FuncName = funcName;
            Args = args;
        }
        
        public override string ToString()
        {
			var q = FuncName + "(";
			for (int i=0; i < Args.Length; ++i)
			{
				if (i > 0)
					q += ", ";
				q += Args[i].ToString();
			}
			q += ")";
			return q;
        }

        public override bool ValueEquals(Expression other)
        {
            return ValueEquals(other as FunctionCallExpression);
        }

        public bool ValueEquals(FunctionCallExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FuncName.Equals(other.FuncName) && Args.SequenceEqual(other.Args, new ExpressionValueEqualityComparer());
        }
    }
}
