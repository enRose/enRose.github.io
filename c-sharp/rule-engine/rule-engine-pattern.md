# Rule Engine Pattern

**29 Jan 2020**

Before delving into what the pattern is, it is important to know where the inspiration is drawn from - [Unix Rule of Separation](https://enrose.github.io/c-sharp/unix-rule-of-separation).

## Motivation

Quite often in write operations, we need to satisfy a set of requirements before data persistence. Or, we have a set of preliminary requirements to be met before servicing users. Or maybe validate some data against a set of validation rules.

In a contrived example below, we need to check the conditions of the zoo and report on each species wellbeing.

To implement, naturally we think in if statement.

```C#
public class ZooChecker
{
    public string Check(Zoo zoo)
    {
        var checks = new List<string>
        {
            ValidateCats(zoo.Cats),
            ValidateFrogs(zoo.Frogs),
            ValidateDolphins(zoo.Dolphins),
        };

        var failed = checks.FirstOrDefault(
                x => x != "Ok"
            );

        return failed ?? "Ok";
    }

    public string ValidateCats(List<Cat> cats)
    {
        if (cats.Food < 5)
        {
            return "Not enough food";
        }

        if (cats.Water < 10)
        {
            return "Not enough water";
        }

        if (cats.Shelter.Capacity < cats.Members.Count)
        {
            return "Too many cats";
        }

        var healthReport = "";

        foreach (var cat in cats.Members)
        {
            if (cat.Health.Rating < 5)
            {
                healthReport += cat.name + cat.Health.Condition;
            }
        }

        return healthReport == "" ? healthReport : ""
    }

    public string ValidateDolphins()
    {
        // Do dolphins specific checks.
    }

    public string ValidateFrogs()
    {
        // Do frogs specific checks.
    }
}
```

There are several issues with this approach:

1. Drown in a sea of if statements.

    In production code, I have seen a single validation class has a train of if-else statements last for hundreds line of code, some that have 7 or 8 conditions in a single if statement. I must say whoever reads it must be half human half compiler.

    Well, this is not a problem if:

    ```C#
        if (this code is written only once and never be changed again ||
            ((the developer wrote it can make changes fairly quick && never leaves the company || next developer can make changes fairly quick && never leaves the company) && has tests cover && easy to write new tests))
        {
            return "Not a problem";
        }
        else
        {
            return "Houston, we have a problem";
        }
    ```

2. Ad-hoc priority management.

    Say we care more about the health conditions of animals more than shelter capacity so we want to return the health validation error first, then we have to fiddle around the validation method copy and past block of code does health check up to the top.

    This lack of priority management opens up the possibility of errors. Even worse if sections of the if-else blocks interlinked or share state.

3. Lack of state management.

    There is no easy way to share state or to export the side effect after validations have run.

    In programming, what we do in essence, is read or write (mutate) state and the logic to do so. So state is pretty important. The side effect of one validation method might become the input state of subsequent or other validation methods. Imagine we have validation for scavenger fish which input state is depending of other species output state.

4. Lack of variety of logical operations.

    There is no declarative way to specify how do we want validation methods to be run. We may want to run till we hit the first fails. We may want to run all the validation methods does not matter they fail or not.

    As need arises, we may need more different executions.

## The componentry of an engine

- Context

    The shared state that a collection of rules are running in. It defines the input state as well any side effect rules might have. As Rule Engine evolves, Context could be separated out as an agnostic state machine mutable or immutable.

- Rule

    A block of code that follows Single Responsibility and offloads side effect to Context if any.

    Some think that Single Responsibility means doing one thing. However, the advocate of SOLID Robert C. Martin who started off SOLID (although he did not invent the abbreviation) says that SR means single reason to change. In real production code, I found the latter is more practical because it is literally impossible to make each single method or class do one thing or we might get buried in what I call the function-inception - little one liner functions although does one thing but really have no point because the function names does not give us more understanding than that one liner code itself! And we're really just abusing Stack in memory.

    Also it makes sense because the whole idea of SOLID is being agile - how to go about responding to change. And if a unit of code has a single reason to change it is highly likely all its sub-units have the same frequency of change. Although it is doing multiple things, because this single reason, all its sub-units are cohesively glued together in the same theme.

- Engine

    A generic, rule agnostic unit of operation that runs a collection of rules in a given configuration - **async, pipe, first-in-first-run, parallel, exclusive-or, logical-and**, etc.

    The key is to separate the business rules vs running the rules. So rules and rule-runner can evolve independently.

Let's see the rule engine then we demonstrate how to use it.

> There are many ways to write an engine and this is one of them. You extend the concept by adding async or parallel runner etc. 

```c#
using System.Collections.Generic;
using System.Linq;

namespace Barin.RuleEngine.NonAsyncEngine
{
    public abstract class Rule<T>
    {
        public int Priority { get; set; }
        public virtual string ReasonIfFails => "Invalid";
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

			var failed = rules.FirstOrDefault(r => 
                r.IsValid(Ctx) == false);

            return failed == null ? valid : failed.ReasonIfFails;
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
```

This demo engine has two running configurations: hits the first failed rule then short-circuit; and runs all rules returns a dictionary contains any rules that failed.

Let's look at the rules and rule collection.



## Rules

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using Barin.RuleEngine.Util;

namespace Barin.RuleEngine.NonAsyncRules
{
    public class CatRuleCtx
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public List<Owner> Owners { get; set; }
    }

    public class CatRules : RuleEngine<CatRuleCtx>
    {
        public CatRules(CatRuleCtx ctx)
        {
            Ctx = ctx;

            Rules = new List<Rule<CatRuleCtx>> {
                new MustHaveAnOwnerRule(),
                new MustLoveCatRule(),
                new CurrentOwnersMustOver16YearsOld(),
            };
        }
    }

    public class MustHaveAtLeastOneCurrentOwnerRule : Rule<CatRuleCtx>
    {
        public override bool IsValid(CatRuleCtx ctx)
        {
            var currentOwners = ctx.Owners.FirstOrDefault(
                x => x.IsCurrent
            );

            return currentOwners != null;
        }
    }

    public class MustLoveCatRule : Rule<CatRuleCtx>
    {
        public override bool IsValid(CatRuleCtx ctx)
        {
            var areTheyAllLoveCats = ctx.Owners
                .Where(x => x.IsCurrent)
                .All(
                    x => x.Attitude == "has a cats loving heart"
                );

            return areTheyAllLoveCats;
        }
    }

    public class CurrentOwnersMustOver16YearsOld : Rule<CatRuleCtx>
    {
        public override bool IsValid(CatRuleCtx ctx)
        {
            var allOver16 = ctx.Owners
                .Where(x => x.IsCurrent)
                .All(
                    x => Moment.IsOver(16, x.Dob)
                );

            return allOver16;
        }
    }
}
```

## Usage

```c#
var ctx = new CatRuleCtx();
var catRules = new CatRules(ctx);
var isValid = await catRules.Run().ConfigureAwait(false);
```
