/*
  Copyright (C) 2008-2012 Jeroen Frijters

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
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
#if !NO_SYMBOL_WRITER
using System.Diagnostics.SymbolStore;
#endif
using IKVM.Reflection.Metadata;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Emit
{
	public sealed class MethodBuilder : MethodInfo
	{
		private readonly TypeBuilder typeBuilder;
		private readonly string name;
		private readonly int pseudoToken;
		private int nameIndex;
		private int signature;
		private Type returnType;
		private Type[] parameterTypes;
		private PackedCustomModifiers customModifiers;
		private MethodAttributes attributes;
		private MethodImplAttributes implFlags;
		private ILGenerator ilgen;
		private int rva = -1;
		private CallingConventions callingConvention;
		private List<ParameterBuilder> parameters;
		private GenericTypeParameterBuilder[] gtpb;
		private List<CustomAttributeBuilder> declarativeSecurity;
		private MethodSignature methodSignature;
		private bool initLocals = true;

		internal MethodBuilder(TypeBuilder typeBuilder, string name, MethodAttributes attributes, CallingConventions callingConvention)
		{
			this.typeBuilder = typeBuilder;
			this.name = name;
			this.pseudoToken = typeBuilder.ModuleBuilder.AllocPseudoToken();
			this.attributes = attributes;
			if ((attributes & MethodAttributes.Static) == 0)
			{
				callingConvention |= CallingConventions.HasThis;
			}
			this.callingConvention = callingConvention;
		}

		public ILGenerator GetILGenerator()
		{
			return GetILGenerator(16);
		}

		public ILGenerator GetILGenerator(int streamSize)
		{
			if (rva != -1)
			{
				throw new InvalidOperationException();
			}
			if (ilgen == null)
			{
				ilgen = new ILGenerator(typeBuilder.ModuleBuilder, streamSize);
			}
			return ilgen;
		}

		public void __ReleaseILGenerator()
		{
			if (ilgen != null)
			{
#if !NO_SYMBOL_WRITER
				if (this.ModuleBuilder.symbolWriter != null)
				{
					this.ModuleBuilder.symbolWriter.OpenMethod(new SymbolToken(-pseudoToken | 0x06000000), this);
				}
#endif
				rva = ilgen.WriteBody(initLocals);
#if !NO_SYMBOL_WRITER
				if (this.ModuleBuilder.symbolWriter != null)
				{
					this.ModuleBuilder.symbolWriter.CloseMethod();
				}
#endif
				ilgen = null;
			}
		}

		public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
		{
			SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		private void SetDllImportPseudoCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			CallingConvention? callingConvention = customBuilder.GetFieldValue<CallingConvention>("CallingConvention");
			CharSet? charSet = customBuilder.GetFieldValue<CharSet>("CharSet");
			SetDllImportPseudoCustomAttribute((string)customBuilder.GetConstructorArgument(0),
				(string)customBuilder.GetFieldValue("EntryPoint"),
				callingConvention,
				charSet,
				(bool?)customBuilder.GetFieldValue("BestFitMapping"),
				(bool?)customBuilder.GetFieldValue("ThrowOnUnmappableChar"),
				(bool?)customBuilder.GetFieldValue("SetLastError"),
				(bool?)customBuilder.GetFieldValue("PreserveSig"),
				(bool?)customBuilder.GetFieldValue("ExactSpelling"));
		}

		internal void SetDllImportPseudoCustomAttribute(string dllName, string entryName, CallingConvention? nativeCallConv, CharSet? nativeCharSet,
			bool? bestFitMapping, bool? throwOnUnmappableChar, bool? setLastError, bool? preserveSig, bool? exactSpelling)
		{
			const short NoMangle = 0x0001;
			const short CharSetMask = 0x0006;
			const short CharSetNotSpec = 0x0000;
			const short CharSetAnsi = 0x0002;
			const short CharSetUnicode = 0x0004;
			const short CharSetAuto = 0x0006;
			const short SupportsLastError = 0x0040;
			const short CallConvMask = 0x0700;
			const short CallConvWinapi = 0x0100;
			const short CallConvCdecl = 0x0200;
			const short CallConvStdcall = 0x0300;
			const short CallConvThiscall = 0x0400;
			const short CallConvFastcall = 0x0500;
			// non-standard flags
			const short BestFitOn = 0x0010;
			const short BestFitOff = 0x0020;
			const short CharMapErrorOn = 0x1000;
			const short CharMapErrorOff = 0x2000;
			short flags = CharSetNotSpec | CallConvWinapi;
			if (bestFitMapping.HasValue)
			{
				flags |= bestFitMapping.Value ? BestFitOn : BestFitOff;
			}
			if (throwOnUnmappableChar.HasValue)
			{
				flags |= throwOnUnmappableChar.Value ? CharMapErrorOn : CharMapErrorOff;
			}
			if (nativeCallConv.HasValue)
			{
				flags &= ~CallConvMask;
				switch (nativeCallConv.Value)
				{
					case System.Runtime.InteropServices.CallingConvention.Cdecl:
						flags |= CallConvCdecl;
						break;
					case System.Runtime.InteropServices.CallingConvention.FastCall:
						flags |= CallConvFastcall;
						break;
					case System.Runtime.InteropServices.CallingConvention.StdCall:
						flags |= CallConvStdcall;
						break;
					case System.Runtime.InteropServices.CallingConvention.ThisCall:
						flags |= CallConvThiscall;
						break;
					case System.Runtime.InteropServices.CallingConvention.Winapi:
						flags |= CallConvWinapi;
						break;
				}
			}
			if (nativeCharSet.HasValue)
			{
				flags &= ~CharSetMask;
				switch (nativeCharSet.Value)
				{
					case CharSet.Ansi:
					case CharSet.None:
						flags |= CharSetAnsi;
						break;
					case CharSet.Auto:
						flags |= CharSetAuto;
						break;
					case CharSet.Unicode:
						flags |= CharSetUnicode;
						break;
				}
			}
			if (exactSpelling.HasValue && exactSpelling.Value)
			{
				flags |= NoMangle;
			}
			if (!preserveSig.HasValue || preserveSig.Value)
			{
				implFlags |= MethodImplAttributes.PreserveSig;
			}
			if (setLastError.HasValue && setLastError.Value)
			{
				flags |= SupportsLastError;
			}
			ImplMapTable.Record rec = new ImplMapTable.Record();
			rec.MappingFlags = flags;
			rec.MemberForwarded = pseudoToken;
			rec.ImportName = this.ModuleBuilder.Strings.Add(entryName ?? name);
			rec.ImportScope = this.ModuleBuilder.ModuleRef.FindOrAddRecord(dllName == null ? 0 : this.ModuleBuilder.Strings.Add(dllName));
			this.ModuleBuilder.ImplMap.AddRecord(rec);
		}

		private void SetMethodImplAttribute(CustomAttributeBuilder customBuilder)
		{
			MethodImplOptions opt;
			switch (customBuilder.Constructor.ParameterCount)
			{
				case 0:
					opt = 0;
					break;
				case 1:
					{
						object val = customBuilder.GetConstructorArgument(0);
						if (val is short)
						{
							opt = (MethodImplOptions)(short)val;
						}
						else if (val is int)
						{
							opt = (MethodImplOptions)(int)val;
						}
						else
						{
							opt = (MethodImplOptions)val;
						}
						break;
					}
				default:
					throw new NotSupportedException();
			}
			MethodCodeType? type = customBuilder.GetFieldValue<MethodCodeType>("MethodCodeType");
			implFlags = (MethodImplAttributes)opt;
			if (type.HasValue)
			{
				implFlags |= (MethodImplAttributes)type;
			}
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			switch (customBuilder.KnownCA)
			{
				case KnownCA.DllImportAttribute:
					SetDllImportPseudoCustomAttribute(customBuilder.DecodeBlob(this.Module.Assembly));
					attributes |= MethodAttributes.PinvokeImpl;
					break;
				case KnownCA.MethodImplAttribute:
					SetMethodImplAttribute(customBuilder.DecodeBlob(this.Module.Assembly));
					break;
				case KnownCA.PreserveSigAttribute:
					implFlags |= MethodImplAttributes.PreserveSig;
					break;
				case KnownCA.SpecialNameAttribute:
					attributes |= MethodAttributes.SpecialName;
					break;
				case KnownCA.SuppressUnmanagedCodeSecurityAttribute:
					attributes |= MethodAttributes.HasSecurity;
					goto default;
				default:
					this.ModuleBuilder.SetCustomAttribute(pseudoToken, customBuilder);
					break;
			}
		}

		public void __AddDeclarativeSecurity(CustomAttributeBuilder customBuilder)
		{
			attributes |= MethodAttributes.HasSecurity;
			if (declarativeSecurity == null)
			{
				declarativeSecurity = new List<CustomAttributeBuilder>();
			}
			declarativeSecurity.Add(customBuilder);
		}

#if !CORECLR
		public void AddDeclarativeSecurity(System.Security.Permissions.SecurityAction securityAction, System.Security.PermissionSet permissionSet)
		{
			this.ModuleBuilder.AddDeclarativeSecurity(pseudoToken, securityAction, permissionSet);
			this.attributes |= MethodAttributes.HasSecurity;
		}
#endif

		public void SetImplementationFlags(MethodImplAttributes attributes)
		{
			implFlags = attributes;
		}

		public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string strParamName)
		{
			if (parameters == null)
			{
				parameters = new List<ParameterBuilder>();
			}
			this.ModuleBuilder.Param.AddVirtualRecord();
			ParameterBuilder pb = new ParameterBuilder(this.ModuleBuilder, position, attributes, strParamName);
			if (parameters.Count == 0 || position >= parameters[parameters.Count - 1].Position)
			{
				parameters.Add(pb);
			}
			else
			{
				for (int i = 0; i < parameters.Count; i++)
				{
					if (parameters[i].Position > position)
					{
						parameters.Insert(i, pb);
						break;
					}
				}
			}
			return pb;
		}

		private void CheckSig()
		{
			if (methodSignature != null)
			{
				throw new InvalidOperationException("The method signature can not be modified after it has been used.");
			}
		}

		public void SetParameters(params Type[] parameterTypes)
		{
			CheckSig();
			this.parameterTypes = Util.Copy(parameterTypes);
		}

		public void SetReturnType(Type returnType)
		{
			CheckSig();
			this.returnType = returnType ?? this.Module.universe.System_Void;
		}

		public void SetSignature(Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
		{
			SetSignature(returnType, parameterTypes, PackedCustomModifiers.CreateFromExternal(returnTypeOptionalCustomModifiers, returnTypeRequiredCustomModifiers,
				parameterTypeOptionalCustomModifiers, parameterTypeRequiredCustomModifiers, Util.NullSafeLength(parameterTypes)));
		}

		public void __SetSignature(Type returnType, CustomModifiers returnTypeCustomModifiers, Type[] parameterTypes, CustomModifiers[] parameterTypeCustomModifiers)
		{
			SetSignature(returnType, parameterTypes, PackedCustomModifiers.CreateFromExternal(returnTypeCustomModifiers, parameterTypeCustomModifiers, Util.NullSafeLength(parameterTypes)));
		}

		private void SetSignature(Type returnType, Type[] parameterTypes, PackedCustomModifiers customModifiers)
		{
			CheckSig();
			this.returnType = returnType ?? this.Module.universe.System_Void;
			this.parameterTypes = Util.Copy(parameterTypes);
			this.customModifiers = customModifiers;
		}

		public GenericTypeParameterBuilder[] DefineGenericParameters(params string[] names)
		{
			CheckSig();
			if (gtpb != null)
			{
				throw new InvalidOperationException("Generic parameters already defined.");
			}
			gtpb = new GenericTypeParameterBuilder[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				gtpb[i] = new GenericTypeParameterBuilder(names[i], this, i);
			}
			return (GenericTypeParameterBuilder[])gtpb.Clone();
		}

		public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
		{
			return new GenericMethodInstance(typeBuilder, this, typeArguments);
		}

		public override MethodInfo GetGenericMethodDefinition()
		{
			if (gtpb == null)
			{
				throw new InvalidOperationException();
			}
			return this;
		}

		public override Type[] GetGenericArguments()
		{
			return Util.Copy(gtpb);
		}

		internal override Type GetGenericMethodArgument(int index)
		{
			return gtpb[index];
		}

		internal override int GetGenericMethodArgumentCount()
		{
			return gtpb == null ? 0 : gtpb.Length;
		}

		public override Type ReturnType
		{
			get { return returnType; }
		}

		public override ParameterInfo ReturnParameter
		{
			get { return new ParameterInfoImpl(this, -1); }
		}

		public override MethodAttributes Attributes
		{
			get { return attributes; }
		}

		public void __SetAttributes(MethodAttributes attributes)
		{
			this.attributes = attributes;
		}

		public void __SetCallingConvention(CallingConventions callingConvention)
		{
			this.callingConvention = callingConvention;
			this.methodSignature = null;
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return implFlags;
		}

		private sealed class ParameterInfoImpl : ParameterInfo
		{
			private readonly MethodBuilder method;
			private readonly int parameter;

			internal ParameterInfoImpl(MethodBuilder method, int parameter)
			{
				this.method = method;
				this.parameter = parameter;
			}

			private ParameterBuilder ParameterBuilder
			{
				get
				{
					if (method.parameters != null)
					{
						foreach (ParameterBuilder pb in method.parameters)
						{
							// ParameterBuilder.Position is 1-based
							if (pb.Position - 1 == parameter)
							{
								return pb;
							}
						}
					}
					return null;
				}
			}

			public override string Name
			{
				get
				{
					ParameterBuilder pb = this.ParameterBuilder;
					return pb != null ? pb.Name : null;
				}
			}

			public override Type ParameterType
			{
				get { return parameter == -1 ? method.returnType : method.parameterTypes[parameter]; }
			}

			public override ParameterAttributes Attributes
			{
				get
				{
					ParameterBuilder pb = this.ParameterBuilder;
					return pb != null ? (ParameterAttributes)pb.Attributes : ParameterAttributes.None;
				}
			}

			public override int Position
			{
				get { return parameter; }
			}

			public override object RawDefaultValue
			{
				get
				{
					ParameterBuilder pb = this.ParameterBuilder;
					if (pb != null && (pb.Attributes & (int)ParameterAttributes.HasDefault) != 0)
					{
						return method.ModuleBuilder.Constant.GetRawConstantValue(method.ModuleBuilder, pb.PseudoToken);
					}
					if (pb != null && (pb.Attributes & (int)ParameterAttributes.Optional) != 0)
					{
						return Missing.Value;
					}
					return null;
				}
			}

			public override CustomModifiers __GetCustomModifiers()
			{
				return method.customModifiers.GetParameterCustomModifiers(parameter);
			}

			public override bool __TryGetFieldMarshal(out FieldMarshal fieldMarshal)
			{
				fieldMarshal = new FieldMarshal();
				return false;
			}

			public override MemberInfo Member
			{
				get { return method; }
			}

			public override int MetadataToken
			{
				get
				{
					ParameterBuilder pb = this.ParameterBuilder;
					return pb != null ? pb.PseudoToken : 0x08000000;
				}
			}

			internal override Module Module
			{
				get { return method.Module; }
			}
		}

		public override ParameterInfo[] GetParameters()
		{
			ParameterInfo[] parameters = new ParameterInfo[parameterTypes.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				parameters[i] = new ParameterInfoImpl(this, i);
			}
			return parameters;
		}

		internal override int ParameterCount
		{
			get { return parameterTypes.Length; }
		}

		public override Type DeclaringType
		{
			get { return typeBuilder.IsModulePseudoType ? null : typeBuilder; }
		}

		public override string Name
		{
			get { return name; }
		}

		public override CallingConventions CallingConvention
		{
			get { return callingConvention; }
		}

		public override int MetadataToken
		{
			get { return pseudoToken; }
		}

		public override bool IsGenericMethod
		{
			get { return gtpb != null; }
		}

		public override bool IsGenericMethodDefinition
		{
			get { return gtpb != null; }
		}

		public override Module Module
		{
			get { return typeBuilder.Module; }
		}

		public Module GetModule()
		{
			return typeBuilder.Module;
		}

		public MethodToken GetToken()
		{
			return new MethodToken(pseudoToken);
		}

		public override MethodBody GetMethodBody()
		{
			throw new NotSupportedException();
		}

		public override int __MethodRVA
		{
			get { throw new NotImplementedException(); }
		}

		public bool InitLocals
		{
			get { return initLocals; }
			set { initLocals = value; }
		}

		public void __AddUnmanagedExport(string name, int ordinal)
		{
			this.ModuleBuilder.AddUnmanagedExport(name, ordinal, this, new RelativeVirtualAddress(0xFFFFFFFF));
		}

		public void CreateMethodBody(byte[] il, int count)
		{
			if (il == null)
			{
				throw new NotSupportedException();
			}
			if (il.Length != count)
			{
				Array.Resize(ref il, count);
			}
			SetMethodBody(il, 16, null, null, null);
		}

		public void SetMethodBody(byte[] il, int maxStack, byte[] localSignature, IEnumerable<ExceptionHandler> exceptionHandlers, IEnumerable<int> tokenFixups)
		{
			ByteBuffer bb = this.ModuleBuilder.methodBodies;

			if (localSignature == null && exceptionHandlers == null && maxStack <= 8 && il.Length < 64)
			{
				rva = bb.Position;
				ILGenerator.WriteTinyHeader(bb, il.Length);
			}
			else
			{
				// fat headers require 4-byte alignment
				bb.Align(4);
				rva = bb.Position;
				ILGenerator.WriteFatHeader(bb, initLocals, exceptionHandlers != null, (ushort)maxStack, il.Length,
					localSignature == null ? 0 : this.ModuleBuilder.GetSignatureToken(localSignature, localSignature.Length).Token);
			}

			if (tokenFixups != null)
			{
				ILGenerator.AddTokenFixups(bb.Position, this.ModuleBuilder.tokenFixupOffsets, tokenFixups);
			}
			bb.Write(il);

			if (exceptionHandlers != null)
			{
				List<ILGenerator.ExceptionBlock> exceptions = new List<ILGenerator.ExceptionBlock>();
				foreach (ExceptionHandler block in exceptionHandlers)
				{
					exceptions.Add(new ILGenerator.ExceptionBlock(block));
				}
				ILGenerator.WriteExceptionHandlers(bb, exceptions);
			}
		}

		internal void Bake()
		{
			this.nameIndex = this.ModuleBuilder.Strings.Add(name);
			this.signature = this.ModuleBuilder.GetSignatureBlobIndex(this.MethodSignature);

			__ReleaseILGenerator();

			if (declarativeSecurity != null)
			{
				this.ModuleBuilder.AddDeclarativeSecurity(pseudoToken, declarativeSecurity);
			}
		}

		internal ModuleBuilder ModuleBuilder
		{
			get { return typeBuilder.ModuleBuilder; }
		}

		internal void WriteMethodDefRecord(int baseRVA, MetadataWriter mw, ref int paramList)
		{
			if (rva != -1)
			{
				mw.Write(rva + baseRVA);
			}
			else
			{
				mw.Write(0);
			}
			mw.Write((short)implFlags);
			mw.Write((short)attributes);
			mw.WriteStringIndex(nameIndex);
			mw.WriteBlobIndex(signature);
			mw.WriteParam(paramList);
			if (parameters != null)
			{
				paramList += parameters.Count;
			}
		}

		internal void WriteParamRecords(MetadataWriter mw)
		{
			if (parameters != null)
			{
				foreach (ParameterBuilder pb in parameters)
				{
					pb.WriteParamRecord(mw);
				}
			}
		}

		internal void FixupToken(int token, ref int parameterToken)
		{
			typeBuilder.ModuleBuilder.RegisterTokenFixup(this.pseudoToken, token);
			if (parameters != null)
			{
				foreach (ParameterBuilder pb in parameters)
				{
					pb.FixupToken(parameterToken++);
				}
			}
		}

		internal override MethodSignature MethodSignature
		{
			get
			{
				if (methodSignature == null)
				{
					methodSignature = MethodSignature.MakeFromBuilder(returnType ?? typeBuilder.Universe.System_Void, parameterTypes ?? Type.EmptyTypes,
						customModifiers, callingConvention, gtpb == null ? 0 : gtpb.Length);
				}
				return methodSignature;
			}
		}

		internal override int ImportTo(ModuleBuilder other)
		{
			return other.ImportMethodOrField(typeBuilder, name, this.MethodSignature);
		}

		internal void CheckBaked()
		{
			typeBuilder.CheckBaked();
		}

		internal override int GetCurrentToken()
		{
			if (typeBuilder.ModuleBuilder.IsSaved)
			{
				return typeBuilder.ModuleBuilder.ResolvePseudoToken(pseudoToken);
			}
			else
			{
				return pseudoToken;
			}
		}

		internal override bool IsBaked
		{
			get { return typeBuilder.IsBaked; }
		}
	}
}
