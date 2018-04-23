namespace Mono.test_867
{
    class Test
    {
        [Uno.Testing.Test] public static void test_867() { Main(); }
        public static void Main()
        {
            new BaseJobController ();
            new JobController ();
        }
    }
    
    public interface IUser
    {
    }
    
    public class User : IUser
    {
    }
    
    public interface IJobController
    {
        IUser User { get; }
    }
    
    public class BaseController
    {
        public virtual IUser User { get; set; }
    }
    
    public class BaseJobController : BaseController
    {
        public new User User { get; set; }
    }
    
    public class JobController : BaseJobController, IJobController
    {
    }
}
