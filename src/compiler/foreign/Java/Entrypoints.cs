using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.IO;

namespace Uno.Compiler.Foreign.Java
{
    internal class Entrypoints
	{
		public readonly HashSet<string> NativeJavaMethods = new HashSet<string>();
		readonly IEnvironment _environment;
		readonly Disk _disk;
		readonly Converters.Converter _convert;

		readonly List<Member> _externBlockRequiredMembers = new List<Member>();

		public Entrypoints(IEnvironment env, Disk disk, IILFactory ilFactory, Converters.Converter convert)
		{
			_environment = env;
			_disk = disk;
			_convert = convert;

			UnoType = ilFactory.GetType("Uno.Compiler.ExportTargetInterop.Foreign.Android.ExternBlockHost");
		}

		public void RequireMember(Member m)
		{
			_externBlockRequiredMembers.Add(m);
		}

		readonly List<string> _cppEntrypoints = new List<string>();

		public void AddCppEntrypoint(ForeignMacroExpr ufunc, string name)
		{
			_cppEntrypoints.Add(GenCppNativeMethod(ufunc, name));
		}

		// Gen the c++ side of the native call. This is an extern'd (JNICALL) function which translates
		// the jni types into uno types and then calls the uno method named in the macro
		string GenCppNativeMethod(ForeignMacroExpr ufunc, string name)
		{
			using (var tw = new StringWriter())
			{
				using (var ftw = new TextFormatter(tw))
				{
					ftw.WriteLine(_convert.Type.UnoToJniType(ufunc.ReturnType) + " JNICALL " + name + "(" + ufunc.CppSignatureArgs + ")");
					ftw.Indent("{");
					ftw.WriteLine("uAutoReleasePool pool;");
					ftw.WriteLine("INIT_JNI;");

					// NEED TO UNIFY uParam & TYPE CONVERTER
					foreach (var arg in ufunc.Params)
						ftw.WriteLines(arg.JniToUnoCoversion);

					var call = ufunc.CallToUno;



					if (ufunc.ReturnType.FullName != "void")
					{
						// ADD CASTUNOTOJNIFORRETURN TAKE THE FM AND GENERATE THE ARGS NAMES IN TYPE CONVERTER
						var rcast = _convert.Type.CastUnoToJni(ufunc.ReturnType, Converters.Converter.JniFreeingTechnique.WithScope, "__jresult", "__result");
						ftw.WriteLine(_convert.Type.UnoToJniType(ufunc.ReturnType) + " " + rcast.JniVarName+";");
						ftw.Indent("JTRY");
						ftw.WriteLine(rcast.UnoTmpVarLet(call));
						ftw.WriteLine(rcast.CastSet);
						ftw.Unindent("JCATCH");
						ftw.WriteLine("return " + rcast.JniVarName + ";");
					}
					else
					{
						ftw.Indent("JTRY");
						ftw.WriteLine(call + ";");
						ftw.Unindent("JCATCH");
					}
					ftw.Unindent("}");

					NativeRegistrations.Add("{(char* const)\"" + name + "\", (char* const)\"" + ufunc.JniSignature + "\", (void *)&" + name + "},");

					return tw.ToString();
				}
			}
		}

		public readonly HashSet<string> NativeRegistrations = new HashSet<string>();
		public readonly DataType UnoType;


		readonly HashSet<string> _externBlockIncludes = new HashSet<string>() {
			"jni.h",
			"@{Uno.Exception:Include}",
			"@{global::Android.Base.JNI:Include}",
			"@{global::Android.Base.Wrappers.JWrapper:Include}",
			"@{global::Android.Base.Wrappers.JavaObjectHelper:Include}"
		};

		public void Include(DataType dt)
		{
			if (dt.IsVoid) return;
			if (dt.IsArray)
				Include(dt.ElementType);
			else
				_externBlockIncludes.Add("@{" + dt.FullName + ":Include}");
		}

		public void Include(IEnumerable<DataType> types)
		{
			foreach (var dt in types)
				Include(dt);
		}

		public void WriteJava()
		{
			var path = _environment.Combine(_environment.GetString("Java.SourceDirectory"), "com", "foreign");
			var filePath = Path.Combine(path, "ExternedBlockHost.java");
			_disk.WriteAllText(filePath, GenJavaClassCode());
		}

		// All the c++ entrypoints are put in one uno class, the ExternBlockHost. This also is responsible for
		// registering the same entrypoints with the java native methods
		public void WriteCpp(ForeignHelpers helpers)
		{			
			helpers.SourceInclude(_externBlockIncludes, UnoType);
			helpers.SourceDeclaration(_cppEntrypoints, UnoType);
			helpers.Require(_externBlockRequiredMembers, UnoType);
			MakeJniRegisterBlock();
		}

		// Helper method for WriteExternBlockData
		void MakeJniRegisterBlock()
		{
			var lines = new List<string> { "static JNINativeMethod native_activity_funcs[] = {" };
			lines.AddRange(NativeRegistrations);
			lines.Add("};");
			lines.Add("int funcCount = " + NativeRegistrations.Count + ";");
			lines.Add("if ((int)jni->RegisterNatives(JniHelper::GetNativeExternClass(), native_activity_funcs, funcCount)<0) {");
			lines.Add("U_FATAL(\"COULD NOT REGISTER NATIVE EXTERN FUNCTIONS\");");
			lines.Add("}");
			var finalCode = string.Join("\n", lines);

			_environment.Require(UnoType, "Java.Extern.RegisterFunction", UnoType.Source, finalCode);
		}

		string GenJavaClassCode()
		{
			using (var tw = new StringWriter())
			{
				using (var ftw = new TextFormatter(tw))
				{
					ftw.WriteLine("package com.foreign;\n");
					ftw.WriteLine("import com.uno.UnoObject;");
					ftw.WriteLine("import com.uno.BoolArray;");
					ftw.WriteLine("import com.uno.ByteArray;");
					ftw.WriteLine("import com.uno.CharArray;");
					ftw.WriteLine("import com.uno.DoubleArray;");
					ftw.WriteLine("import com.uno.FloatArray;");
					ftw.WriteLine("import com.uno.IntArray;");
					ftw.WriteLine("import com.uno.LongArray;");
					ftw.WriteLine("import com.uno.ObjectArray;");
					ftw.WriteLine("import com.uno.ShortArray;");
					ftw.WriteLine("import com.uno.StringArray;");
					ftw.WriteLine("public class ExternedBlockHost");
					ftw.Indent("{");

					ftw.WriteLine("@Deprecated");
					ftw.WriteLine("static void debug_log(Object message)");
					ftw.Indent("{");
					ftw.WriteLine("android.util.Log.d(\"" + _environment.GetString("Activity.Name") + "\", (message==null ? \"null\" : message.toString()));");
					ftw.Unindent("}");

					foreach (var method in NativeJavaMethods)
					{
						ftw.WriteLines(method);
					}
					ftw.Unindent("}");
					return tw.ToString();
				}
			}
		}
	}
}

