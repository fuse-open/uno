using System;
using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.IO;

namespace Uno.Compiler.Foreign.Java
{
    internal partial class ForeignJavaPass : ForeignPass
	{
		class JavaClass
		{
		    public readonly string FullName;
		    public readonly Dictionary<DataType, JavaClass> StaticNestedClasses = new Dictionary<DataType, JavaClass>();
		    readonly HashSet<string> _usings = new HashSet<string>();
			readonly List<JavaMethod> _methods = new List<JavaMethod>();
			readonly string _name;
			readonly string _package;
			readonly bool _nested;

			readonly IEnvironment _environment;
		    readonly ForeignHelpers _helpers;
		    readonly Converters.Converter _convert;
		    readonly Entrypoints _blockHost;

			public JavaClass(DataType dt, ForeignHelpers helpers, Converters.Converter convert, Entrypoints blockHost, IEnvironment env)
			{
				_environment = env;
				_blockHost = blockHost;
				_helpers = helpers;
				_convert = convert;

				FullName = convert.Name.ComForeignClassName(dt);
				var split = FullName.LastIndexOf(".", StringComparison.Ordinal);
				_name = FullName.Substring(split + 1);
				_package = FullName.Substring(0, split);
				_usings.AddRange(helpers.GetForeignIncludes(dt, "Java", env));
				_nested = dt.IsNestedType;
			}

			public void AddForeignMethod(ForeignMethod fm, ExpandInterceptor expandInterceptor)
			{				
				var jmtd = new JavaMethod(fm, _helpers, _convert, expandInterceptor, _environment);
				_methods.Add(jmtd);
				_blockHost.NativeJavaMethods.Add(jmtd.GenJavaEntrypointMethod(this));
			}

			HashSet<string> AggregateImports()
			{
				var result = new HashSet<string>();
				result.AddRange(_usings);
				foreach (var nested in StaticNestedClasses.Values)
				{
					result.AddRange(nested.AggregateImports());
				}
				return result;
			}

			string GenJavaClassCode()
			{
				using (var tw = new StringWriter())
				{
					using (var ftw = new TextFormatter(tw))
					{
						if (!_nested)
						{
							ftw.WriteLine("package " + _package + ";\n");
							ftw.WriteLine("// fuse defined imports");
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
							ftw.WriteLine("import com.foreign.UnoHelper;");
							ftw.WriteLine("import com.foreign.UnoWrapped;");
							ftw.WriteLine("import com.foreign.ExternedBlockHost;");

							var userDefinedImports = AggregateImports();
							if (userDefinedImports.Count > 0)
							{
								ftw.WriteLine("\n// user defined imports");
								foreach (var u in userDefinedImports)
								{
									ftw.WriteLine("import " + u + ";");
								}
							}
						}
						ftw.WriteLine("\npublic" + (_nested ? " static" : "") + " class " + _name);
						ftw.Indent("{");

						if (!_nested)
						{
							ftw.WriteLine("@Deprecated");
							ftw.WriteLine("static void debug_log(Object message)");
							ftw.Indent("{");
							ftw.WriteLine("android.util.Log.d(\"" + _environment.GetString("Activity.Name") + "\", (message==null ? \"null\" : message.toString()));");
							ftw.Unindent("}\n");
						}

						foreach (var method in _methods)
						{
							ftw.WriteLines(method.Source);
						}

						foreach (var staticNestedClass in StaticNestedClasses.Values)
						{
							ftw.WriteLines(staticNestedClass.GenJavaClassCode());
						}

						ftw.Unindent("}");
						return tw.ToString();
					}
				}
			}

			public void WriteJavaClass(Disk disk)
			{
				if (_nested)
					throw new Exception("ForeignCode: Attempted to write dedicated javae file for nested class.");
				
				var filePath = FullName.Replace(".", "/") + ".java";
				var path = _environment.Combine(_environment.GetString("Java.SourceDirectory"), filePath);
				disk.WriteAllText(path, GenJavaClassCode());
			}
		}
	}
}

