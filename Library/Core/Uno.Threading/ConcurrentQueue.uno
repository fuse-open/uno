using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    [DotNetType("System.Collections.Concurrent.ConcurrentQueue`1")]
    public class ConcurrentQueue<T>
    {
        readonly Queue<T> _queue;
        readonly object _mutex;

        public ConcurrentQueue()
        {
            _queue = new Queue<T>();
            _mutex = new object();
        }

        public void Enqueue(T item)
        {
            lock (_mutex)
                _queue.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            var ret = false;
            item = default(T);

            lock (_mutex)
            {
                if (_queue.Count > 0)
                {
                    item = _queue.Dequeue();
                    ret = true;
                }
            }

            return ret;
        }

        public int Count
        {
            get
            {
                int count = 0;

                lock (_mutex)
                    count = _queue.Count;

                return count;
            }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }
    }
}
