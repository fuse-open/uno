using System;
using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API.Domain.Serialization;
using Uno.IO;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlDocument : UxlEntity
    {
        public const uint Magic = 0x044C5855;

        public readonly SourcePackage Package;
        public readonly UxlBackendType Backend;
        public readonly SourceValue? Condition;

        public readonly List<SourceValue> Usings = new List<SourceValue>();
        public readonly List<UxlDefine> Defines = new List<UxlDefine>();
        public readonly List<UxlDeclare> Declarations = new List<UxlDeclare>();
        public readonly List<UxlDeprecate> Deprecations = new List<UxlDeprecate>();
        public readonly List<UxlTemplate> Templates = new List<UxlTemplate>();
        public readonly List<UxlType> Types = new List<UxlType>();

        public UxlDocument(SourcePackage upk, UxlBackendType backend, SourceValue? cond)
        {
            Package = upk;
            Backend = backend;
            Condition = cond;
        }

        internal static CacheWriter CreateWriter(SourcePackage upk, string filename)
        {
            if (upk.IsUnknown)
                throw new InvalidOperationException("UxlDocument: Unknown source package");

            var w = new CacheWriter(upk, filename);
            w.Write(Magic);
            return w;
        }

        internal static CacheReader CreateReader(SourcePackage upk, string filename)
        {
            var r = new CacheReader(upk, filename);
            r.VerifyMagic(Magic);
            return r;
        }

        public void Write(CacheWriter f)
        {
            UxlDocumentFlags flags = 0;
            if (Condition != null)
                flags |= UxlDocumentFlags.HasCondition;
            if (Defines.Count > 0)
                flags |= UxlDocumentFlags.Defines;
            if (Usings.Count > 0)
                flags |= UxlDocumentFlags.Usings;
            if (Declarations.Count > 0)
                flags |= UxlDocumentFlags.Declarations;
            if (Deprecations.Count > 0)
                flags |= UxlDocumentFlags.Deprecations;
            if (Templates.Count > 0)
                flags |= UxlDocumentFlags.Templates;
            if (Types.Count > 0)
                flags |= UxlDocumentFlags.Types;

            f.Write((byte)Backend);
            f.Write((byte)flags);
            if (flags.HasFlag(UxlDocumentFlags.HasCondition))
                f.WriteGlobal(Condition.Value);
            if (flags.HasFlag(UxlDocumentFlags.Defines))
                f.WriteList(Defines, (w, x) => x.Write(w));
            if (flags.HasFlag(UxlDocumentFlags.Usings))
                f.WriteList(Usings, (w, x) => w.WriteGlobal(x));
            if (flags.HasFlag(UxlDocumentFlags.Declarations))
                f.WriteList(Declarations, (w, x) => x.Write(w));
            if (flags.HasFlag(UxlDocumentFlags.Deprecations))
                f.WriteList(Deprecations, (w, x) => x.Write(w));
            if (flags.HasFlag(UxlDocumentFlags.Templates))
                f.WriteList(Templates, (w, x) => x.Write(w));
            if (flags.HasFlag(UxlDocumentFlags.Types))
                f.WriteList(Types, (w, x) => x.Write(w));

            var entityFlags = EntityFlags;
            f.Write((byte)entityFlags);
            WriteEntity(f, entityFlags);
        }

        public static UxlDocument Read(CacheReader f, SourcePackage upk)
        {
            var backend = (UxlBackendType) f.ReadByte();
            var flags = (UxlDocumentFlags)f.ReadByte();

            SourceValue? cond = null;
            if (flags.HasFlag(UxlDocumentFlags.HasCondition))
                cond = f.ReadGlobalValue();

            var doc = new UxlDocument(upk, backend, cond);

            if (flags.HasFlag(UxlDocumentFlags.Defines))
                f.ReadList(doc.Defines, UxlDefine.Read);
            if (flags.HasFlag(UxlDocumentFlags.Usings))
                f.ReadList(doc.Usings, r => r.ReadGlobalValue());
            if (flags.HasFlag(UxlDocumentFlags.Declarations))
                f.ReadList(doc.Declarations, UxlDeclare.Read);
            if (flags.HasFlag(UxlDocumentFlags.Deprecations))
                f.ReadList(doc.Deprecations, UxlDeprecate.Read);
            if (flags.HasFlag(UxlDocumentFlags.Templates))
                f.ReadList(doc.Templates, UxlTemplate.Read);
            if (flags.HasFlag(UxlDocumentFlags.Types))
                f.ReadList(doc.Types, UxlType.Read);

            var entityFlags = (UxlEntityFlags)f.ReadByte();
            doc.ReadEntity(f, entityFlags);
            return doc;
        }
    }
}
