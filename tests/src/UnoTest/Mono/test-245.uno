namespace Mono.test_245
{
    public class Class2 
    {
        private AliasDefOperations __delegate;
    
        public string def_kind {
            get {
                return __delegate.def_kind;
            }
        }
    
        [Uno.Testing.Test] public static void test_245() { Main(); }
        public static void Main()
        { }
    }
    
    public interface AliasDefOperations : ContainedOperations, IDLTypeOperations 
    {
    }
    
    public interface ContainedOperations : IRObjectOperations 
    {
    }
    
    public interface IDLTypeOperations : IRObjectOperations 
    {
    }
    
    public interface IRObjectOperations
    {
       string def_kind { get; }
    }
}
