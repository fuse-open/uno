using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.WeakReference`1")]
    public sealed class WeakReference<T>
        where T : class
    {
        [WeakReference]
        T _target;

        public WeakReference(T target)
        {
            _target = target;
        }

        public void SetTarget(T target)
        {
            _target = target;
        }

        public bool TryGetTarget(out T target)
        {
            var result = _target;
            target = result;
            return result != null;
        }
    }
}
