using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Barin.RuleEngine
{
    public class ClusterRuleCtx
    {
        public Application App { get; set; }

        public List<Reference> References { get; set; }
    }
    
    public class RuleClusterUsage
    {
        public RuleResult Run()
        {
            var ctx = new ClusterRuleCtx();

            return new RuleCluster().Rules.FirstFails(ctx);
        }
    }

    public class RuleCluster
    {
        public List<Func<ClusterRuleCtx, RuleResult>> Rules { get; set; }

        public RuleCluster()
        {
            Rules = new List<Func<ClusterRuleCtx, RuleResult>>() {};

            Rules.AddRange(new EligibilityRule().Rules);

            Rules.AddRange(new MarsRule().Rules);
        }
    }

    public class EligibilityRule
    {
        public List<Func<ClusterRuleCtx, RuleResult>> Rules =>
            new List<Func<ClusterRuleCtx, RuleResult>> {
                TermsConditionsRule,
                AgeRule
            };

        public RuleResult TermsConditionsRule(ClusterRuleCtx ctx)
        {
            return new RuleResult {
                IsValid = ctx.App.DoYouAgreeTermsconditions.IsYes(),
                
                Reason = $"Customer did not agree Ts & Cs: " +
                    $"{ctx.App.DoYouAgreeTermsconditions}"
            };
        }

        public RuleResult AgeRule(ClusterRuleCtx ctx)
        {
            return new RuleResult {
                IsValid = Util.IsOver(18, ctx.App.Dob),

                Reason = $"Customer underaged: " +
                    $"{ctx.App.Dob}"
            };
        }
    }

    public class MarsRule
    {
        public List<Func<ClusterRuleCtx, RuleResult>> Rules =>
            new List<Func<ClusterRuleCtx, RuleResult>> {
                NumOfYearsOnMarsRule,
                AddressRule_CSharp_8
            };

        public RuleResult NumOfYearsOnMarsRule(ClusterRuleCtx ctx)
        {
            return new RuleResult
            {
                IsValid = ctx.App.NumOfYearsOnMars >= 5,
                
                Reason = $"Not enough years on Mars: " +
                    $"{ctx.App.NumOfYearsOnMars}"
            };
        }

        public RuleResult AddressRule_CSharp_8(ClusterRuleCtx ctx)
        {
            var months = ctx.App.MonthsAtCurrentAddress;
            var years = ctx.App.YearsAtCurrentAddress;
            var totalInMonths = months + years * 12;

            var preAddress = ctx.App.PreviousAddress;
            var homeAddress = ctx.App.HomeAddress;

            if (preAddress.IsAddressValid() == false) 
            {
                return new RuleResult() { 
                    IsValid = false,
                    Reason = "Previous address invalid"
                };
            }

            var r = totalInMonths switch
            {
                var n when n < 0 => (false, "below zero"),

                var n when n < 12 => (!preAddress.IsSame(homeAddress), 
                    "Previous address cannot be the same as home address" +
                    " if current address is less than 12 months"),

                var n when n >= 12 => (true, ""),
            
                _ => (true, "")
            };

            return new RuleResult() { 
                IsValid = r.Item1,
                Reason = r.Item2
            };
        }
    }
}
