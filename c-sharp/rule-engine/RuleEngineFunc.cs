using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Barin.RuleEngine
{
    public class RuleResult
    { 
        public bool IsValid { get; set; }
        public string Reason { get; set; }
    }

    public class RuleResultWithSideEffect
    {
        public bool IsValid { get; set; }
        public string Reason { get; set; }
        public (Type type, object val) SideEffect { get; set; }
    }

    public class RuleEngine<T> where T : class, new()
    {
        public Dictionary<Type, object> SideEffect { get; set; }
            = new Dictionary<Type, object>();

        public TSideEffect Pick<TSideEffect>() where TSideEffect: class
        {
            if (SideEffect.Count == 0) 
            {
                return null;
            }

            if (SideEffect.TryGetValue(typeof(TSideEffect), out object value))
            {
                return (TSideEffect)value;
            }

            return null;
        }

        public RuleResultWithSideEffect FirstFailsWithSideEffect(
            IList<Func<T, RuleResultWithSideEffect>> sequentialRules, T ruleCtx)
        {
            RuleResultWithSideEffect firstFailed = null;

            _ = sequentialRules?.FirstOrDefault(r =>
            {
                var result = r(ruleCtx);

                SideEffect[result.SideEffect.type] = result.SideEffect.val;

                firstFailed = result.IsValid == false ? result : null;

                return firstFailed != null;
            });

            return firstFailed;
        }

        public RuleResult FirstFails(
            IList<Func<T, RuleResult>> sequentialRules, T ruleCtx)
        {
            RuleResult firstFailed = null;

            _ = sequentialRules?.FirstOrDefault(r =>
            {
                var result = r(ruleCtx);

                firstFailed = result.IsValid == false ? result : null;

                return firstFailed != null;
            });

            return firstFailed;
        }

        public IEnumerable<RuleResult> All(
            IList<Func<T, RuleResult>> sequentialRules, T ruleCtx)
            => sequentialRules?.Select(r => r(ruleCtx));

        public async Task<RuleResult> FirstFails(
            IList<Func<T, Task<RuleResult>>> sequentialAsyncRules, T ruleCtx)
        {
            if (sequentialAsyncRules?.Count < 1)
            {
                return null;
            }

            RuleResult firstFailed = null;

            foreach (var rule in sequentialAsyncRules)
            {
                var r = await rule(ruleCtx).ConfigureAwait(false);

                if (r.IsValid == false)
                {
                    firstFailed = r;

                    break;
                }
            }

            return firstFailed;
        }

        public async Task<IList<RuleResult>> ParallelAll(
            List<Func<T, Task<RuleResult>>> parallelAsyncRules, T ruleCtx)
        {
            if (parallelAsyncRules?.Count < 1)
            {
                return null;
            }

            var tasks = parallelAsyncRules.Select(r => r(ruleCtx));

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    public static class RuleEngineExtensions
    {
        public static RuleResult FirstFails<T> (
            this IList<Func<T, RuleResult>> sequentialRules, T ruleCtx)
            where T : class, new()
        {
            return new RuleEngine<T>().FirstFails(sequentialRules, ruleCtx);
        }
        
        public static IEnumerable<RuleResult> All<T>(
            this IList<Func<T, RuleResult>> sequentialRules, T ruleCtx)
            where T : class, new() =>
            new RuleEngineCluster<T>().All(sequentialRules, ruleCtx);

        public static async Task<RuleResult> FirstFails<T>(
            this IList<Func<T, Task<RuleResult>>> sequentialAsyncRules,  T ruleCtx)
            where T : class, new()
        {
            return await new RuleEngine<T>()
                .FirstFails(sequentialAsyncRules, ruleCtx)
                .ConfigureAwait(false);
        }

        public static async Task<IList<RuleResult>> ParallelAll<T>(
            this List<Func<T, Task<RuleResult>>> parallelAsyncRules, T ruleCtx)
            where T : class, new()
        {
            return await new RuleEngine<T>()
                .ParallelAll(parallelAsyncRules, ruleCtx)
                .ConfigureAwait(false);
        }
    }
}
