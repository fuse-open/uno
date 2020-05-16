using System;
using System.Collections.Generic;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Foreign.Java.Converters
{
    internal partial class Converter
	{
		public class TypeConverter
		{
			readonly DataType _boxedJavaObject;
			readonly IEssentials _essentials;
			readonly Converter _convert;
			readonly ForeignHelpers _helpers;
			public readonly DataType Void;

			public TypeConverter(Converter converter, DataType boxedJavaObject, IEssentials essentials, ForeignHelpers helpers)
			{
				_helpers = helpers;
				_convert = converter;
				_boxedJavaObject = boxedJavaObject;
				_essentials = essentials;
				Void = _essentials.Void;
			}

			public bool IsJavaObject(DataType dt)
			{
				return dt.IsSubclassOfOrEqual(_boxedJavaObject);
			}

			public bool IsVoid(DataType dt)
			{
				return dt.FullName == "void";
			}

			static readonly Dictionary<string, string> _javaTypes = new Dictionary<string, string>
			{
				{ "void", "void" },
				{ "float", "float" },
				{ "double", "double" },
				{ "long", "long" },
				{ "short", "short" },
				{ "char", "char" },
				{ "int", "int" },
				{ "sbyte", "byte" },
				{ "bool", "boolean" },
				{ "string", "String" },
				{ "Uno.String", "String" },
				{ "bool[]", "com.uno.BoolArray" },
				{ "sbyte[]", "com.uno.ByteArray" },
				{ "byte[]", "com.uno.ByteArray" },
				{ "char[]", "com.uno.CharArray" },
				{ "short[]", "com.uno.ShortArray" },
				{ "int[]", "com.uno.IntArray" },
				{ "long[]", "com.uno.LongArray" },
				{ "float[]", "com.uno.FloatArray" },
				{ "double[]", "com.uno.DoubleArray" },
				{ "string[]", "com.uno.StringArray" },
				{ "object[]", "com.uno.ObjectArray" }
			};

			static readonly Dictionary<string, string> _jniCppType = new Dictionary<string, string>
			{
				{ "void", "void" },
				{ "float", "jfloat" },
				{ "double", "jdouble" },
				{ "long", "jlong" },
				{ "short", "jshort" },
				{ "char", "jchar" },
				{ "int", "jint" },
				{ "sbyte", "jbyte" },
				{ "bool", "jboolean" },
				{ "string", "jobject" },
				{ "bool[]", "jobject" },
				{ "byte[]", "jobject" },
				{ "sbyte[]", "jobject" },
				{ "char[]", "jobject" },
				{ "short[]", "jobject" },
				{ "int[]", "jobject" },
				{ "long[]", "jobject" },
				{ "float[]", "jobject" },
				{ "double[]", "jobject" },
				{ "string[]", "jobject" },
				{ "object[]", "jobject" },
			};

			static readonly Dictionary<string, string> _jniSigType = new Dictionary<string, string>
			{
				{ "double", "D" },
				{ "short", "S" },
				{ "float", "F" },
				{ "long", "J" },
				{ "char", "C" },
				{ "void", "V" },
				{ "int", "I" },
				{ "sbyte", "B" },
				{ "bool", "Z" },
				{ "string", "Ljava/lang/String;" },
				{ "Uno.String", "Ljava/lang/String;" },
				{ "bool[]", "Lcom/uno/BoolArray;" },
				{ "byte[]", "Lcom/uno/ByteArray;" },
				{ "sbyte[]", "Lcom/uno/ByteArray;" },
				{ "char[]", "Lcom/uno/CharArray;" },
				{ "short[]", "Lcom/uno/ShortArray;" },
				{ "int[]", "Lcom/uno/IntArray;" },
				{ "long[]", "Lcom/uno/LongArray;" },
				{ "float[]", "Lcom/uno/FloatArray;" },
				{ "double[]", "Lcom/uno/DoubleArray;" },
				{ "string[]", "Lcom/uno/StringArray;" },
				{ "object[]", "Lcom/uno/ObjectArray;" },
			};

			public bool HasJavaEquivalent(DataType dt)
			{
				return _jniSigType.ContainsKey(dt.FullName);
			}

			public string UnoToJniSigType(DataType dt)
			{
				if (dt.IsEnum)
					return UnoToJniSigType(dt.Base);

				string result;
				if (!_jniSigType.TryGetValue(dt.FullName, out result))
				{
					if (dt.IsSubclassOfOrEqual(_essentials.Delegate))
					{
						result = "L" + _convert.Name.JavaDelegateName((DelegateType)dt, true).Replace(".", "/") + ";";
					}
					else if (dt == _boxedJavaObject)
					{
						result = "Ljava/lang/Object;";
					}
					else
					{
						result = "Lcom/uno/UnoObject;";
					}
				}
				return result;
			}

			public string UnoToJniType(DataType dt, bool distinguishStrings=false)
			{
				if (dt.IsEnum)
					return UnoToJniType(dt.Base);
				string result;
				if (!_jniCppType.TryGetValue(dt.FullName, out result))
					result = "jobject";
				if (dt.IsSubclassOfOrEqual(_essentials.String) && distinguishStrings)
					result = "jstring";
				return result;
			}

			public string UnoToJavaType(DataType dt, bool useObjectForUnoObject)
			{
                // If confused about the use of useObjectForUnoObject please see the
                // GenJavaEntrypointMethod method in JavaMethod.cs

				if (dt.IsEnum)
					return UnoToJavaType(dt.Base, useObjectForUnoObject);

				string result;
				if (!_javaTypes.TryGetValue(dt.FullName, out result))
				{
					if (dt.IsSubclassOfOrEqual(_essentials.Delegate))
					{
						result = _convert.Name.JavaDelegateName((DelegateType)dt, true);
					}
					else if (dt == _boxedJavaObject || useObjectForUnoObject)
					{
						result = "Object";
					}
					else
					{
						result = "UnoObject";
					}
				}
				return result;
			}

			public string CastJniToUno(DataType unoType, string line, bool stackArg)
			{
				var unbox = stackArg ? "UnBox" : "UnBoxFreeingLocalRef";
				if (_helpers.IsPrimitive(unoType) || unoType.IsEnum)
					return "(@{" + unoType.FullName + "})" + line;
				if (unoType == _essentials.String)
					return "JniHelper::JavaToUnoString((jstring)" + line + ")";
				if (IsJavaObject(unoType))
					return "(@{" + unoType.FullName + "})@{global::Android.Base.Wrappers.JavaObjectHelper.JObjectToJWrapper(global::Android.Base.Primitives.ujobject,bool):Call(" + line + ", " + stackArg.ToString().ToLower() + ")}";
				if (unoType.IsStruct)
					return _helpers.UnboxStruct(unoType, "@{" + ForeignJavaPass.UnoToJavaBoxingClass.FullName + "." + unbox + "(global::Android.Base.Primitives.ujobject):Call(" + line + ")}");
				if (!IsVoid(unoType))
					return _helpers.CastTo(unoType, "@{" + ForeignJavaPass.UnoToJavaBoxingClass.FullName + "." + unbox + "(global::Android.Base.Primitives.ujobject):Call(" + line + ")}");
				throw new InvalidCastException("ForeignCode: Could not establish how to convert from '" + line + "' to the uno type '" + unoType + "'");
			}

			static int _uniqueVarNum = -1;
			public JniValueCast CastUnoToJni(DataType dt, JniFreeingTechnique free, string jniTmpVarName, string unoTmpVarName = null)
			{
				return new JniValueCast(dt, free, jniTmpVarName, unoTmpVarName ?? "_tmp_" + (_uniqueVarNum += 1),
										_convert, _essentials, _helpers);
			}
		}

		public enum JniFreeingTechnique
		{
			Default, WithScope, None
		}

		public class JniValueCast
		{
			public readonly Func<string, string> UnoTmpVarLet;
			public readonly string UnoTmpVarName;
			public readonly string JniVarName;
			public readonly string CastSet;
			public readonly string CastLet;
			public readonly string Free;

			public JniValueCast(DataType dt, JniFreeingTechnique free, string jniTmpVarName, string unoTmpVarName,
							   Converter convert, IEssentials essentials, ForeignHelpers helpers)
			{
				JniVarName = jniTmpVarName;
				UnoTmpVarName = unoTmpVarName;
				UnoTmpVarLet = line => "@{" + dt.FullName + "} " + unoTmpVarName + "=" + line + ";";
				var typeUsuallyFreed = true;
				string cast;

				if (helpers.IsPrimitive(dt) || dt.IsEnum)
				{
					cast = "(" + convert.Type.UnoToJniType(dt) + ")" + unoTmpVarName;
					typeUsuallyFreed = false;
				}
				else if (dt == essentials.String)
				{
					cast = "JniHelper::UnoToJavaString(" + unoTmpVarName + ")";
				}
				else if (convert.Type.IsJavaObject(dt))
				{
					cast = "(" + unoTmpVarName + "==nullptr ? nullptr : U_JNIVAR->NewLocalRef(@{global::Android.Base.Wrappers.IJWrapper:Of((@{global::Android.Base.Wrappers.IJWrapper})" + unoTmpVarName + ")._GetJavaObject():Call()}))";
				}
				else if (dt.IsSubclassOfOrEqual(essentials.Delegate))
				{
					var d = (DelegateType)dt;
					cast = "@{" + ForeignJavaPass.UnoToJavaBoxingClass.FullName + ".BoxDelegate(object,global::Android.Base.Primitives.ConstCharPtr):Call((@{object})" + unoTmpVarName + ", \"" + convert.Name.JavaDelegateName(d, true) + "\")}";
				}
				else if (convert.Type.HasJavaEquivalent(dt))
				{
					cast = "@{" + ForeignJavaPass.UnoToJavaBoxingClass.FullName + ".Box(" + dt.FullName + "):Call(" + unoTmpVarName + ")}";
				}
				else if (dt.IsStruct)
				{
					cast = "@{" + ForeignJavaPass.UnoToJavaBoxingClass.FullName + ".Box(object):Call(" + helpers.BoxStruct(dt, unoTmpVarName) + ")}";
				}
				else
				{
					cast = "@{" + ForeignJavaPass.UnoToJavaBoxingClass.FullName + ".Box(object):Call(" + unoTmpVarName + ")}";
				}

				if (typeUsuallyFreed)
				{
					switch (free)
					{
						case JniFreeingTechnique.Default:
							Free = "if (" + JniVarName + "!=nullptr) { U_JNIVAR->DeleteLocalRef(" + JniVarName + "); }";
							break;
						case JniFreeingTechnique.WithScope:
							if (dt.IsStruct) // no null check needed if is uno struct
								cast = "U_JNIVAR->NewLocalRef((" + cast + "))";
							else
								cast = "(" + unoTmpVarName + "==nullptr ? nullptr : U_JNIVAR->NewLocalRef((" + cast + ")))";
							Free = "";
							break;
						case JniFreeingTechnique.None:
							Free = "";
							break;
					    default:
					        throw new ArgumentOutOfRangeException(nameof(free), free, null);
					}
				}
				CastSet = jniTmpVarName + " = " + cast + ";";
				CastLet = convert.Type.UnoToJniType(dt, true) + " " + CastSet;
			}
		}
	}
}
