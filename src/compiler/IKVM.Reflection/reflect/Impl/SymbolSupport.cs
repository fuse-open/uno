/*
  Copyright (C) 2008, 2009 Jeroen Frijters

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
using System.Runtime.InteropServices;
#if !NO_SYMBOL_WRITER
using System.Diagnostics.SymbolStore;
#endif
using IKVM.Reflection.Emit;

namespace IKVM.Reflection.Impl
{
	[StructLayout(LayoutKind.Sequential)]
	struct IMAGE_DEBUG_DIRECTORY
	{
		public uint Characteristics;
		public uint TimeDateStamp;
		public ushort MajorVersion;
		public ushort MinorVersion;
		public uint Type;
		public uint SizeOfData;
		public uint AddressOfRawData;
		public uint PointerToRawData;
	}

#if NO_SYMBOL_WRITER
	struct SymbolToken
	{
		internal SymbolToken(int value) { }
	}

	interface ISymbolWriterImpl
	{
		byte[] GetDebugInfo(ref IMAGE_DEBUG_DIRECTORY idd);
		void RemapToken(int oldToken, int newToken);
		void DefineLocalVariable2(string name, FieldAttributes attributes, int signature, int addrKind, int addr1, int addr2, int addr3, int startOffset, int endOffset);
		void OpenMethod(SymbolToken symbolToken, MethodBase mb);
		bool IsDeterministic { get; }
	}
#else
	interface ISymbolWriterImpl : ISymbolWriter
	{
		byte[] GetDebugInfo(ref IMAGE_DEBUG_DIRECTORY idd);
		void RemapToken(int oldToken, int newToken);
		void DefineLocalVariable2(string name, FieldAttributes attributes, int signature, SymAddressKind addrKind, int addr1, int addr2, int addr3, int startOffset, int endOffset);
		void OpenMethod(SymbolToken symbolToken, MethodBase mb);
		bool IsDeterministic { get; }
	}
#endif

	static class SymbolSupport
	{
		internal static ISymbolWriterImpl CreateSymbolWriterFor(ModuleBuilder moduleBuilder)
		{
#if NO_SYMBOL_WRITER
			throw new NotSupportedException("IKVM.Reflection compiled with NO_SYMBOL_WRITER does not support writing debugging symbols.");
#else
			if (Universe.MonoRuntime)
			{
#if MONO
				return new MdbWriter(moduleBuilder);
#else
				return new DummySymbolWriter();
#endif
			}
			else
			{
				return new PdbWriter(moduleBuilder);
			}
#endif
		}

		internal static byte[] GetDebugInfo(ISymbolWriterImpl writer, ref IMAGE_DEBUG_DIRECTORY idd)
		{
			return writer.GetDebugInfo(ref idd);
		}

		internal static void RemapToken(ISymbolWriterImpl writer, int oldToken, int newToken)
		{
			writer.RemapToken(oldToken, newToken);
		}
	}

    class DummySymbolDocumentWriter : ISymbolDocumentWriter
    {
        public void SetSource(byte[] source)
        {
        }

        public void SetCheckSum(Guid algorithmId, byte[] checkSum)
        {
        }
    }

    class DummySymbolWriter : ISymbolWriterImpl
    {
        public void Initialize(IntPtr emitter, string filename, bool fFullBuild)
        {
        }

        public ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType)
        {
            return new DummySymbolDocumentWriter();
        }

        public void SetUserEntryPoint(SymbolToken entryMethod)
        {
        }

        public void OpenMethod(SymbolToken method)
        {
        }

        public void CloseMethod()
        {
        }

        public void DefineSequencePoints(ISymbolDocumentWriter document, int[] offsets, int[] lines, int[] columns, int[] endLines,
            int[] endColumns)
        {
        }

        public int OpenScope(int startOffset)
        {
            return 0;
        }

        public void CloseScope(int endOffset)
        {
        }

        public void SetScopeRange(int scopeID, int startOffset, int endOffset)
        {
        }

        public void DefineLocalVariable(string name, System.Reflection.FieldAttributes attributes, byte[] signature, SymAddressKind addrKind, int addr1,
            int addr2, int addr3, int startOffset, int endOffset)
        {
        }

        public void DefineParameter(string name, System.Reflection.ParameterAttributes attributes, int sequence, SymAddressKind addrKind, int addr1,
            int addr2, int addr3)
        {
        }

        public void DefineField(SymbolToken parent, string name, System.Reflection.FieldAttributes attributes, byte[] signature, SymAddressKind addrKind,
            int addr1, int addr2, int addr3)
        {
        }

        public void DefineGlobalVariable(string name, System.Reflection.FieldAttributes attributes, byte[] signature, SymAddressKind addrKind, int addr1,
            int addr2, int addr3)
        {
        }

        public void Close()
        {
        }

        public void SetSymAttribute(SymbolToken parent, string name, byte[] data)
        {
        }

        public void OpenNamespace(string name)
        {
        }

        public void CloseNamespace()
        {
        }

        public void UsingNamespace(string fullName)
        {
        }

        public void SetMethodSourceRange(ISymbolDocumentWriter startDoc, int startLine, int startColumn, ISymbolDocumentWriter endDoc,
            int endLine, int endColumn)
        {
        }

        public void SetUnderlyingWriter(IntPtr underlyingWriter)
        {
        }

        public byte[] GetDebugInfo(ref IMAGE_DEBUG_DIRECTORY idd)
        {
            return new byte[0];
        }

        public void RemapToken(int oldToken, int newToken)
        {
        }

        public void DefineLocalVariable2(string name, FieldAttributes attributes, int signature, SymAddressKind addrKind, int addr1,
            int addr2, int addr3, int startOffset, int endOffset)
        {
        }

        public void OpenMethod(SymbolToken symbolToken, MethodBase mb)
        {
        }

        public bool IsDeterministic => true;
    }
}
