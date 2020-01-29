```
namespace Barin.RuleEngineExample
{
    // For DI to work, we need an interface.
    public interface ICatRuleCtx {}

    public class CatRuleCtx : ICatRuleCtx, IV8RuleCtx
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public List<Owner> Owners { get; set; }
        public Owner GoodOwner { get; set; }
        public string FailedAt { get; set; }

        [Dependency]
        public ICatRepository CatRepository { get; set; }
    }

    public class CatRules : V8Rule<CompanyRuleCtx>
    {
        public CompanyRules(CompanyRuleCtx ctx)
        {
            Ctx = ctx;
        }

        public async Task<bool> LinkedOwnersRule()
        {
            var owners = await Ctx.CatRepository.GetOwners(
                Ctx.Id
            )
            .ConfigureAwait(false);
            
             Ctx.Owners = owners;

            return owners?.Count > 0;
        }

        public Task<bool> MustLoveCatRule()
        {
            var goodOwner = Ctx.Owners.FirstOrDefault(
                x => x.attitude == "has a cats loving heart"
            );

            if (goodOwner == null)
            {
                return Task.FromResult(false);
            }

            Ctx.GoodOwner = goodOwner;

            return Task.FromResult(true);
        }
    }
}
```
