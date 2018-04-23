namespace Mono.test_136
{
    //
    // Tests that explicit and normal implementations of methods are handled
    // properly.  Before we used to have the normal method implementation
    // "implement" the classes, so that it would go into an infinite loop.
    // (bug #26334)
    //
    // Now explicit implementations are defined first.
    //
    using Uno;
    
    public interface IDiagnostic
    {
        void Stop();
    } 
    public interface IAutomobile
    {
        void Stop();
    }
    
    public class MyCar: IAutomobile, IDiagnostic {
        public bool diag_stop, car_stop, auto_stop;
        
        void IDiagnostic.Stop() {
            diag_stop = true;
        }
    
        public void Stop() {
            car_stop = true;
            IAutomobile self = (IAutomobile)this; // cast this
            self.Stop(); // forwarding call
        }
    
        void IAutomobile.Stop()
        {
            auto_stop = true;
        }
    }
    
    class TestConflict {
        [Uno.Testing.Test] public static void test_136() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            MyCar car1 = new MyCar();
            car1.Stop(); // calls the IAutomobile.Stop implementation
            
            IDiagnostic car2 = new MyCar();
            car2.Stop();
            
            IAutomobile car3 = new MyCar();
            car3.Stop();
    
            if (!car1.car_stop)
                return 1;
    
            if (car1.diag_stop)
                return 2;
    
            Console.WriteLine ("ok");
            return 0;
        }
    }
}
