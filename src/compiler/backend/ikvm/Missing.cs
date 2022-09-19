/*
  Copyright (C) 2011-2012 Jeroen Frijters

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.

  Jeroen Frijters
  jeroen@frijters.net
  
*/
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IKVM.Reflection
{
#if !CORECLR
	[Serializable]
#endif
	public sealed class MissingAssemblyException : InvalidOperationException
	{
#if !CORECLR
		[NonSerialized]
#endif
		private readonly MissingAssembly assembly;

		internal MissingAssemblyException(MissingAssembly assembly)
			: base("Assembly '" + assembly.FullName + "' is a missing assembly and does not support the requested operation.")
		{
			this.assembly = assembly;
		}

#if !CORECLR
		private MissingAssemblyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
#endif

		public Assembly Assembly
		{
			get { return assembly; }
		}
	}

#if !CORECLR
	[Serializable]
#endif
	public sealed class MissingModuleException : InvalidOperationException
	{
#if !CORECLR
		[NonSerialized]
#endif
		private readonly MissingModule module;

		internal MissingModuleException(MissingModule module)
			: base("Module from missing assembly '" + module.Assembly.FullName + "' does not support the requested operation.")
		{
			this.module = module;
		}

#if !CORECLR
		private MissingModuleException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
#endif

		public Module Module
		{
			get { return module; }
		}
	}

#if !CORECLR
	[Serializable]
#endif
	public sealed class MissingMemberException : InvalidOperationException
	{
#if !CORECLR
		[NonSerialized]
#endif
		private readonly MemberInfo member;

		internal MissingMemberException(MemberInfo member)
			: base("Member '" + member + "' is a missing member and does not support the requested operation.")
		{
			this.member = member;
		}

#if !CORECLR
		private MissingMemberException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
#endif

		public MemberInfo MemberInfo
		{
			get { return member; }
		}
	}

	public struct MissingGenericMethodBuilder
	{
		private readonly MissingMethod method;

		public MissingGenericMethodBuilder(Type declaringType, CallingConventions callingConvention, string name, int genericParameterCount)
		{
			method = new MissingMethod(declaringType, name, new MethodSignature(null, null, new PackedCustomModifiers(), callingConvention, genericParameterCount));
		}

		public Type[] GetGenericArguments()
		{
			return method.GetGenericArguments();
		}

		public void SetSignature(Type returnType, CustomModifiers returnTypeCustomModifiers, Type[] parameterTypes, CustomModifiers[] parameterTypeCustomModifiers)
		{
			method.signature = new MethodSignature(
				returnType ?? method.Module.universe.System_Void,
				Util.Copy(parameterTypes),
				PackedCustomModifiers.CreateFromExternal(returnTypeCustomModifiers, parameterTypeCustomModifiers, parameterTypes.Length),
				method.signature.CallingConvention,
				method.signature.GenericParameterCount);
		}

		[Obsolete("Please use SetSignature(Type, CustomModifiers, Type[], CustomModifiers[]) instead.")]
		public void SetSignature(Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
		{
			method.signature = new MethodSignature(
				returnType ?? method.Module.universe.System_Void,
				Util.Copy(parameterTypes),
				PackedCustomModifiers.CreateFromExternal(returnTypeOptionalCustomModifiers, returnTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers, parameterTypeRequiredCustomModifiers, parameterTypes.Length),
				method.signature.CallingConvention,
				method.signature.GenericParameterCount);
		}

		public MethodInfo Finish()
		{
			return method;
		}
	}

	sealed class MissingAssembly : Assembly
	{
		private readonly MissingModule module;

		internal MissingAssembly(Universe universe, string name)
			: base(universe)
		{
			module = new MissingModule(this, -1);
			this.fullName = name;
		}

		public override Type[] GetTypes()
		{
			throw new MissingAssemblyException(this);
		}

		public override AssemblyName GetName()
		{
			return new AssemblyName(fullName);
		}

		public override string ImageRuntimeVersion
		{
			get { throw new MissingAssemblyException(this); }
		}

		public override Module ManifestModule
		{
			get { return module; }
		}

		public override MethodInfo EntryPoint
		{
			get { throw new MissingAssemblyException(this); }
		}

		public override string Location
		{
			get { throw new MissingAssemblyException(this); }
		}

		public override AssemblyName[] GetReferencedAssemblies()
		{
			throw new MissingAssemblyException(this);
		}

		public override Module[] GetModules(bool getResourceModules)
		{
			throw new MissingAssemblyException(this);
		}

		public override Module[] GetLoadedModules(bool getResourceModules)
		{
			throw new MissingAssemblyException(this);
		}

		public override Module GetModule(string name)
		{
			throw new MissingAssemblyException(this);
		}

		public override string[] GetManifestResourceNames()
		{
			throw new MissingAssemblyException(this);
		}

		public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			throw new MissingAssemblyException(this);
		}

		public override System.IO.Stream GetManifestResourceStream(string resourceName)
		{
			throw new MissingAssemblyException(this);
		}

		public override bool __IsMissing
		{
			get { return true; }
		}

		internal override Type FindType(TypeName typeName)
		{
			return null;
		}

		internal override Type FindTypeIgnoreCase(TypeName lowerCaseName)
		{
			return null;
		}

		internal override IList<CustomAttributeData> GetCustomAttributesData(Type attributeType)
		{
			throw new MissingAssemblyException(this);
		}
	}

	sealed class MissingModule : NonPEModule
	{
		private readonly Assembly assembly;
		private readonly int index;

		internal MissingModule(Assembly assembly, int index)
			: base(assembly.universe)
		{
			this.assembly = assembly;
			this.index = index;
		}

		public override int MDStreamVersion
		{
			get { throw new MissingModuleException(this); }
		}

		public override Assembly Assembly
		{
			get { return assembly; }
		}

		public override string FullyQualifiedName
		{
			get { throw new MissingModuleException(this); }
		}

		public override string Name
		{
			get
			{
				if (index == -1)
				{
					throw new MissingModuleException(this);
				}
				return assembly.ManifestModule.GetString(assembly.ManifestModule.File.records[index].Name);
			}
		}

		public override Guid ModuleVersionId
		{
			get { throw new MissingModuleException(this); }
		}

		public override string ScopeName
		{
			get { throw new MissingModuleException(this); }
		}

		internal override Type FindType(TypeName typeName)
		{
			return null;
		}

		internal override Type FindTypeIgnoreCase(TypeName lowerCaseName)
		{
			return null;
		}

		internal override void GetTypesImpl(System.Collections.Generic.List<Type> list)
		{
			throw new MissingModuleException(this);
		}

		public override void __GetDataDirectoryEntry(int index, out int rva, out int length)
		{
			throw new MissingModuleException(this);
		}

		public override IList<CustomAttributeData> __GetPlaceholderAssemblyCustomAttributes(bool multiple, bool security)
		{
			throw new MissingModuleException(this);
		}

		public override long __RelativeVirtualAddressToFileOffset(int rva)
		{
			throw new MissingModuleException(this);
		}

		public override __StandAloneMethodSig __ResolveStandAloneMethodSig(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new MissingModuleException(this);
		}

		public override int __Subsystem
		{
			get { throw new MissingModuleException(this); }
		}

		internal override void ExportTypes(int fileToken, IKVM.Reflection.Emit.ModuleBuilder manifestModule)
		{
			throw new MissingModuleException(this);
		}

		public override void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			throw new MissingModuleException(this);
		}

		public override bool __IsMissing
		{
			get { return true; }
		}

		protected override Exception InvalidOperationException()
		{
			return new MissingModuleException(this);
		}

		protected override Exception NotSupportedException()
		{
			return new MissingModuleException(this);
		}

		protected override Exception ArgumentOutOfRangeException()
		{
			return new MissingModuleException(this);
		}

		public override byte[] __ModuleHash
		{
			get
			{
				if (index == -1)
				{
					throw new MissingModuleException(this);
				}
				if (assembly.ManifestModule.File.records[index].HashValue == 0)
				{
					return null;
				}
				IKVM.Reflection.Reader.ByteReader br = assembly.ManifestModule.GetBlob(assembly.ManifestModule.File.records[index].HashValue);
				return br.ReadBytes(br.Length);
			}
		}
	}

	sealed class MissingType : Type
	{
		private readonly Module module;
		private readonly Type declaringType;
		private readonly string ns;
		private readonly string name;
		private Type[] typeArgs;
		private int token;
		private int flags;
		private bool cyclicTypeForwarder;

		internal MissingType(Module module, Type declaringType, string ns, string name)
		{
			this.module = module;
			this.declaringType = declaringType;
			this.ns = ns;
			this.name = name;
			MarkKnownType(ns, name);

			// HACK we need to handle the Windows Runtime projected types that change from ValueType to Class or v.v.
			if (WindowsRuntimeProjection.IsProjectedValueType(ns, name, module))
			{
				typeFlags |= TypeFlags.ValueType;
			}
			else if (WindowsRuntimeProjection.IsProjectedReferenceType(ns, name, module))
			{
				typeFlags |= TypeFlags.NotValueType;
			}
		}

		internal override MethodBase FindMethod(string name, MethodSignature signature)
		{
			MethodInfo method = new MissingMethod(this, name, signature);
			if (name == ".ctor")
			{
				return new ConstructorInfoImpl(method);
			}
			return method;
		}

		internal override FieldInfo FindField(string name, FieldSignature signature)
		{
			return new MissingField(this, name, signature);
		}

		internal override Type FindNestedType(TypeName name)
		{
			return null;
		}

		internal override Type FindNestedTypeIgnoreCase(TypeName lowerCaseName)
		{
			return null;
		}

		public override bool __IsMissing
		{
			get { return true; }
		}

		public override Type DeclaringType
		{
			get { return declaringType; }
		}

		internal override TypeName TypeName
		{
			get { return new TypeName(ns, name); }
		}

		public override string Name
		{
			get { return TypeNameParser.Escape(name); }
		}

		public override string FullName
		{
			get { return GetFullName(); }
		}

		public override Module Module
		{
			get { return module; }
		}

		public override int MetadataToken
		{
			get { return token; }
		}

		public override bool IsValueType
		{
			get
			{
				switch (typeFlags & (TypeFlags.ValueType | TypeFlags.NotValueType))
				{
					case TypeFlags.ValueType:
						return true;
					case TypeFlags.NotValueType:
						return false;
					case TypeFlags.ValueType | TypeFlags.NotValueType:
						if (WindowsRuntimeProjection.IsProjectedValueType(ns, name, module))
						{
							typeFlags &= ~TypeFlags.NotValueType;
							return true;
						}
						if (WindowsRuntimeProjection.IsProjectedReferenceType(ns, name, module))
						{
							typeFlags &= ~TypeFlags.ValueType;
							return false;
						}
						goto default;
					default:
						if (module.universe.ResolveMissingTypeIsValueType(this))
						{
							typeFlags |= TypeFlags.ValueType;
						}
						else
						{
							typeFlags |= TypeFlags.NotValueType;
						}
						return (typeFlags & TypeFlags.ValueType) != 0;
				}
			}
		}

		public override Type BaseType
		{
			get { throw new MissingMemberException(this); }
		}

		public override TypeAttributes Attributes
		{
			get { throw new MissingMemberException(this); }
		}

		public override Type[] __GetDeclaredTypes()
		{
			throw new MissingMemberException(this);
		}

		public override Type[] __GetDeclaredInterfaces()
		{
			throw new MissingMemberException(this);
		}

		public override MethodBase[] __GetDeclaredMethods()
		{
			throw new MissingMemberException(this);
		}

		public override __MethodImplMap __GetMethodImplMap()
		{
			throw new MissingMemberException(this);
		}

		public override FieldInfo[] __GetDeclaredFields()
		{
			throw new MissingMemberException(this);
		}

		public override EventInfo[] __GetDeclaredEvents()
		{
			throw new MissingMemberException(this);
		}

		public override PropertyInfo[] __GetDeclaredProperties()
		{
			throw new MissingMemberException(this);
		}

		public override CustomModifiers __GetCustomModifiers()
		{
			throw new MissingMemberException(this);
		}

		public override Type[] GetGenericArguments()
		{
			throw new MissingMemberException(this);
		}

		public override CustomModifiers[] __GetGenericArgumentsCustomModifiers()
		{
			throw new MissingMemberException(this);
		}

		public override bool __GetLayout(out int packingSize, out int typeSize)
		{
			throw new MissingMemberException(this);
		}

		public override bool IsGenericType
		{
			get { throw new MissingMemberException(this); }
		}

		public override bool IsGenericTypeDefinition
		{
			get { throw new MissingMemberException(this); }
		}

		internal override Type GetGenericTypeArgument(int index)
		{
			if (typeArgs == null)
			{
				typeArgs = new Type[index + 1];
			}
			else if (typeArgs.Length <= index)
			{
				Array.Resize(ref typeArgs, index + 1);
			}
			return typeArgs[index] ?? (typeArgs[index] = new MissingTypeParameter(this, index));
		}

		internal override Type BindTypeParameters(IGenericBinder binder)
		{
			return this;
		}

		internal override Type SetMetadataTokenForMissing(int token, int flags)
		{
			this.token = token;
			this.flags = flags;
			return this;
		}

		internal override Type SetCyclicTypeForwarder()
		{
			this.cyclicTypeForwarder = true;
			return this;
		}

		internal override bool IsBaked
		{
			get { throw new MissingMemberException(this); }
		}

		public override bool __IsTypeForwarder
		{
			// CorTypeAttr.tdForwarder
			get { return (flags & 0x00200000) != 0; }
		}

		public override bool __IsCyclicTypeForwarder
		{
			get { return cyclicTypeForwarder; }
		}
	}

	sealed class MissingTypeParameter : IKVM.Reflection.Reader.TypeParameterType
	{
		private readonly MemberInfo owner;
		private readonly int index;

		internal MissingTypeParameter(Type owner, int index)
			: this(owner, index, Signature.ELEMENT_TYPE_VAR)
		{
		}

		internal MissingTypeParameter(MethodInfo owner, int index)
			: this(owner, index, Signature.ELEMENT_TYPE_MVAR)
		{
		}

		private MissingTypeParameter(MemberInfo owner, int index, byte sigElementType)
			: base(sigElementType)
		{
			this.owner = owner;
			this.index = index;
		}

		public override Module Module
		{
			get { return owner.Module; }
		}

		public override string Name
		{
			get { return null; }
		}

		public override int GenericParameterPosition
		{
			get { return index; }
		}

		public override MethodBase DeclaringMethod
		{
			get { return owner as MethodBase; }
		}

		public override Type DeclaringType
		{
			get { return owner as Type; }
		}

		internal override Type BindTypeParameters(IGenericBinder binder)
		{
			if (owner is MethodBase)
			{
				return binder.BindMethodParameter(this);
			}
			else
			{
				return binder.BindTypeParameter(this);
			}
		}

		internal override bool IsBaked
		{
			get { return owner.IsBaked; }
		}
	}

	sealed class MissingMethod : MethodInfo
	{
		private readonly Type declaringType;
		private readonly string name;
		internal MethodSignature signature;
		private MethodInfo forwarder;
		private Type[] typeArgs;

		internal MissingMethod(Type declaringType, string name, MethodSignature signature)
		{
			this.declaringType = declaringType;
			this.name = name;
			this.signature = signature;
		}

		private MethodInfo Forwarder
		{
			get
			{
				MethodInfo method = TryGetForwarder();
				if (method == null)
				{
					throw new MissingMemberException(this);
				}
				return method;
			}
		}

		private MethodInfo TryGetForwarder()
		{
			if (forwarder == null && !declaringType.__IsMissing)
			{
				MethodBase mb = declaringType.FindMethod(name, signature);
				ConstructorInfo ci = mb as ConstructorInfo;
				if (ci != null)
				{
					forwarder = ci.GetMethodInfo();
				}
				else
				{
					forwarder = (MethodInfo)mb;
				}
			}
			return forwarder;
		}

		public override bool __IsMissing
		{
			get { return TryGetForwarder() == null; }
		}

		public override Type ReturnType
		{
			get { return signature.GetReturnType(this); }
		}

		public override ParameterInfo ReturnParameter
		{
			get { return new ParameterInfoImpl(this, -1); }
		}

		internal override MethodSignature MethodSignature
		{
			get { return signature; }
		}

		internal override int ParameterCount
		{
			get { return signature.GetParameterCount(); }
		}

		private sealed class ParameterInfoImpl : ParameterInfo
		{
			private readonly MissingMethod method;
			private readonly int index;

			internal ParameterInfoImpl(MissingMethod method, int index)
			{
				this.method = method;
				this.index = index;
			}

			private ParameterInfo Forwarder
			{
				get { return index == -1 ? method.Forwarder.ReturnParameter : method.Forwarder.GetParameters()[index]; }
			}

			public override string Name
			{
				get { return Forwarder.Name; }
			}

			public override Type ParameterType
			{
				get { return index == -1 ? method.signature.GetReturnType(method) : method.signature.GetParameterType(method, index); }
			}

			public override ParameterAttributes Attributes
			{
				get { return Forwarder.Attributes; }
			}

			public override int Position
			{
				get { return index; }
			}

			public override object RawDefaultValue
			{
				get { return Forwarder.RawDefaultValue; }
			}

			public override CustomModifiers __GetCustomModifiers()
			{
				return index == -1
					? method.signature.GetReturnTypeCustomModifiers(method)
					: method.signature.GetParameterCustomModifiers(method, index);
			}

			public override bool __TryGetFieldMarshal(out FieldMarshal fieldMarshal)
			{
				return Forwarder.__TryGetFieldMarshal(out fieldMarshal);
			}

			public override MemberInfo Member
			{
				get { return method; }
			}

			public override int MetadataToken
			{
				get { return Forwarder.MetadataToken; }
			}

			internal override Module Module
			{
				get { return method.Module; }
			}

			public override string ToString()
			{
				return Forwarder.ToString();
			}
		}

		public override ParameterInfo[] GetParameters()
		{
			ParameterInfo[] parameters = new ParameterInfo[signature.GetParameterCount()];
			for (int i = 0; i < parameters.Length; i++)
			{
				parameters[i] = new ParameterInfoImpl(this, i);
			}
			return parameters;
		}

		public override MethodAttributes Attributes
		{
			get { return Forwarder.Attributes; }
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return Forwarder.GetMethodImplementationFlags();
		}

		public override MethodBody GetMethodBody()
		{
			return Forwarder.GetMethodBody();
		}

		public override int __MethodRVA
		{
			get { return Forwarder.__MethodRVA; }
		}

		public override CallingConventions CallingConvention
		{
			get { return signature.CallingConvention; }
		}

		internal override int ImportTo(IKVM.Reflection.Emit.ModuleBuilder module)
		{
			MethodInfo method = TryGetForwarder();
			if (method != null)
			{
				return method.ImportTo(module);
			}
			return module.ImportMethodOrField(declaringType, this.Name, this.MethodSignature);
		}

		public override string Name
		{
			get { return name; }
		}

		public override Type DeclaringType
		{
			get { return declaringType.IsModulePseudoType ? null : declaringType; }
		}

		public override Module Module
		{
			get { return declaringType.Module; }
		}

		public override bool Equals(object obj)
		{
			MissingMethod other = obj as MissingMethod;
			return other != null
				&& other.declaringType == declaringType
				&& other.name == name
				&& other.signature.Equals(signature);
		}

		public override int GetHashCode()
		{
			return declaringType.GetHashCode() ^ name.GetHashCode() ^ signature.GetHashCode();
		}

		internal override MethodBase BindTypeParameters(Type type)
		{
			MethodInfo forwarder = TryGetForwarder();
			if (forwarder != null)
			{
				return forwarder.BindTypeParameters(type);
			}
			return new GenericMethodInstance(type, this, null);
		}

		public override bool ContainsGenericParameters
		{
			get { return Forwarder.ContainsGenericParameters; }
		}

		public override Type[] GetGenericArguments()
		{
			MethodInfo method = TryGetForwarder();
			if (method != null)
			{
				return Forwarder.GetGenericArguments();
			}
			if (typeArgs == null)
			{
				typeArgs = new Type[signature.GenericParameterCount];
				for (int i = 0; i < typeArgs.Length; i++)
				{
					typeArgs[i] = new MissingTypeParameter(this, i);
				}
			}
			return Util.Copy(typeArgs);
		}

		internal override Type GetGenericMethodArgument(int index)
		{
			return GetGenericArguments()[index];
		}

		internal override int GetGenericMethodArgumentCount()
		{
			return Forwarder.GetGenericMethodArgumentCount();
		}

		public override MethodInfo GetGenericMethodDefinition()
		{
			return Forwarder.GetGenericMethodDefinition();
		}

		internal override MethodInfo GetMethodOnTypeDefinition()
		{
			return Forwarder.GetMethodOnTypeDefinition();
		}

		internal override bool HasThis
		{
			get { return (signature.CallingConvention & (CallingConventions.HasThis | CallingConventions.ExplicitThis)) == CallingConventions.HasThis; }
		}

		public override bool IsGenericMethod
		{
			get { return IsGenericMethodDefinition; }
		}

		public override bool IsGenericMethodDefinition
		{
			get { return signature.GenericParameterCount != 0; }
		}

		public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
		{
			MethodInfo method = TryGetForwarder();
			if (method != null)
			{
				return method.MakeGenericMethod(typeArguments);
			}
			return new GenericMethodInstance(declaringType, this, typeArguments);
		}

		public override int MetadataToken
		{
			get { return Forwarder.MetadataToken; }
		}

		internal override int GetCurrentToken()
		{
			return Forwarder.GetCurrentToken();
		}

		internal override bool IsBaked
		{
			get { return Forwarder.IsBaked; }
		}
	}

	sealed class MissingField : FieldInfo
	{
		private readonly Type declaringType;
		private readonly string name;
		private readonly FieldSignature signature;
		private FieldInfo forwarder;

		internal MissingField(Type declaringType, string name, FieldSignature signature)
		{
			this.declaringType = declaringType;
			this.name = name;
			this.signature = signature;
		}

		private FieldInfo Forwarder
		{
			get
			{
				FieldInfo field = TryGetForwarder();
				if (field == null)
				{
					throw new MissingMemberException(this);
				}
				return field;
			}
		}

		private FieldInfo TryGetForwarder()
		{
			if (forwarder == null && !declaringType.__IsMissing)
			{
				forwarder = declaringType.FindField(name, signature);
			}
			return forwarder;
		}

		public override bool __IsMissing
		{
			get { return TryGetForwarder() == null; }
		}

		public override FieldAttributes Attributes
		{
			get { return Forwarder.Attributes; }
		}

		public override void __GetDataFromRVA(byte[] data, int offset, int length)
		{
			Forwarder.__GetDataFromRVA(data, offset, length);
		}

		public override int __FieldRVA
		{
			get { return Forwarder.__FieldRVA; }
		}

		public override bool __TryGetFieldOffset(out int offset)
		{
			return Forwarder.__TryGetFieldOffset(out offset);
		}

		public override object GetRawConstantValue()
		{
			return Forwarder.GetRawConstantValue();
		}

		internal override FieldSignature FieldSignature
		{
			get { return signature; }
		}

		internal override int ImportTo(IKVM.Reflection.Emit.ModuleBuilder module)
		{
			FieldInfo field = TryGetForwarder();
			if (field != null)
			{
				return field.ImportTo(module);
			}
			return module.ImportMethodOrField(declaringType, this.Name, this.FieldSignature);
		}

		public override string Name
		{
			get { return name; }
		}

		public override Type DeclaringType
		{
			get { return declaringType.IsModulePseudoType ? null : declaringType; }
		}

		public override Module Module
		{
			get { return declaringType.Module; }
		}

		internal override FieldInfo BindTypeParameters(Type type)
		{
			FieldInfo forwarder = TryGetForwarder();
			if (forwarder != null)
			{
				return forwarder.BindTypeParameters(type);
			}
			return new GenericFieldInstance(type, this);
		}

		public override int MetadataToken
		{
			get { return Forwarder.MetadataToken; }
		}

		public override bool Equals(object obj)
		{
			MissingField other = obj as MissingField;
			return other != null
				&& other.declaringType == declaringType
				&& other.name == name
				&& other.signature.Equals(signature);
		}

		public override int GetHashCode()
		{
			return declaringType.GetHashCode() ^ name.GetHashCode() ^ signature.GetHashCode();
		}

		public override string ToString()
		{
			return this.FieldType.Name + " " + this.Name;
		}

		internal override int GetCurrentToken()
		{
			return Forwarder.GetCurrentToken();
		}

		internal override bool IsBaked
		{
			get { return Forwarder.IsBaked; }
		}
	}

	// NOTE this is currently only used by CustomAttributeData (because there is no other way to refer to a property)
	sealed class MissingProperty : PropertyInfo
	{
		private readonly Type declaringType;
		private readonly string name;
		private readonly PropertySignature signature;

		internal MissingProperty(Type declaringType, string name, PropertySignature signature)
		{
			this.declaringType = declaringType;
			this.name = name;
			this.signature = signature;
		}

		public override PropertyAttributes Attributes
		{
			get { throw new MissingMemberException(this); }
		}

		public override bool CanRead
		{
			get { throw new MissingMemberException(this); }
		}

		public override bool CanWrite
		{
			get { throw new MissingMemberException(this); }
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			throw new MissingMemberException(this);
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			throw new MissingMemberException(this);
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			throw new MissingMemberException(this);
		}

		public override object GetRawConstantValue()
		{
			throw new MissingMemberException(this);
		}

		internal override bool IsPublic
		{
			get { throw new MissingMemberException(this); }
		}

		internal override bool IsNonPrivate
		{
			get { throw new MissingMemberException(this); }
		}

		internal override bool IsStatic
		{
			get { throw new MissingMemberException(this); }
		}

		internal override PropertySignature PropertySignature
		{
			get { return signature; }
		}

		public override string Name
		{
			get { return name; }
		}

		public override Type DeclaringType
		{
			get { return declaringType; }
		}

		public override Module Module
		{
			get { return declaringType.Module; }
		}

		internal override bool IsBaked
		{
			get { return declaringType.IsBaked; }
		}

		internal override int GetCurrentToken()
		{
			throw new MissingMemberException(this);
		}
	}
}
