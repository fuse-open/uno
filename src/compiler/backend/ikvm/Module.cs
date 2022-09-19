/*
  Copyright (C) 2009-2013 Jeroen Frijters

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
using IKVM.Reflection.Metadata;
using IKVM.Reflection.Reader;

namespace IKVM.Reflection
{
	public sealed class RawModule : IDisposable
	{
		private readonly ModuleReader module;
		private readonly bool isManifestModule;
		private bool imported;

		internal RawModule(ModuleReader module)
		{
			this.module = module;
			this.isManifestModule = module.Assembly != null;
		}

		public string Location
		{
			get { return module.FullyQualifiedName; }
		}

		public bool IsManifestModule
		{
			get { return isManifestModule; }
		}

		public Guid ModuleVersionId
		{
			get { return module.ModuleVersionId; }
		}

		public string ImageRuntimeVersion
		{
			get { return module.__ImageRuntimeVersion; }
		}

		public int MDStreamVersion
		{
			get { return module.MDStreamVersion; }
		}

		private void CheckManifestModule()
		{
			if (!IsManifestModule)
			{
				throw new BadImageFormatException("Module does not contain a manifest");
			}
		}

		public AssemblyName GetAssemblyName()
		{
			CheckManifestModule();
			return module.Assembly.GetName();
		}

		public AssemblyName[] GetReferencedAssemblies()
		{
			return module.__GetReferencedAssemblies();
		}

		public void Dispose()
		{
			if (!imported)
			{
				module.Dispose();
			}
		}

		internal AssemblyReader ToAssembly()
		{
			if (imported)
			{
				throw new InvalidOperationException();
			}
			imported = true;
			return (AssemblyReader)module.Assembly;
		}

		internal Module ToModule(Assembly assembly)
		{
			if (module.Assembly != null)
			{
				throw new InvalidOperationException();
			}
			imported = true;
			module.SetAssembly(assembly);
			return module;
		}
	}

	public abstract class Module : ICustomAttributeProvider
	{
		internal readonly Universe universe;
		internal readonly ModuleTable ModuleTable = new ModuleTable();
		internal readonly TypeRefTable TypeRef = new TypeRefTable();
		internal readonly TypeDefTable TypeDef = new TypeDefTable();
		internal readonly FieldPtrTable FieldPtr = new FieldPtrTable();
		internal readonly FieldTable Field = new FieldTable();
		internal readonly MemberRefTable MemberRef = new MemberRefTable();
		internal readonly ConstantTable Constant = new ConstantTable();
		internal readonly CustomAttributeTable CustomAttribute = new CustomAttributeTable();
		internal readonly FieldMarshalTable FieldMarshal = new FieldMarshalTable();
		internal readonly DeclSecurityTable DeclSecurity = new DeclSecurityTable();
		internal readonly ClassLayoutTable ClassLayout = new ClassLayoutTable();
		internal readonly FieldLayoutTable FieldLayout = new FieldLayoutTable();
		internal readonly ParamPtrTable ParamPtr = new ParamPtrTable();
		internal readonly ParamTable Param = new ParamTable();
		internal readonly InterfaceImplTable InterfaceImpl = new InterfaceImplTable();
		internal readonly StandAloneSigTable StandAloneSig = new StandAloneSigTable();
		internal readonly EventMapTable EventMap = new EventMapTable();
		internal readonly EventPtrTable EventPtr = new EventPtrTable();
		internal readonly EventTable Event = new EventTable();
		internal readonly PropertyMapTable PropertyMap = new PropertyMapTable();
		internal readonly PropertyPtrTable PropertyPtr = new PropertyPtrTable();
		internal readonly PropertyTable Property = new PropertyTable();
		internal readonly MethodSemanticsTable MethodSemantics = new MethodSemanticsTable();
		internal readonly MethodImplTable MethodImpl = new MethodImplTable();
		internal readonly ModuleRefTable ModuleRef = new ModuleRefTable();
		internal readonly TypeSpecTable TypeSpec = new TypeSpecTable();
		internal readonly ImplMapTable ImplMap = new ImplMapTable();
		internal readonly FieldRVATable FieldRVA = new FieldRVATable();
		internal readonly AssemblyTable AssemblyTable = new AssemblyTable();
		internal readonly AssemblyRefTable AssemblyRef = new AssemblyRefTable();
		internal readonly MethodPtrTable MethodPtr = new MethodPtrTable();
		internal readonly MethodDefTable MethodDef = new MethodDefTable();
		internal readonly NestedClassTable NestedClass = new NestedClassTable();
		internal readonly FileTable File = new FileTable();
		internal readonly ExportedTypeTable ExportedType = new ExportedTypeTable();
		internal readonly ManifestResourceTable ManifestResource = new ManifestResourceTable();
		internal readonly GenericParamTable GenericParam = new GenericParamTable();
		internal readonly MethodSpecTable MethodSpec = new MethodSpecTable();
		internal readonly GenericParamConstraintTable GenericParamConstraint = new GenericParamConstraintTable();
		internal readonly DocumentTable Document = new DocumentTable();
		internal readonly MethodDebugInformationTable MethodDebugInformation = new MethodDebugInformationTable();
		internal readonly LocalScopeTable LocalScope = new LocalScopeTable();
		internal readonly LocalVariableTable LocalVariable = new LocalVariableTable();
		internal readonly LocalConstantTable LocalConstant = new LocalConstantTable();
		internal readonly ImportScopeTable ImportScope = new ImportScopeTable();
		internal readonly StateMachineTable StateMachine = new StateMachineTable();
		internal readonly CustomDebugInformationTable CustomDebugInformation = new CustomDebugInformationTable();

		protected Module(Universe universe)
		{
			this.universe = universe;
		}

		internal Table[] GetTables()
		{
			Table[] tables = new Table[64];
			tables[ModuleTable.Index] = ModuleTable;
			tables[TypeRefTable.Index] = TypeRef;
			tables[TypeDefTable.Index] = TypeDef;
			tables[FieldPtrTable.Index] = FieldPtr;
			tables[FieldTable.Index] = Field;
			tables[MemberRefTable.Index] = MemberRef;
			tables[ConstantTable.Index] = Constant;
			tables[CustomAttributeTable.Index] = CustomAttribute;
			tables[FieldMarshalTable.Index] = FieldMarshal;
			tables[DeclSecurityTable.Index] = DeclSecurity;
			tables[ClassLayoutTable.Index] = ClassLayout;
			tables[FieldLayoutTable.Index] = FieldLayout;
			tables[ParamPtrTable.Index] = ParamPtr;
			tables[ParamTable.Index] = Param;
			tables[InterfaceImplTable.Index] = InterfaceImpl;
			tables[StandAloneSigTable.Index] = StandAloneSig;
			tables[EventMapTable.Index] = EventMap;
			tables[EventPtrTable.Index] = EventPtr;
			tables[EventTable.Index] = Event;
			tables[PropertyMapTable.Index] = PropertyMap;
			tables[PropertyPtrTable.Index] = PropertyPtr;
			tables[PropertyTable.Index] = Property;
			tables[MethodSemanticsTable.Index] = MethodSemantics;
			tables[MethodImplTable.Index] = MethodImpl;
			tables[ModuleRefTable.Index] = ModuleRef;
			tables[TypeSpecTable.Index] = TypeSpec;
			tables[ImplMapTable.Index] = ImplMap;
			tables[FieldRVATable.Index] = FieldRVA;
			tables[AssemblyTable.Index] = AssemblyTable;
			tables[AssemblyRefTable.Index] = AssemblyRef;
			tables[MethodPtrTable.Index] = MethodPtr;
			tables[MethodDefTable.Index] = MethodDef;
			tables[NestedClassTable.Index] = NestedClass;
			tables[FileTable.Index] = File;
			tables[ExportedTypeTable.Index] = ExportedType;
			tables[ManifestResourceTable.Index] = ManifestResource;
			tables[GenericParamTable.Index] = GenericParam;
			tables[MethodSpecTable.Index] = MethodSpec;
			tables[GenericParamConstraintTable.Index] = GenericParamConstraint;
			tables[DocumentTable.Index] = Document;
			tables[MethodDebugInformationTable.Index] = MethodDebugInformation;
			tables[LocalScopeTable.Index] = LocalScope;
			tables[LocalVariableTable.Index] = LocalVariable;
			tables[LocalConstantTable.Index] = LocalConstant;
			tables[ImportScopeTable.Index] = ImportScope;
			tables[StateMachineTable.Index] = StateMachine;
			tables[CustomDebugInformationTable.Index] = CustomDebugInformation;
			return tables;
		}

		public virtual void __GetDataDirectoryEntry(int index, out int rva, out int length)
		{
			throw new NotSupportedException();
		}

		public virtual long __RelativeVirtualAddressToFileOffset(int rva)
		{
			throw new NotSupportedException();
		}

		public bool __GetSectionInfo(int rva, out string name, out int characteristics)
		{
			int virtualAddress;
			int virtualSize;
			int pointerToRawData;
			int sizeOfRawData;
			return __GetSectionInfo(rva, out name, out characteristics, out virtualAddress, out virtualSize, out pointerToRawData, out sizeOfRawData);
		}

		public virtual bool __GetSectionInfo(int rva, out string name, out int characteristics, out int virtualAddress, out int virtualSize, out int pointerToRawData, out int sizeOfRawData)
		{
			throw new NotSupportedException();
		}

		public virtual int __ReadDataFromRVA(int rva, byte[] data, int offset, int length)
		{
			throw new NotSupportedException();
		}

		public virtual void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			throw new NotSupportedException();
		}

		public virtual int __Subsystem
		{
			get { throw new NotSupportedException(); }
		}

		public FieldInfo GetField(string name)
		{
			return GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public FieldInfo GetField(string name, BindingFlags bindingFlags)
		{
			return IsResource() ? null : GetModuleType().GetField(name, bindingFlags | BindingFlags.DeclaredOnly);
		}

		public FieldInfo[] GetFields()
		{
			return GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			return IsResource() ? Empty<FieldInfo>.Array : GetModuleType().GetFields(bindingFlags | BindingFlags.DeclaredOnly);
		}

		public MethodInfo GetMethod(string name)
		{
			return IsResource() ? null : GetModuleType().GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public MethodInfo GetMethod(string name, Type[] types)
		{
			return IsResource() ? null : GetModuleType().GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, types, null);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
		{
			return IsResource() ? null : GetModuleType().GetMethod(name, bindingAttr | BindingFlags.DeclaredOnly, binder, callConv, types, modifiers);
		}

		public MethodInfo[] GetMethods()
		{
			return GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			return IsResource() ? Empty<MethodInfo>.Array : GetModuleType().GetMethods(bindingFlags | BindingFlags.DeclaredOnly);
		}

		public ConstructorInfo __ModuleInitializer
		{
			get { return IsResource() ? null : GetModuleType().TypeInitializer; }
		}

		public virtual byte[] ResolveSignature(int metadataToken)
		{
			throw new NotSupportedException();
		}

		public virtual __StandAloneMethodSig __ResolveStandAloneMethodSig(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new NotSupportedException();
		}

		public virtual CustomModifiers __ResolveTypeSpecCustomModifiers(int typeSpecToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw new NotSupportedException();
		}

		public int MetadataToken
		{
			get { return IsResource() ? 0 : 1; }
		}

		public abstract int MDStreamVersion { get ;}
		public abstract Assembly Assembly { get; }
		public abstract string FullyQualifiedName { get; }
		public abstract string Name { get; }
		public abstract Guid ModuleVersionId { get; }
		public abstract MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);
		public abstract FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);
		public abstract MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);

		public abstract string ResolveString(int metadataToken);
		public abstract Type[] __ResolveOptionalParameterTypes(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments, out CustomModifiers[] customModifiers);
		public abstract string ScopeName { get; }

		internal abstract void GetTypesImpl(List<Type> list);

		internal abstract Type FindType(TypeName name);
		internal abstract Type FindTypeIgnoreCase(TypeName lowerCaseName);

		[Obsolete("Please use __ResolveOptionalParameterTypes(int, Type[], Type[], out CustomModifiers[]) instead.")]
		public Type[] __ResolveOptionalParameterTypes(int metadataToken)
		{
			CustomModifiers[] dummy;
			return __ResolveOptionalParameterTypes(metadataToken, null, null, out dummy);
		}

		public Type GetType(string className)
		{
			return GetType(className, false, false);
		}

		public Type GetType(string className, bool ignoreCase)
		{
			return GetType(className, false, ignoreCase);
		}

		public Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			TypeNameParser parser = TypeNameParser.Parse(className, throwOnError);
			if (parser.Error)
			{
				return null;
			}
			if (parser.AssemblyName != null)
			{
				if (throwOnError)
				{
					throw new ArgumentException("Type names passed to Module.GetType() must not specify an assembly.");
				}
				else
				{
					return null;
				}
			}
			TypeName typeName = TypeName.Split(TypeNameParser.Unescape(parser.FirstNamePart));
			Type type = ignoreCase
				? FindTypeIgnoreCase(typeName.ToLowerInvariant())
				: FindType(typeName);
			if (type == null && __IsMissing)
			{
				throw new MissingModuleException((MissingModule)this);
			}
			return parser.Expand(type, this, throwOnError, className, false, ignoreCase);
		}

		public Type[] GetTypes()
		{
			List<Type> list = new List<Type>();
			GetTypesImpl(list);
			return list.ToArray();
		}

		public Type[] FindTypes(TypeFilter filter, object filterCriteria)
		{
			List<Type> list = new List<Type>();
			foreach (Type type in GetTypes())
			{
				if (filter(type, filterCriteria))
				{
					list.Add(type);
				}
			}
			return list.ToArray();
		}

		public virtual bool IsResource()
		{
			return false;
		}

		public Type ResolveType(int metadataToken)
		{
			return ResolveType(metadataToken, null, null);
		}

		internal sealed class GenericContext : IGenericContext
		{
			private readonly Type[] genericTypeArguments;
			private readonly Type[] genericMethodArguments;

			internal GenericContext(Type[] genericTypeArguments, Type[] genericMethodArguments)
			{
				this.genericTypeArguments = genericTypeArguments;
				this.genericMethodArguments = genericMethodArguments;
			}

			public Type GetGenericTypeArgument(int index)
			{
				return genericTypeArguments[index];
			}

			public Type GetGenericMethodArgument(int index)
			{
				return genericMethodArguments[index];
			}
		}

		public Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			if ((metadataToken >> 24) == TypeSpecTable.Index)
			{
				return ResolveType(metadataToken, new GenericContext(genericTypeArguments, genericMethodArguments));
			}
			else
			{
				return ResolveType(metadataToken, null);
			}
		}

		internal abstract Type ResolveType(int metadataToken, IGenericContext context);

		public MethodBase ResolveMethod(int metadataToken)
		{
			return ResolveMethod(metadataToken, null, null);
		}

		public FieldInfo ResolveField(int metadataToken)
		{
			return ResolveField(metadataToken, null, null);
		}

		public MemberInfo ResolveMember(int metadataToken)
		{
			return ResolveMember(metadataToken, null, null);
		}

		public bool IsDefined(Type attributeType, bool inherit)
		{
			return CustomAttributeData.__GetCustomAttributes(this, attributeType, inherit).Count != 0;
		}

		public IList<CustomAttributeData> __GetCustomAttributes(Type attributeType, bool inherit)
		{
			return CustomAttributeData.__GetCustomAttributes(this, attributeType, inherit);
		}

		public IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributes(this);
		}

		public IEnumerable<CustomAttributeData> CustomAttributes
		{
			get { return GetCustomAttributesData(); }
		}

		public virtual IList<CustomAttributeData> __GetPlaceholderAssemblyCustomAttributes(bool multiple, bool security)
		{
			return Empty<CustomAttributeData>.Array;
		}

		public abstract AssemblyName[] __GetReferencedAssemblies();

		public virtual void __ResolveReferencedAssemblies(Assembly[] assemblies)
		{
			throw new NotSupportedException();
		}

		public abstract string[] __GetReferencedModules();

		public abstract Type[] __GetReferencedTypes();

		public abstract Type[] __GetExportedTypes();

		public virtual bool __IsMissing
		{
			get { return false; }
		}

		public long __ImageBase
		{
			get { return GetImageBaseImpl(); }
		}

		protected abstract long GetImageBaseImpl();

		public long __StackReserve
		{
			get { return GetStackReserveImpl(); }
		}

		protected abstract long GetStackReserveImpl();

		public int __FileAlignment
		{
			get { return GetFileAlignmentImpl(); }
		}

		protected abstract int GetFileAlignmentImpl();

		public DllCharacteristics __DllCharacteristics
		{
			get { return GetDllCharacteristicsImpl(); }
		}

		protected abstract DllCharacteristics GetDllCharacteristicsImpl();

		public virtual byte[] __ModuleHash
		{
			get { throw new NotSupportedException(); }
		}

		public virtual int __EntryPointRVA
		{
			get { throw new NotSupportedException(); }
		}

		public virtual int __EntryPointToken
		{
			get { throw new NotSupportedException(); }
		}

		public virtual string __ImageRuntimeVersion
		{
			get { throw new NotSupportedException(); }
		}

		public virtual bool __IsMetadataOnly
		{
			get { throw new NotSupportedException(); }
		}

		public IEnumerable<CustomAttributeData> __EnumerateCustomAttributeTable()
		{
			List<CustomAttributeData> list = new List<CustomAttributeData>(CustomAttribute.RowCount);
			for (int i = 0; i < CustomAttribute.RowCount; i++)
			{
				list.Add(new CustomAttributeData(this, i));
			}
			return list;
		}

		[Obsolete]
		public List<CustomAttributeData> __GetCustomAttributesFor(int token)
		{
			return CustomAttributeData.GetCustomAttributesImpl(new List<CustomAttributeData>(), this, token, null);
		}

		public bool __TryGetImplMap(int token, out ImplMapFlags mappingFlags, out string importName, out string importScope)
		{
			foreach (int i in ImplMap.Filter(token))
			{
				mappingFlags = (ImplMapFlags)(ushort)ImplMap.records[i].MappingFlags;
				importName = GetString(ImplMap.records[i].ImportName);
				importScope = GetString(ModuleRef.records[(ImplMap.records[i].ImportScope & 0xFFFFFF) - 1]);
				return true;
			}
			mappingFlags = 0;
			importName = null;
			importScope = null;
			return false;
		}

#if !NO_AUTHENTICODE
		public virtual System.Security.Cryptography.X509Certificates.X509Certificate GetSignerCertificate()
		{
			return null;
		}
#endif // !NO_AUTHENTICODE

		internal abstract Type GetModuleType();

		internal abstract ByteReader GetBlob(int blobIndex);

		internal abstract Guid GetGuid(int guidIndex);

		internal IList<CustomAttributeData> GetDeclarativeSecurity(int metadataToken)
		{
			List<CustomAttributeData> list = new List<CustomAttributeData>();
			foreach (int i in DeclSecurity.Filter(metadataToken))
			{
				CustomAttributeData.ReadDeclarativeSecurity(this, i, list);
			}
			return list;
		}

		internal virtual void Dispose()
		{
		}

		internal virtual void ExportTypes(int fileToken, IKVM.Reflection.Emit.ModuleBuilder manifestModule)
		{
		}

		internal virtual string GetString(int index)
		{
			throw new NotSupportedException();
		}
	}

	abstract class NonPEModule : Module
	{
		protected NonPEModule(Universe universe)
			: base(universe)
		{
		}

		protected virtual Exception InvalidOperationException()
		{
			return new InvalidOperationException();
		}

		protected virtual Exception NotSupportedException()
		{
			return new NotSupportedException();
		}

		protected virtual Exception ArgumentOutOfRangeException()
		{
			return new ArgumentOutOfRangeException();
		}

		internal sealed override Type GetModuleType()
		{
			throw InvalidOperationException();
		}

		internal sealed override ByteReader GetBlob(int blobIndex)
		{
			throw InvalidOperationException();
		}

		internal sealed override Guid GetGuid(int guidIndex)
		{
			throw InvalidOperationException();
		}

		public sealed override AssemblyName[] __GetReferencedAssemblies()
		{
			throw NotSupportedException();
		}

		public sealed override string[] __GetReferencedModules()
		{
			throw NotSupportedException();
		}

		public override Type[] __GetReferencedTypes()
		{
			throw NotSupportedException();
		}

		public override Type[] __GetExportedTypes()
		{
			throw NotSupportedException();
		}

		protected sealed override long GetImageBaseImpl()
		{
			throw NotSupportedException();
		}

		protected sealed override long GetStackReserveImpl()
		{
			throw NotSupportedException();
		}

		protected sealed override int GetFileAlignmentImpl()
		{
			throw NotSupportedException();
		}

		protected override DllCharacteristics GetDllCharacteristicsImpl()
		{
			throw NotSupportedException();
		}

		internal sealed override Type ResolveType(int metadataToken, IGenericContext context)
		{
			throw ArgumentOutOfRangeException();
		}

		public sealed override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw ArgumentOutOfRangeException();
		}

		public sealed override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw ArgumentOutOfRangeException();
		}

		public sealed override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw ArgumentOutOfRangeException();
		}

		public sealed override string ResolveString(int metadataToken)
		{
			throw ArgumentOutOfRangeException();
		}

		public sealed override Type[] __ResolveOptionalParameterTypes(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments, out CustomModifiers[] customModifiers)
		{
			throw ArgumentOutOfRangeException();
		}
	}

	public delegate bool TypeFilter(Type m, object filterCriteria);
	public delegate bool MemberFilter(MemberInfo m, object filterCriteria);
}
