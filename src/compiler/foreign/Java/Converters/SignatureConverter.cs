using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Foreign.Java.Converters
{
    internal partial class Converter
	{
		public class SignatureConverter
		{
		    readonly Converters.Converter _convert;
			readonly IEssentials _essentials;

			public SignatureConverter(Converters.Converter convert, IEssentials essentials)
			{
				_convert = convert;
				_essentials = essentials;
			}

			public string GenCppSignature(List<MacroParam> uparams)
			{
				var sig = new List<string>() { "JNIEnv *env", "jclass clazz" };
				if (uparams.Count > 0)
				{
					sig.AddRange(uparams.Select(x => x.CppTypedName));
					sig.AddRange(uparams.Where(x => x.HasPointerArg).Select(x => "jlong " + x.CppArgName));
				}
				return string.Join(",", sig);
			}

			public string GenJniSignature(List<MacroParam> uparams, DataType returnType)
			{
				var jniArgs = uparams.Select(x => x.JniSigType).ToList();
				jniArgs.AddRange(uparams.Where(x => x.HasPointerArg).Select(x => "J"));
				return "(" + string.Join("", jniArgs) + ")" + _convert.Type.UnoToJniSigType(returnType);
			}

			public string GetJniSignature(Function f)
			{
				var types = f.Parameters.Select(x => _convert.Type.UnoToJniSigType(x.Type)).ToList();
				if (!f.IsStatic)
					types.Insert(0, _convert.Type.UnoToJniSigType(_essentials.Object));
				return "(" + string.Join("", types) + ")" + _convert.Type.UnoToJniSigType(f.ReturnType);
			}

			public string GetJniSignature(DelegateType d)
			{
				var types = d.Parameters.Select(x => _convert.Type.UnoToJniSigType(x.Type)).ToList();
				if (!d.IsStatic)
					types.Insert(0, _convert.Type.UnoToJniSigType(_essentials.Object));
				return "(" + string.Join("", types) + ")" + _convert.Type.UnoToJniSigType(d.ReturnType);
			}

		}
	}
}

