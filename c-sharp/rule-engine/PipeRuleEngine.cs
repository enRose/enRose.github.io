using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barin.CrossCode
{
    public class StoreItem
    {
        public Type Type { get; set; }
        public dynamic State { get; set; }
    }

    public sealed class Store
    {
        private static readonly Lazy<Store> lazy = new Lazy<Store>(() => new Store());

        public static Store Instance { get { return lazy.Value; } }

        private Store() { }

        public Queue<StoreItem> Queue { get; set; } = new Queue<StoreItem>();

        public T DequeueOf<T>() where T: class
        {
            var s = Queue.Dequeue();

            if (typeof(T).Equals(s.Type))
            {
                return Convert.ChangeType(s.State, typeof(T));
            }

            return default;
        }

        public void EnqueueOf<T>(T s)
        {
            Queue.Enqueue(new StoreItem
            {
                Type = typeof(T),
                State = s
            });
        }
    }

    public abstract class PipeRule
    {
        public string ReasonIfFails => GetType().Name;

        public Store store = Store.Instance;

        public async Task<V> Fetch<T, V>(Func<T, Task<V>> f) 
            where T : class 
            where V : class
        {
            var current = store.DequeueOf<T>();

            var next = await f(current).ConfigureAwait(false);

            store.EnqueueOf(next);

            return next;
        }

        public Tuple<bool, V> Run<V, T>(Func<T, Tuple<bool, V>> v)
            where T : class
            where V : class
        {
            var current = store.DequeueOf<T>();

            var next = v(current);

            store.EnqueueOf(next.Item2);

            return next;
        }
    }

    public class PipeEngine
    {
        public List<PipeRule> Rules { get; set; } = new List<PipeRule>();
    }
}
