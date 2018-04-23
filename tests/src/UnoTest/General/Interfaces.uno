using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Interfaces
    {
        class A
        {
            public string Foo() { return null; }
        }

        interface IB
        {
            string Foo();
        }

        class B : A, IB
        {
        }

        interface IFoo
        {
        }

        interface IBar
        {
        }

        class FooBar : IFoo, IBar
        {
        }

        public interface IWrapper { }
        public class Wrapper : IWrapper { }

        public interface AnnotatedElement : IWrapper
        {
            T getAnnotation<T>(Class<T> arg0) where T : Wrapper;
            T getAnnotation2<T>(Class<T> arg0) where T : Wrapper;
        }

        public static class WrapperFactory
        {
            public static Wrapper Test()
            {
                return new Wrapper();
            }
        }

        public class ClassBase
        {
            public virtual T Foo<T>(T wrap) where T : Wrapper
            {
                return null;
            }
        }

        public class JObject : Wrapper {}

        public abstract class Enum<E> : JObject where E:IWrapper
        {
        }

        public class Test : Enum<JObject>
        {
        }

        public class Class<T> : ClassBase, AnnotatedElement where T : Wrapper
        {
            public override T Foo<T>(T wrap) // Constraints not allowed, inherited from base method
            {
                return null;
            }

            T AnnotatedElement.getAnnotation<T>(Class<T> arg0) // Constraints not allowed, inherited from interface method
            {
                return (T)WrapperFactory.Test();
            }

            public T getAnnotation2<T>(Class<T> arg0) where T : Wrapper // Constraints required, must be identical to interface method
            {
                return default(T);
            }
        }

        [Test]
        public void Run()
        {
            object foobar = new FooBar();

            var foo = (IFoo)foobar;
            var bar = (IBar)foo;

            var asIFoo = bar as IFoo;
            var asIBar = foo as IBar;

            var isIFoo = bar is IFoo;
            var isIBar = foo is IBar;

            var a = (AnnotatedElement)new Class<Wrapper> ();
            a.getAnnotation<Wrapper>(null);
            a.getAnnotation2<Wrapper>(null);
            ((ClassBase)a).Foo<Wrapper>(null);

            Func<string> func = ((IB)new B()).Foo;
            Assert.IsTrue(func() == null);
        }
    }
}
