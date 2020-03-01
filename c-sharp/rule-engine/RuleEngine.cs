using System.Collections.Generic;
using System.Linq;

namespace Barin.RuleEngine.NonAsyncEngine
{
    public abstract class Rule<T>
    {
        public int Priority { get; set; }
        public abstract bool IsValid(T ctx);
    }

	public enum Priority
	{
		Ascending,
		Descending,
	}

    public abstract class RuleEngine<T>
    {
        public T Ctx { get; set; }

        public Priority Priority { get; set; }

        public IEnumerable<Rule<T>> Rules { get; set; }

        public virtual string FirstFails(string valid = "Ok")
        {
			var rules = Priority == Priority.Ascending ?
				Rules.OrderBy(x => x.Priority) :
				Rules.OrderByDescending(x => x.Priority);

			var failedRule = rules.FirstOrDefault(r =>
                r.IsValid(Ctx) == false);

            return failedRule == null ? valid : nameof(failedRule);
        }

        public virtual IDictionary<string, string> All(string valid = "Ok")
        {
			return rules
                .Where(r => r.IsValid(Ctx) == false)
                .Select(r => new KeyValuePair<string, string>(
                    r.ReasonIfFails, nameof(r)
                ))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}