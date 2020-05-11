using System.IO;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.IO;

namespace Uno.Compiler.Foreign.Java
{
    internal partial class ForeignJavaPass : ForeignPass
	{
		class JavaMethod
		{
		    public readonly string Source;
		    readonly ForeignMethod _foreignMethod;
			readonly ForeignHelpers _helpers;
			readonly Converters.Converter _convert;
			readonly IEnvironment _environment;
			readonly string _argsWithTypes;
			readonly string _argsNoTypes;

			public JavaMethod(ForeignMethod fm, ForeignHelpers helpers, Converters.Converter convert, ExpandInterceptor expandInterceptor, IEnvironment env)
			{
				_convert = convert;
				_helpers = helpers;
				_environment = env;
				_foreignMethod = fm;
				_argsWithTypes = JavaMethodArgs(fm.InitialUnoMethod);
				_argsNoTypes = JavaMethodArgs(fm.InitialUnoMethod, false);
				Source = GenJavaMethod(fm.InitialUnoMethod, fm.JavaName, expandInterceptor);
			}

			string GenJavaMethod(Function f, string javaMethodName, ExpandInterceptor expandInterceptor)
			{
				using (var tw = new StringWriter())
				{
					using (var ftw = new TextFormatter(tw))
					{
						// We have a special case here which is ugly
						//var returnType = f.ReturnType
						foreach (var annotation in _foreignMethod.Annotations)
						{
							ftw.WriteLine("@" + annotation);
						}
						ftw.WriteLine("public static " + _convert.Type.UnoToJavaType(f.ReturnType, true) + " " + javaMethodName + "(" + _argsWithTypes + ")");
						ftw.Indent("{");
						ftw.WriteLines(_environment.Expand(f.Source, expandInterceptor, _helpers.GetForeignCodeFromFunction(f), false, f, _helpers.GetExternFunctionScopes(f)));
						ftw.Unindent("}");
						return tw.ToString();
					}
				}
			}

			string JavaMethodArgs(IParametersEntity f, bool types = true)
			{
				if (types)
				{
					var args = string.Join(",", f.Parameters.Select(_convert.Param.UnoToJavaParameter));
					if (!f.IsStatic)
					{
						if (f.Parameters.Length > 0)
							args = "final UnoObject _this, " + args;
						else
							args = "final UnoObject _this";
					}
					return args;
				}
				else
				{
					var args = string.Join(",", f.Parameters.Select(x => x.UnoName));
					if (!f.IsStatic)
					{
						if (f.Parameters.Length > 0)
							args = "_this, " + args;
						else
							args = "_this";
					}
					return args;
				}
			}

			public string GenJavaEntrypointMethod(JavaClass jcls)
			{
				using (var tw = new StringWriter())
				{
					using (var ftw = new TextFormatter(tw))
					{
						var returnType = _foreignMethod.InitialUnoMethod.ReturnType;
						var returnTypeStr = _convert.Type.UnoToJavaType(returnType, false);
						var javaMethodName = _foreignMethod.JavaName;

						ftw.WriteLine("public static " + returnTypeStr + " " + javaMethodName + "(" + _argsWithTypes + ")");
						ftw.Indent("{");
						var callLine = jcls.FullName + "." + javaMethodName + "(" + _argsNoTypes + ");";

						// the reason for the cast here is that in the case of UnoObjects we allow
						// the callLine to return an Object rather than an UnoObject. 
						// We do this for cases like this:
						//
						// [Foreign(Language.Java)]
						// static object Get(Java.Object handle, Java.Object key)
						// @{
						//     @SuppressWarnings("unchecked")
						//     Map<Object, Object> map = (Map<Object, Object>)handle;
						//     return map.get(key);
						// @}
						//
						// Where the return type is clearly uno's object, however a java
						// object has been passed. BUT that java Object is an UnoObject.
						// The user didnt know as, whilst we have always wrapped uno objects,
						// in UnoObject we would then present it to the user as java's
						// Object.
						//
						// By keeping the return type of the foreign function `Object` and
						// making the entrypoints do the casting we keep current code
						// working and minimize surprises. However we do this at the cost
						// of potential runtime type errors. (Though no more than before).
						if (!_convert.Type.IsVoid(returnType))
							callLine = "return (" + returnTypeStr + ") " + callLine;
						ftw.WriteLine(callLine);
						ftw.Unindent("}");
						return tw.ToString();
					}
				}
			}
		}
	}
}