using System.Collections.Generic;
using System.IO;
using Uno.IO;

namespace Uno.Compiler.Foreign.Java
{
    internal class UnoCallToForeignMethod
	{
		public List<string> SetUp;
		public string Recieve;
		public string Convert;
		public List<string> TearDown;
		public string Return;

		public UnoCallToForeignMethod(ForeignMethod fm, Converters.Converter convert, string javaMethodName)
		{
			var jniSignature = fm.JniSignature;

			// gen code for getting class, methods and potentially casting vars
			var tearDown = new List<string>();
			var jniVarStrings = new List<string> { "__cls", "__mtd" };
			var safeVarPrefix = fm.GetSafeVarPrefixForCpp();
			var setup = new List<string>
				{
					"INIT_JNI;",
					"jclass __cls = JniHelper::GetNativeExternClass();",
					"WITH_STATIC_JAVA_METHOD(__mtd, __cls, \"" + javaMethodName + "\", \"" + jniSignature + "\");"
				};

			if (!fm.IsStatic)
			{
				var thisCast = convert.Type.CastUnoToJni(convert.Essentials.Object, Converters.Converter.JniFreeingTechnique.Default, safeVarPrefix + "this_", safeVarPrefix + "uthis");
				setup.Add(thisCast.UnoTmpVarLet("@IL$$"));
				setup.Add(thisCast.CastLet);
				jniVarStrings.Add(thisCast.JniVarName);
				tearDown.Add(thisCast.Free);
			}

			for (var i = 0; i < fm.InitialUnoMethod.Parameters.Length; i++)
			{
				var p = fm.InitialUnoMethod.Parameters[i];
				var jvar = convert.Type.CastUnoToJni(p.Type, Converters.Converter.JniFreeingTechnique.Default, safeVarPrefix + p.Name, safeVarPrefix + "u" + p.Name);
				setup.Add(jvar.UnoTmpVarLet("@IL$" + i));
				setup.Add(jvar.CastLet);
				jniVarStrings.Add(jvar.JniVarName);
				tearDown.Add(jvar.Free);
			}

			if (fm.UnoReturnType == convert.Essentials.String)
				tearDown.Add("if (__jresult!=nullptr && U_JNIVAR->GetObjectRefType(__jresult) == JNILocalRefType) U_JNIVAR->DeleteLocalRef(__jresult);");

			//--------------------------

			var call = "U_JNIVAR->" + GetJniCallFunction(fm) + "(" + string.Join(",", jniVarStrings) + ")";

			SetUp = setup;

			Recieve = convert.Type.IsVoid(fm.UnoReturnType)
							 ? call + ";" 
			                 : convert.Type.UnoToJniType(fm.UnoReturnType) + " __jresult = " + call + ";";


			Convert = fm.UnoReturnType.IsVoid
						? "" : "@{" + fm.UnoReturnType.FullName + "} __result = " + convert.Type.CastJniToUno(fm.UnoReturnType, "__jresult", false) + ";"; 

			TearDown = tearDown;

			Return = convert.Type.IsVoid(fm.UnoReturnType) ? "" : "return __result;";
		}

		public string ToSource()
		{
			using (var tw = new StringWriter())
			{
				using (var ftw = new TextFormatter(tw))
				{
					ftw.Indent("{");
					ftw.WriteLines(string.Join("\n", SetUp));
					ftw.WriteLine(Recieve);
					ftw.WriteLines(Convert);
					foreach (var line in TearDown)
						ftw.WriteLine(line);
					ftw.WriteLine("@{global::Android.Base.JNI.CheckException():Call()};");
					ftw.WriteLines(Return);
					ftw.Unindent("}");
					return tw.ToString();
				}
			}
		}

	    static string GetJniCallFunction(ForeignMethod fm)
		{
			var returnTypeName = fm.UnoReturnType.IsEnum
				? fm.UnoReturnType.Base.FullName
				: fm.UnoReturnType.FullName;

			string result;
			if (!_jniCallFunc.TryGetValue(returnTypeName, out result))
				result = "Object";
			return "CallStatic" + result + "Method";
		}

		static readonly Dictionary<string, string> _jniCallFunc = new Dictionary<string, string>
		{
			{ "void", "Void" },
			{ "float", "Float" },
			{ "double", "Double" },
			{ "long", "Long" },
			{ "short", "Short" },
			{ "char", "Char" },
			{ "int", "Int" },
			{ "sbyte", "Byte" },
			{ "bool", "Boolean" }
		};
	}
}