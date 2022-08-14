using System.Reflection;
using Uno.Compiler.ExportTargetInterop;
using Uno.IO;

namespace System.Reflection
{
    [DotNetType]
    extern(DOTNET) 
    public abstract class Assembly
    {
        public virtual extern string Location { get; }
        public virtual extern bool GlobalAssemblyCache { get; }

        public virtual extern string[] GetManifestResourceNames();
        public virtual extern Stream GetManifestResourceStream(string filename);
        public virtual extern AssemblyName GetName();
        public static extern Assembly GetEntryAssembly();
        public static extern Assembly GetExecutingAssembly();
        public static extern Assembly LoadFrom(string assemblyFile);
    }

    [DotNetType]
    extern(DOTNET) 
    public sealed class AssemblyName
    {
        public virtual extern string Name { get; }
    }
}

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Type")]
    [extern(CPLUSPLUS) Set("TypeName", "uType*")]
    [extern(CPLUSPLUS) Set("TypeOfType", "uType")]
    public sealed class Type
    {
        public static readonly Type[] EmptyTypes = new Type[0];

        internal Type()
        {
        }

        public static bool operator ==(Type a, Type b)
        {
            return ReferenceEquals(a, b);
        }

        public static bool operator !=(Type a, Type b)
        {
            return !ReferenceEquals(a, b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }

        // Note: The following methods may be declared abstract or virtual in .NET,
        // but this is not necessary in Uno because the class is sealed.
        public Type BaseType
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<Type> "$$->Base";
                else
                    throw new NotImplementedException();
            }
        }

        public bool ContainsGenericParameters
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "!$$->IsClosed()";
                else
                    throw new NotImplementedException();
            }
        }

        public string FullName
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<string> "uString::Utf8($$->FullName)";
                else
                    throw new NotImplementedException();
            }
        }

        public int GenericParameterPosition
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<int> "$$->Type == uTypeTypeGeneric ? (int)((uGenericType*)$$)->GenericIndex : -1";
                else
                    throw new NotImplementedException();
            }
        }

        public bool HasElementType
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->Type == uTypeTypeArray || $$->Type == uTypeTypeByRef";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsArray
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->Type == uTypeTypeArray";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsByRef
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->Type == uTypeTypeByRef";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsClass
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->Type != uTypeTypeInterface && U_IS_OBJECT($$)";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsEnum
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->Type == uTypeTypeEnum";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsGenericParameter
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->Type == uTypeTypeGeneric";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsGenericType
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->GenericCount > 0";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsGenericTypeDefinition
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->GenericCount > 0 && $$->Definition == $$";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsInterface
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "$$->Type == uTypeTypeInterface";
                else
                    throw new NotImplementedException();
            }
        }

        public bool IsValueType
        {
            get
            {
                if defined(CPLUSPLUS)
                    return extern<bool> "U_IS_VALUE($$)";
                else
                    throw new NotImplementedException();
            }
        }

        public Type GetElementType()
        {
            if defined(CPLUSPLUS)
                return IsArray ? extern<Type> "((uArrayType*)$$)->ElementType"
                     : IsByRef ? extern<Type> "((uByRefType*)$$)->ValueType"
                     : null;
            else
                throw new NotImplementedException();
        }

        public Type[] GetGenericArguments()
        {
            if defined(CPLUSPLUS)
            {
                var array = new Type[extern<int> "$$->GenericCount"];
                for (int i = 0; i < array.Length; i++)
                    array[i] = extern<Type>(i) "$$->Generics[$0]";
                return array;
            }
            else
                throw new NotImplementedException();
        }

        public Type GetGenericTypeDefinition()
        {
            if defined(CPLUSPLUS)
                return extern<Type> "$$->Definition";
            else
                throw new NotImplementedException();
        }

        public Type[] GetInterfaces()
        {
            if defined(CPLUSPLUS)
            {
                var array = new Type[extern<int> "$$->InterfaceCount"];
                for (int i = 0; i < array.Length; i++)
                    array[i] = extern<Type>(i) "$$->Interfaces[$0].Type";
                return array;
            }
            else
                throw new NotImplementedException();
        }

        extern(REFLECTION)
        public static Type GetType(string typeName)
        {
            if defined(CPLUSPLUS)
                return extern<Type> "uReflection::GetType($0)";
            else
                throw new NotImplementedException();
        }

        extern(REFLECTION)
        public static Type GetType(string typeName, bool throwOnError)
        {
            if defined(CPLUSPLUS)
            {
                var type = extern<Type> "uReflection::GetType($0)";
                if (throwOnError && type == null)
                    throw new ArgumentException(nameof(typeName));
                return type;
            }
            else
                throw new NotImplementedException();
        }

        public bool IsSubclassOf(Type c)
        {
            if defined(CPLUSPLUS)
                return extern<bool> "$$->Is($0)";
            else
                throw new NotImplementedException();
        }

        public Type MakeArrayType()
        {
            if defined(CPLUSPLUS)
                return extern<Type> "$$->Array()";
            else
                throw new NotImplementedException();
        }

        public Type MakeByRefType()
        {
            if defined(CPLUSPLUS)
                return extern<Type> "$$->ByRef()";
            else
                throw new NotImplementedException();
        }

        public Type MakeGenericType(params Type[] typeArguments)
        {
            if defined(CPLUSPLUS)
            {
                if (typeArguments == null)
                    throw new ArgumentNullException(nameof(typeArguments));
                return extern<Type> "$$->MakeGeneric((size_t)$0->Length(), (uType**)$0->Ptr())";
            }
            else
                throw new NotImplementedException();
        }

        extern(DOTNET)
        public Assembly Assembly { get; }
    }
}
