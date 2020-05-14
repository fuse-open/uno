
using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Backends.CPlusPlus;

namespace Uno.Compiler.Foreign
{
	class ForeignHelpers
	{
		readonly IEnvironment Environment;
		readonly IEssentials Essentials;
		readonly CppBackend Backend;
		readonly IBuildData Data;
		Function _context;
		Source _source;


		public ForeignHelpers(IEnvironment environment, IEssentials essentials, CppBackend backend, IBuildData data)
		{
			Environment = environment;
			Essentials = essentials;
			Backend = backend;
			Data = data;
		}

		public void CacheContext(Function context, Source source)
		{
			_context = context;
			_source = source;
		}

		public string GetForeignAttribute(Function f)
		{
			var faIndex = f.Prototype.TryGetAttribute(Essentials.ForeignAttribute);
			if (faIndex != null)
			{
				// Filter out the generated `New` function
				if (!f.IsGenerated)
				{
					return Essentials.Language.Literals[(int)faIndex].Name;
				}
			}
			return null;
		}

		public List<string> GetForeignIncludes(DataType dt, string language, IEnvironment env)
		{
			var result = new List<string>();

			if (dt.HasAttribute(Essentials.ForeignIncludeAttribute))
			{
				foreach (var attr in dt.Attributes)
				{
					if (attr.ReferencedType == Essentials.ForeignIncludeAttribute &&
						Essentials.Language.Literals[(int)attr.Arguments[0].ConstantValue].Name == language)
					{
						foreach (var arg in attr.Arguments.Skip(1))
						{
							result.Add(env.Expand(dt.Source, (string)arg.ConstantValue));
						}
					}
				}
			}

			return result;
		}

		public List<string> GetForeignAnnotations(Function f, string language)
		{
			var result = new List<string>();

			if (f.HasAttribute(Essentials.ForeignAnnotationAttribute))
			{
				foreach (var attr in f.Attributes)
				{
					if (attr.ReferencedType == Essentials.ForeignAnnotationAttribute &&
						Essentials.Language.Literals[(int)attr.Arguments[0].ConstantValue].Name == language)
					{
						foreach (var arg in attr.Arguments.Skip(1))
						{
							result.Add((string)arg.ConstantValue);
						}
					}
				}
			}

			return result;
		}

		public ExternScope GetForeignExternScopeFromFunction(Function f)
		{
			var externScope = f.Body.Statements[f.Body.Statements.Count - 1] as ExternScope;

			if (externScope != null)
			{
				return externScope;
			}

			throw new Exception(f.Source + "The body of a Foreign function must be declared as '@{...@}'.");
		}

		public string GetForeignCodeFromFunction(Function f)
		{
			return GetForeignExternScopeFromFunction(f).String;
		}

		public Namescope[] GetExternFunctionScopes(Function f)
		{
			return GetForeignExternScopeFromFunction(f).Scopes;
		}

		public void ReplaceBody(Function f, string newCode)
		{
			var originalScope = GetForeignExternScopeFromFunction(f);

		    var newScope = new ExternScope(
		        originalScope.Source,
		        AttributeList.Empty,
		        newCode,
		        CreateObject(originalScope.Source, f),
		        originalScope.Arguments,
		        originalScope.Scopes);

		    f.Body.Statements[f.Body.Statements.Count - 1] = newScope;
		}

		// TODO merge with ExtensionTransform.CreateObject?
		static Expression CreateObject(Source src, Function f)
		{
			if (f.IsStatic)
				return null;

			return new This(src, f.DeclaringType).Address;
		}

		public bool TryAddProperty(DataType target, string key, Element element)
		{
			TypeExtension ext = null;
			if (!Environment.TryGetExtension(target, out ext))
			{
				ext = new TypeExtension(target.Source, target);
				Data.Extensions.TypeExtensions[target] = ext;
			}

			Element existingElement;

			if (!ext.Properties.TryGetValue(key, out existingElement))
			{
				ext.Properties.Add(key, element);
				return true;
			}
			return false;
		}

		public string FullGlobalName(DataType dt)
		{
			return NeedsGlobalPrefix(dt) ? "global::" + dt.FullName : dt.FullName;
		}

		bool NeedsGlobalPrefix(DataType dt)
		{
			if (dt.IsArray)
				return NeedsGlobalPrefix(dt.ElementType);

			return !(IsPrimitive(dt) ||
				dt.IsVoid ||
				dt == Essentials.String ||
				dt == Essentials.Object);
		}

		public void SourceDeclaration(string declaration, DataType target, Source s = null)
		{
			if (s == null)
				s = target.Source;
			SourceDeclaration(new List<string>() { declaration }, target, s);
		}

		public void SourceDeclaration(List<string> declarations, DataType target, Source s = null)
		{
			if (s == null)
				s = target.Source;
			Environment.Require(target, "Source.Declaration",
				declarations.Select(d => new Element(s, d)).ToArray());
		}

		public void SourceInclude(string include, DataType target, Source s = null)
		{
			if (s == null)
				s = target.Source;
			SourceInclude(new List<string>() { include }, target, s);
		}

		public void SourceInclude(IEnumerable<string> includes, DataType target, Source s = null)
		{
			if (s == null)
				s = target.Source;
			Environment.Require(target, "Source.Include",
				includes.Select(d => new Element(s, d)).ToArray());
		}

		public void Require(DataType dt, DataType target, Source s = null)
		{
			if (s == null)
				s = target.Source;
			Require(new List<DataType>() { dt }, target, s);
		}

		public void Require(List<DataType> types, DataType target, Source s = null)
		{
			if (s == null)
				s = target.Source;
			Environment.Require(target, "Entity",
				types.Select(t => new Element(s, t.FullName)).ToArray());
		}

		public void Require(Member f, DataType target, Source s = null)
		{
			if (s == null)
				s = target.Source;
			Require(new List<Member>() { f }, target, s);
		}

		public void Require(List<Member> members, DataType target, Source s = null)
		{
			if (s == null)
				s = target.Source;
			Environment.Require(target, "Entity",
				members.Select(f => new Element(s, UnoSignature(f))).ToArray());
		}

		public string UnoSignature(Member m)
		{
			if (m is Field)
				return (m as Field).FullName;
			else if (m is Property)
				return (m as Property).FullName;
			else if (m is Function)
				return (m as Function).FullName + "(" + string.Join(",", (m as Function).Parameters.Select(x => x.Type.FullName)) + ")";
			else
				throw new Exception("Foreign Code: invalid member type to generate signature from");
		}

		public bool IsPrimitive(Parameter p)
		{
			return IsPrimitive(p.Type);
		}

		public bool IsPrimitive(DataType dt)
		{
			return dt == Essentials.Bool ||
				dt == Essentials.Byte ||
				dt == Essentials.SByte ||
				dt == Essentials.Char ||
				dt == Essentials.Double ||
				dt == Essentials.Float ||
				dt == Essentials.Int ||
				dt == Essentials.UInt ||
				dt == Essentials.Long ||
				dt == Essentials.ULong ||
				dt == Essentials.Short ||
				dt == Essentials.UShort;
		}


		public Expression StringExpr(DataType dt, string e)
		{
			return new StringExpression(_source, dt, e);
		}

		public string CompiledType(DataType dt)
		{
			return Backend.Decompiler.GetType(_source, _context, dt);
		}

		public string CallStatic(Method method, params Expression[] args)
		{
			return Backend.Decompiler.GetCallMethod(_source, _context, method, null, args);
		}

		public string CallDelegate(Expression del, Expression[] args)
		{
			return Backend.Decompiler.GetCallDelegate(_source, _context, del, args);
		}

		public string ArrayGet(Expression arr, Expression index)
		{
			return Backend.Decompiler.GetLoadElement(_source, _context, ((ArrayType)arr.ReturnType).ElementType, arr, index);
		}

		public string ArraySet(Expression arr, Expression index, Expression value)
		{
			return Backend.Decompiler.GetStoreElement(_source, _context, ((ArrayType)arr.ReturnType).ElementType, arr, index, value);
		}

		public string TypeOf(DataType dt)
		{
			return Backend.Decompiler.GetTypeOf(_source, _context, dt);
		}

		public string BoxStruct(DataType dt, string str)
		{
			// there is no real reason that this couldnt be used on more types.
			// however this is a quick fix and I am not gonna risk that.
			if (!dt.IsStruct)
				throw new Exception("Called BoxStruct on non-struct datatype " + dt.FullName);
			return "uBox(" + TypeOf(dt) + ", " + str + ")";
		}

		public string UnboxStruct(DataType dt, string str)
		{
			// there is no real reason that this couldnt be used on more types.
			// however this is a quick fix and I am not gonna risk that.
			if (!dt.IsStruct)
				throw new Exception("Called UnBoxStruct on non-struct datatype " + dt.FullName);
			return "uUnbox<@{" + dt.FullName + "}>(" + TypeOf(dt) + ", " + str + ")";
		}

		public string CastTo(DataType dt, string x)
		{
			return dt == Essentials.Object
				? x
				: ("uCast<" + CompiledType(dt) + ">(" + x + ", " + TypeOf(dt) + ")");
		}
	}
}