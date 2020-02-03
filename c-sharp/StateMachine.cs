using Barin.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Barin.CrossCode
{
    public class ContextualStateMachine
    {
        public Dictionary<Type, dynamic> Store { get; set; }

        public void Usage()
        {
            var companyCtx = new CompanyRuleCtx{ };

            Store.Add(typeof(CompanyRuleCtx), companyCtx);

            var s = Get<CustomerRuleCtx>();
        }

        public T Get<T>() where T : class
        {
            var t = typeof(T);

            if (Store.TryGetValue(t, out dynamic o))
            {
                return Convert.ChangeType(o, t);
            }

            return default;
        }
    }

    public class StoreItem
    {
        public Type Type { get; set; }
        public dynamic State { get; set; }
    }


    public sealed class SingletonStateMachine
    {
        private static readonly Lazy<SingletonStateMachine> lazy = new Lazy<SingletonStateMachine>(() => new SingletonStateMachine());

        public static SingletonStateMachine Instance { get { return lazy.Value; } }

        private SingletonStateMachine() { }

        public Queue<StoreItem> Queue { get; set; } = new Queue<StoreItem>();

        public T DequeueOf<T>() where T : class
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
}
