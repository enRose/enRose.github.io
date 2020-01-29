using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barin.RuleEngine
{
    public interface IV8RuleCtx
    {
        string FailedAt { get; set; }
    }

    public class V8Rule<T> where T : IV8RuleCtx
    {
        public T Ctx { get; set; }
        public Func<Task<bool>>[] Rules { get; set; }

        public void Init<V>(T ctx) where V : V8Rule<T> 
        {
            Ctx = ctx;
        }

        public async Task<bool> Run()
        {
            if (Rules?.Length < 1)
            {
                return false;
            }

            var success = true;

            foreach (var rule in Rules)
            {
                success = await rule().ConfigureAwait(false);

                if (success == false)
                {
                    Ctx.FailedAt = nameof(rule);

                    break;
                }
            }

            return success;
        }
    }
}
