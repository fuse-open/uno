using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Foreign.Java.Converters
{
    internal partial class Converter
	{
		public class NameConverter
		{
		    readonly Converters.Converter _convert;
		    readonly DataType _fixedName;
			static readonly string _nativeMethodPrefix = "callUno_";
			readonly Dictionary<Member, string> _memberNames = new Dictionary<Member, string>();

			int _uid = -1;
			string GetUID(Member forThisMember = null)
			{
				if (forThisMember != null && forThisMember.HasAttribute(_fixedName))
					return "";

				var name = "";
				if (_memberNames.TryGetValue(forThisMember, out name))
					return name;
				_uid += 1;
				name = _uid.ToString();
				_memberNames.Add(forThisMember, name);
				return name;
			}

			public NameConverter(Converters.Converter convert, IILFactory ilFactory)
			{
				_convert = convert;
				_fixedName = ilFactory.GetType("Uno.Compiler.ExportTargetInterop.Android.ForeignFixedNameAttribute");
			}

		    static string PoorlyMangleName(string name)
			{
				var r = name.Replace("<", "_lt_").Replace(">", "_gt_").Replace("[", "_sql_").Replace("]", "_sqr_").Replace('.', '_');
				return r;
			}

			string PoorlyMangleDelgateName(DelegateType d, bool fullyQualified = true)
			{
				var f = d.UnoName;

				if (fullyQualified)
				{
					if (d.Parent != null && !d.Parent.IsRoot)
						f = "com.foreign." + d.Parent.FullName + "." + f;
					else
						f = "com.foreign." + f;
				}

				var p = d.Parameters.Select(x => _convert.Type.UnoToJavaType(x.Type, false));
				if (p.Any())
				{
					p = p.Select(x => x.Substring(x.LastIndexOf('.') + 1));
					f = f + "_" + string.Join("_", p);
				}
				return f;
			}

			public string JavaDelegateName(DelegateType d, bool fullyQualified = false)
			{
				return PoorlyMangleDelgateName(d, fullyQualified);
			}

			// Create the name of the native method that lives on the java side and that c++ will have to register
			public string GenNativeMethodName(Function f)
			{
				return _nativeMethodPrefix + PoorlyMangleName(f.FullName) + GetUID(f);
			}

			// Create the name of the native method that lives on the java side and that c++ will have to register
			public string GenNativeMethodName(DelegateType d)
			{
				return _nativeMethodPrefix + PoorlyMangleDelgateName(d, false);
			}

			public string GenNativeFieldName(Field f, bool isGetter)
			{
				return _nativeMethodPrefix + PoorlyMangleName(f.FullName) + (isGetter ? "Get" : "Set") + GetUID(f);
			}

			public string GenNativePropertyName(Property f, bool isGetter)
			{
				return _nativeMethodPrefix + PoorlyMangleName(f.FullName) + (isGetter ? "Get" : "Set") + GetUID(f);
			}

			public string JavaMethodName(Function f)
			{
				return f.Name + GetUID(f);
			}

			public string ComForeignClassName(DataType dt)
			{
				var name = dt.Name;
				var parent = dt.Parent;
				while (!parent.IsRoot)
				{
					name = parent.Name + "." + name;
					parent = parent.Parent;
				}
				return "com.foreign." + name;
			}
		}
	}
}