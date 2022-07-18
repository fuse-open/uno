
namespace Uno.UX
{
	public class UXExpressionAttribute: Attribute
	{

	}

	public class UXUnaryOperatorAttribute: Attribute
	{
		public UXUnaryOperatorAttribute(string symbol) {}
	}

	public class UXBinaryOperatorAttribute: Attribute
	{
		public UXBinaryOperatorAttribute(string symbol, int precedence) {}
	}
}

