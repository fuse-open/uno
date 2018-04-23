namespace Mono.gtest_344
{
    using Uno;
    
    public abstract class ConfigurationElement
    {
        protected ConfigurationElement ()
        {
        }
        
        [Uno.Testing.Test] public static void gtest_344() { Main(); }
        public static void Main()
        {
        }
    }
    
    public class CustomConfigurationElement : ConfigurationElement
    {
    }
    
    public class CustomConfigurationElementCollection : BaseCollection<CustomConfigurationElement>
    {
    }
    
    public class BaseCollection<T> where T : ConfigurationElement, new ()
    {
    }
}
