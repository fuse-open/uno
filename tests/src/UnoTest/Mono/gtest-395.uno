namespace Mono.gtest_395
{
    public class RuleBuilder<T> where T : class {}
    
    public interface IDynamicObject {
        RuleBuilder<T> GetRule<T>() where T : class;
    }
    
    public class RubyMethod : IDynamicObject {
        RuleBuilder<T> IDynamicObject.GetRule<T>() /* where T : class */ {
            return new RuleBuilder<T>();
        }
    }
    
    public class T {
        [Uno.Testing.Test] public static void gtest_395() { Main(); }
        public static void Main()
        {
        }
    }
}
