using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Foreign.Java.Converters
{
    internal partial class Converter
	{		
		public readonly TypeConverter Type;
		public readonly NameConverter Name;
		public readonly ParamConverter Param;
		public readonly SignatureConverter Signature;
		public readonly IEssentials Essentials;

		public Converter(DataType boxedJavaObject, IEssentials essentials, IILFactory ilFactory, ForeignHelpers helpers)
		{
			Essentials = essentials;
			Type = new TypeConverter(this, boxedJavaObject, essentials, helpers);
			Name = new NameConverter(this, ilFactory);
			Param = new ParamConverter(this);
			Signature = new SignatureConverter(this, essentials);
		}
	}
}

