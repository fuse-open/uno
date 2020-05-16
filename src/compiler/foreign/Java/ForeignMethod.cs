using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Foreign.Java
{
    internal class ForeignMethod
	{
		readonly IEssentials _essentials;
		public readonly Function InitialUnoMethod;
		public readonly string JavaName;
		public readonly List<string> CppHeadersForDeclaringType;
		public readonly List<string> Annotations;
		public readonly Converters.Converter Convert;
		public readonly ForeignHelpers Helpers;

		public string JniSignature => Convert.Signature.GetJniSignature(InitialUnoMethod);
		public bool IsStatic => InitialUnoMethod.IsStatic;
		public DataType UnoReturnType => InitialUnoMethod.ReturnType;

		public ForeignMethod(Function f, IEssentials essentials, Converters.Converter convert, ForeignHelpers helpers, List<string> annotations)
		{
			_essentials = essentials;
			Convert = convert;
			Helpers = helpers;
			CppHeadersForDeclaringType = new List<string> {
					"jni.h",
					"Uno/JNIHelper.h",
					"@{global::Android.Base.Wrappers.JWrapper:Include}",
					"@{global::Android.Base.Wrappers.JavaObjectHelper:Include}",
					"@{" + ForeignJavaPass.UnoToJavaBoxingClass.FullName + ":Include}"
			};
			Annotations = annotations;
			ValidateMethod(f, essentials);
			InitialUnoMethod = f;
			JavaName = convert.Name.JavaMethodName(f);
		}

		void ValidateMethod(ParametersMember f, IEssentials essentials)
		{
			if (f.Parameters.Any(x => x.Type.IsGenericType))
				throw new Exception("Foreign Code Limitation: The compiler cannot currently convert generics arguments to a Java types.\nFound in: " + f.FullName);
			if (f.ReturnType.IsGenericType)
				throw new Exception("Foreign Code Limitation: The compiler cannot currently handle generics return types.\nFound in: " + f.FullName);

			foreach (var p in f.Parameters)
			{
				if ((p.Type.IsEnum ? p.Type.Base : p.Type) == _essentials.Byte)
					throw new Exception("Foreign Code Limitation: Method '" + f.FullName + "' has an argument '" + p.UnoName + "' of type byte. Java does not support byte. It does however support sbyte.");
			}

			if ((f.ReturnType.IsEnum ? f.ReturnType.Base : f.ReturnType) == _essentials.Byte)
				throw new Exception("Foreign Code Limitation: Method '" + f.FullName + "' has a return type of byte. Java does not support byte. It does however support sbyte.");
		}

				
		public string GetCallFromUno()
		{
			return new UnoCallToForeignMethod(this, Convert, JavaName).ToSource();
		}

	    // This is another cludge method. I need names to use in the c++ that don't clash
	    // with the actual arg names. AFAIK we don't have an API I can use to request safe
	    // names so I settled with sticking enough underscores at the front to not collide.
	    // Of course, this being a cludge, it doesnt look at field names, etc.
		internal string GetSafeVarPrefixForCpp()
		{
			var wip = "_";
			while (InitialUnoMethod.Parameters.Any(x => x.Name.StartsWith(wip)))
			{
				wip += "_";
			}
			return wip;
		}
	}
}

