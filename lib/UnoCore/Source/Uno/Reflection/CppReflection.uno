using Uno.Compiler.ExportTargetInterop;

namespace Uno.Reflection
{
    public interface IField
    {
        string Name { get; }
        bool IsStatic { get; }
        Type DeclaringType { get; }
        Type FieldType { get; }

        object GetValue(object obj);
        void SetValue(object obj, object value);
    }

    public interface IFunction
    {
        string Name { get; }
        bool IsStatic { get; }
        bool IsVirtual { get; }
        Type DeclaringType { get; }
        Type ReturnType { get; }
        Type[] ParameterTypes { get; }

        Delegate CreateDelegate(Type type, object obj);
        object Invoke(object obj, params object[] args);
    }

    [TargetSpecificType]
    [Set("TypeName", "uField*")]
    extern(CPLUSPLUS && REFLECTION)
    public struct CppField : IField
    {
        public static CppField Null
        {
             get { return extern<CppField> "nullptr"; }
        }

        public bool IsNull
        {
            get { return extern<bool> "*$$ == nullptr"; }
        }

        public bool IsStatic
        {
            get { return extern<bool> "uPtr(*$$)->Info().Flags & uFieldFlagsStatic"; }
        }

        public string Name
        {
            get { return extern<string> "uPtr(*$$)->Name"; }
        }

        public Type DeclaringType
        {
            get { return extern<Type> "uPtr(*$$)->DeclaringType"; }
        }

        public Type FieldType
        {
            get { return extern<Type> "uPtr(*$$)->Info().Type"; }
        }

        public object GetValue(object obj)
        {
            return extern<object> "uPtr(*$$)->GetValue($@)";
        }

        public void SetValue(object obj, object value)
        {
            extern<object> "uPtr(*$$)->SetValue($@)";
        }
    }

    [TargetSpecificType]
    [Set("TypeName", "uFunction*")]
    extern(CPLUSPLUS && REFLECTION)
    public struct CppFunction : IFunction
    {
        public static CppFunction Null
        {
            get { return extern<CppFunction> "nullptr"; }
        }

        public bool IsNull
        {
            get { return extern<bool> "*$$ == nullptr"; }
        }

        public bool IsStatic
        {
            get { return extern<bool> "uPtr(*$$)->Flags & uFunctionFlagsStatic"; }
        }

        public bool IsVirtual
        {
            get { return extern<bool> "uPtr(*$$)->Flags & uFunctionFlagsVirtual"; }
        }

        public string Name
        {
            get { return extern<string> "uPtr(*$$)->Name"; }
        }

        public Type DeclaringType
        {
            get { return extern<Type> "uPtr(*$$)->DeclaringType"; }
        }

        public Type ReturnType
        {
            get { return extern<Type> "uPtr(*$$)->ReturnType"; }
        }

        public Type[] ParameterTypes
        {
            get { return extern<Type[]> "uPtr(*$$)->ParameterTypes"; }
        }

        public Delegate CreateDelegate(Type type, object obj)
        {
            return extern<Delegate> "uPtr(*$$)->CreateDelegate($@)";
        }

        public object Invoke(object obj, params object[] args)
        {
            return extern<object> "uPtr(*$$)->Invoke($@)";
        }
    }

    [TargetSpecificImplementation]
    extern(CPLUSPLUS && REFLECTION)
    public class CppReflection
    {
        public static Type[] GetTypes()
        {
            return extern<Type[]> "uReflection::GetTypes()";
        }

        public static CppField[] GetFields(Type type)
        {
            extern "uPtr($0)";
            return extern<CppField[]> "uArray::New(@{CppField[]:TypeOf}, $0->Reflection.FieldCount, $0->Reflection.Fields)";
        }

        public static CppFunction[] GetFunctions(Type type)
        {
            extern "uPtr($0)";
            return extern<CppFunction[]> "uArray::New(@{CppFunction[]:TypeOf}, $0->Reflection.FunctionCount, $0->Reflection.Functions)";
        }

        public static CppField FindField(Type type, string name)
        {
            return extern<CppField> "uPtr($0)->Reflection.GetField($1)";
        }

        public static CppField FindFieldFromObject(object obj, string name)
        {
            return extern<CppField> "$0 != nullptr ? $0->GetType()->Reflection.GetField($1) : nullptr";
        }

        public static CppFunction FindFunction(Type type, string name, params Type[] parameterTypes)
        {
            return extern<CppFunction> "uPtr($0)->Reflection.GetFunction($1, $2)";
        }

        public static CppFunction FindFunctionFromObject(object obj, string name, params Type[] parameterTypes)
        {
            return extern<CppFunction> "$0 != nullptr ? $0->GetType()->Reflection.GetFunction($1, $2) : nullptr";
        }
    }
}
