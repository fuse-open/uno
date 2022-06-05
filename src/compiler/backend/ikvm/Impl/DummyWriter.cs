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
#if NET6_0_OR_GREATER
using System;
using System.Diagnostics.SymbolStore;

namespace IKVM.Reflection.Impl
{
    class DummyWriter : ISymbolWriterImpl
    {
        public void Initialize(IntPtr emitter, string filename, bool fFullBuild)
        {
        }

        public ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType)
        {
            return new DummyDocumentWriter();
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

        public bool IsDeterministic
        {
            get { return false; }
        }
    }

    class DummyDocumentWriter : ISymbolDocumentWriter
    {
        public void SetSource(byte[] source)
        {
        }

        public void SetCheckSum(Guid algorithmId, byte[] checkSum)
        {
        }
    }
}
#endif // NET6_0_OR_GREATER
