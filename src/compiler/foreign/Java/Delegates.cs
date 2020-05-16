using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.IO;

namespace Uno.Compiler.Foreign.Java
{
	// This file focuses on handling calls from java to uno
	// these occur when the user has added macros inside the
	// uno foreign method.

    internal partial class ForeignJavaPass : ForeignPass
	{
		public readonly Dictionary<DelegateType, string> DelegatesSeen = new Dictionary<DelegateType, string>();

		internal void GenDelegatePlumbing(Function f)
		{
			foreach (var p in f.Parameters)
				if (p.Type.IsSubclassOfOrEqual(Essentials.Delegate))
					GenDelegatePlumbing((DelegateType)p.Type);
			if (f.ReturnType.IsSubclassOfOrEqual(Essentials.Delegate))
				GenDelegatePlumbing((DelegateType)f.ReturnType);
		}

		internal void GenDelegatePlumbing(DelegateType d)
		{
			if (!DelegatesSeen.ContainsKey(d))
			{
				DelegateSanityCheck(d);

				var converted = new ForeignMacroExpr(d, Convert, Helpers);
				var entrypointName = Convert.Name.GenNativeMethodName(d);

				// Generate c++ entrypoint
				BlockHost.AddCppEntrypoint(converted, entrypointName);
				BlockHost.Include(d);

				// Generate java native method
				BlockHost.NativeJavaMethods.Add(converted.GenJavaNativeMethod(entrypointName));

				// Generate java runnable/callable class
				DelegatesSeen.Add(d, GenJavaDelegateCode(d, converted, entrypointName));
			}
		}

		void DelegateSanityCheck(DelegateType d)
		{
			// v1 of the delegate passing feature just cant support everything
			// so lets set some limits here.
			if (d.Parameters.Any(x => x.Type.IsSubclassOfOrEqual(Essentials.Delegate)))
				throw new Exception("Foreign Code Limitation: The compiler cannot currently convert delegates with delegate arguments to a Java type.");
			if (d.Parameters.Any(x => x.Type.IsGenericType))
				throw new Exception("Foreign Code Limitation: The compiler cannot currently convert delegates with generics arguments to a Java type.");
		}

		string GenJavaDelegateCode(DelegateType d, ForeignMacroExpr dEntity, string entrypointName)
		{
			using (var tw = new StringWriter())
			{
				using (var ftw = new TextFormatter(tw))
				{
					var voidReturn = d.ReturnType.IsVoid;
					var name = Convert.Name.JavaDelegateName(d);
					var fullName = Convert.Name.JavaDelegateName(d, true);
					var package = fullName.Substring(0, fullName.LastIndexOf('.'));
					var implements = (voidReturn && d.Parameters.Length == 0) ? " implements java.lang.Runnable" : "";
					ftw.WriteLine("package " + package + ";");
					ftw.WriteLine("import com.foreign.*;");
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
					ftw.WriteLine("import com.uno.UnoObject;");
					ftw.WriteLine("");
					ftw.WriteLine("public class " + name + " extends com.uno.UnoObject" + implements);
					ftw.Indent("{");
					ftw.WriteLine("public " + name + "(long ptr) { super(ptr); }");

					ftw.Indent("public " + Convert.Type.UnoToJavaType(d.ReturnType, true) + " run(" + string.Join(", ", d.Parameters.Select(Convert.Param.UnoToJavaParameter)) + ") {");

					var args = d.Parameters.Select(x => x.Name).ToList();
					args.Insert(0, "this");

					ftw.WriteLine((voidReturn ? "" : "return ") + dEntity.GenCallToNativeMethod(entrypointName, args) + ";");

					ftw.Unindent("}");

					ftw.Unindent("}");
					var finalStr = tw.ToString();
					return finalStr;
				}
			}
		}
	}
}
