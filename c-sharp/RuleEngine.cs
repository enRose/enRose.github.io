using System.Collections.Generic;
using System.Linq;

namespace Barin.RuleEngine
{
    public abstract class Rule<T>
    {
        public static string DefaultReason = "Invalid";
        public virtual string ReasonIfFails => DefaultReason;
        public abstract bool IsValid(T ctx);
    }

    public abstract class RuleEngine<T>
    {
        public IList<Rule<T>> Rules { get; set; }

        public virtual string FirstFails(T ruleCtx, string defaultValidity)
        {
            var failed = Rules.FirstOrDefault(r => r.IsValid(ruleCtx) == false);

            return failed == null ? defaultValidity : failed.ReasonIfFails;
        }
    }
}
