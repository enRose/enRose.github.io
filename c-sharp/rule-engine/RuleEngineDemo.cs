using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Barin.RuleEngine.Demo
{
    public class JustInTimeSupplierRuleCtx 
    {
        public ISupplierDetails SupplierDetails { get; set; }

        public IProcurement Procurement { get; set; }

        public List<Supplier> Suppliers { get; set; }

        public Guid CompanyId { get; set; }
    }

    public class JustInTimeSupplierRuleDemo
    {
        public Func<JustInTimeSupplierRuleCtx, Task<RuleResult>>[] Rules => 
            new Func<JustInTimeSupplierRuleCtx, Task<RuleResult>>[] {
                JustInTimeSuppliersRule,
                JustInTimeInventoryAccessRule
            };
        
        public async Task<RuleResult> JustInTimeSuppliersRule(JustInTimeSupplierRuleCtx ctx)
        {
            var jitSuppliers = await ctx.SupplierDetails
                .GetJustInTimeSupplier(ctx.CompanyId).ConfigureAwait(false);

            ctx.Suppliers = jitSuppliers
                .Select(x => new Supplier
                {
                    CompanyId = x.ClientId,
                    Name = x.Name,
                    InventoryAccess = x.ClientTypeRelationshipCode,
                    Type = x.SupplierType
                })
                .ToList();

            return new RuleResult
            {
                IsValid = ctx?.Suppliers?.Count > 0,
                Reason = "No Just-In-Time Supplier available"
            };
        }

        public async Task<RuleResult> JustInTimeInventoryAccessRule(JustInTimeSupplierRuleCtx ctx)
        {
            ctx.Suppliers = ctx.Suppliers
                .Where(x => x.InventoryAccess == "Just-In-Time")
                .ToList();

            return new RuleResult
            {
                IsValid = ctx?.Suppliers?.Count > 0,
                Reason = "No just-in-time inventory access"
            };
        }
    }

    public class Demo
    {
        public async Task<RuleResult> Run()
        {
            var ctx = new JustInTimeSupplierRuleCtx
            {
                SupplierDetails = DI.Get<ISupplierDetails>(),
                Procurement = DI.Get<IProcurement>(),
                CompanyId = new Guid()        
            };

            return await new JustInTimeSupplierRuleDemo()
                .Rules
                .FirstFails(ctx)
                .ConfigureAwait(false);
        }
    }
}
