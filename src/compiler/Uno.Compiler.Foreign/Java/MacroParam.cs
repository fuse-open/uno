using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Foreign.Java
{
    internal class MacroParam
	{
		internal static readonly string CppThisArgName = "jthis";
		internal static readonly string DelegateArgName = "theDelegate";
		internal static readonly string CppSetArgName = "setVal";
		static readonly string _javaArgPointerPostfix = "Ptr";
		public DataType Type;
		public string JniSigType;
		public string CppArgName;
		public string CppTypedName;
		public string JavaTypedName;
		public string UnoArgName;
		public string JniToUnoCoversion;
		public string CallForm;
		public string JavaArgCast;
		public bool HasPointerArg;
		public bool IsThis;
		public HashSet<DataType> EntrypointIncludes = new HashSet<DataType>();

		public MacroParam(Parameter p, Converters.Converter convert, ForeignHelpers helpers)
		{
			var unoArgName = "uno_" + p.Name;
			var isThis = (p.Name == CppThisArgName || p.Name == DelegateArgName);
			var callForm = helpers.IsPrimitive(p.Type) || p.Type.IsEnum || p.Type.IsStruct ? unoArgName : helpers.CastTo(p.Type, unoArgName);

			UnoArgName = unoArgName;
			Type = p.Type;
			JniSigType = convert.Type.UnoToJniSigType(p.Type);
			CppArgName = p.Name + _javaArgPointerPostfix;
			CppTypedName = UnoToJniParameter(p, convert);
			JavaArgCast = "(" + convert.Type.UnoToJavaType(p.Type, false) + ")";
			JavaTypedName = convert.Param.UnoToJavaParameter(p);
			JniToUnoCoversion = GenToUnoConversion(p, unoArgName, convert, helpers);
			if (!helpers.IsPrimitive(p) && p.Type.IsStruct)
				Console.Write("");
			CallForm = callForm;
			HasPointerArg = convert.Type.IsJavaObject(p.Type);
			IsThis = isThis;
		}

	    static string UnoToJniParameter(Parameter p, Converters.Converter convert)
		{
			return convert.Type.UnoToJniType(p.Type) + " " + p.Name;
		}

		// convert the jni type to an uno type
		string GenToUnoConversion(Parameter arg, string unoArgName, Converters.Converter convert, ForeignHelpers helpers)
		{
			EntrypointIncludes.Add(arg.Type);
			return "@{" + arg.Type.FullName + "} " + unoArgName + " = " + convert.Type.CastJniToUno(arg.Type, arg.Name, true) + ";";
		}
	}
}
