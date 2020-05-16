using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Foreign.Java
{
	internal class ForeignMacroExpr
	{
		public string CppSignatureArgs;
		public string JniSignature;
		public string CallToUno;
		public DataType ReturnType;
		public List<MacroParam> Params;
		public HashSet<DataType> EntrypointIncludes = new HashSet<DataType>();
	    readonly Converters.Converter _convert;

		// We use this to get the data we need from the uno function in a form that is easy to use
		// in the generated c++ and java
		public ForeignMacroExpr(Function f, Converters.Converter convert, ForeignHelpers helpers)
		{
			_convert = convert;
			var source = f.Parameters.ToList();
			if (!f.IsStatic)
				source.Insert(0, new Parameter(f.Source, AttributeList.Empty, 0, f.DeclaringType, MacroParam.CppThisArgName, null));
			var uparams = source.Select(p => new MacroParam(p, convert, helpers)).ToList();
			EntrypointIncludes.AddRange(CalcImportsForExternBlock(f));
			EntrypointIncludes.AddRange(uparams.SelectMany(x => x.EntrypointIncludes));

			CppSignatureArgs = convert.Signature.GenCppSignature(uparams);
			Params = uparams;
			JniSignature = convert.Signature.GenJniSignature(uparams, f.ReturnType);
			ReturnType = f.ReturnType;
			CallToUno = GenCppToUnoCall(uparams, f.IsStatic, false, f.UnoName, f.DeclaringType, f.Prototype.IsConstructor);
		}

		public ForeignMacroExpr(DelegateType d, Converters.Converter convert, ForeignHelpers helpers)
		{
			_convert = convert;
			var source = d.Parameters.ToList();
			source.Insert(0, new Parameter(d.Source, AttributeList.Empty, 0, d, MacroParam.DelegateArgName, null));
			var uparams = source.Select(p => new MacroParam(p, convert, helpers)).ToList();
			EntrypointIncludes.AddRange(CalcImportsForExternBlock(d));
			EntrypointIncludes.AddRange(uparams.SelectMany(x => x.EntrypointIncludes));

			CppSignatureArgs = convert.Signature.GenCppSignature(uparams);
			Params = uparams;
			JniSignature = convert.Signature.GenJniSignature(uparams, d.ReturnType);
			ReturnType = d.ReturnType;
			CallToUno = GenCppToUnoCall(uparams, false, true, d.Name, d, false);
		}

		public ForeignMacroExpr(Member f, bool isGetter, Converters.Converter convert, ForeignHelpers helpers)
		{
			_convert = convert;
			var source = new List<Parameter>();
			if (!isGetter)
				source.Insert(0, new Parameter(f.Source, AttributeList.Empty, 0, f.ReturnType, MacroParam.CppSetArgName, null));
			if (!f.IsStatic)
				source.Insert(0, new Parameter(f.Source, AttributeList.Empty, 0, f.DeclaringType, MacroParam.CppThisArgName, null));
			var uparams = source.Select(p => new MacroParam(p, convert, helpers)).ToList();
			EntrypointIncludes.AddRange(uparams.SelectMany(x => x.EntrypointIncludes));
			var returnType = isGetter ? f.ReturnType : convert.Type.Void;

			CppSignatureArgs = convert.Signature.GenCppSignature(uparams);
			Params = uparams;
			JniSignature = convert.Signature.GenJniSignature(uparams, returnType);
			ReturnType = returnType;
			CallToUno = GenCppToUnoGetSet(uparams, f.IsStatic, f.UnoName, f.DeclaringType, isGetter);
		}

		// generate the uxl macro which will call the uno function
		string GenCppToUnoCall(List<MacroParam> uparams, bool isStatic, bool isDelegate, string name, DataType declaringType, bool isConstructor)
		{
			var callArgs = uparams.Where(x => !x.IsThis).Select(x => x.CallForm);
			var callTypes = uparams.Where(x => !x.IsThis).Select(x => x.Type.FullName);

			var of = isStatic ? "" : ":Of(" + uparams[0].UnoArgName + ")";

			if (isConstructor)
			{
				var preCall = declaringType.FullName + "(" + string.Join(",", callTypes) + ")";

				return "@{" + preCall + ":New(" + string.Join(",", callArgs) + ")}";
			}
			else
			{
				var preCall = declaringType.FullName + (isDelegate ? of : of + "." + name + "(" + string.Join(",", callTypes) + ")");

				return "@{" + preCall + ":Call(" + string.Join(",", callArgs) + ")}";
			}
		}

		// generate the uxl macro which will call the uno function
		string GenCppToUnoGetSet(List<MacroParam> uparams, bool isStatic, string name, DataType declaringType, bool IsGetter)
		{
			var callArgs = uparams.Where(x => !x.IsThis).Select(x => x.CallForm);

			if (isStatic)
			{
				if (IsGetter)
					return "@{" + declaringType.FullName + "." + name + "}";
				else
					return "@{" + declaringType.FullName + "." + name + ":Set(" + string.Join(",", callArgs) + ")}";
			}
			else
			{
				if (IsGetter)
					return "@{" + declaringType.FullName + ":Of(" + uparams[0].UnoArgName + ")." + name + ":Get()}";
				else
					return "@{" + declaringType.FullName + ":Of(" + uparams[0].UnoArgName + ")." + name + ":Set(" + string.Join(",", callArgs) + ")}";
			}
		}


		// To call c++ java requires a native method. c++ will register this to a c++ extern'd function
		public string GenJavaNativeMethod(string name)
		{
			var args = Params.Select(x => x.JavaTypedName).ToList();
			args.AddRange(Params.Where(x => x.HasPointerArg).Select(x => "long " + x.CppArgName));
			return "public static native " + _convert.Type.UnoToJavaType(ReturnType, false) + " " + name + "(" + string.Join(",", args) + ");";
		}


		// Gen call from the Java method where the macro was found to the newly generate java native method
		public string GenCallToNativeMethod(string entrypointName, List<string> javaArgs)
		{
			for (int i = 0; i < Params.Count; i++)
				if (Params[i].HasPointerArg)
					javaArgs.Add("UnoHelper.GetUnoObjectRef(" + javaArgs[i] + ")");

			return "ExternedBlockHost." + entrypointName + "(" + string.Join(",", javaArgs) + ")";
		}

		IEnumerable<DataType> CalcImportsForExternBlock(ParametersMember func)
		{
		    var ret = new List<DataType> {func.DeclaringType};
		    ret.AddRange(CalcImportsForExternBlock(func.ReturnType));
			foreach (var arg in func.Parameters)
				if (arg.Type.IsGenericParameterization)
					foreach (var genericParam in arg.Type.GenericArguments)
						ret.AddRange(CalcImportsForExternBlock(genericParam));
			return ret;
		}

		IEnumerable<DataType> CalcImportsForExternBlock(DataType dt)
		{
			if (dt.IsVoid) return new List<DataType>();
			var ret = new List<DataType> { dt };
			if (dt.IsGenericParameterization)
			{
				foreach (var genericParam in dt.GenericArguments)
				{
					ret.Add(genericParam);
					ret.AddRange(CalcImportsForExternBlock(genericParam));
				}
			}
			return ret;
		}
	}
}
