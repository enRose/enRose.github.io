# Unix Rule of Separation: Separate policy from mechanism

__30 Jan 2020__

I personally don't like to think it as a rule as such, rather I see it just like any other rules or principles, as a perspective from which we think and reason our code structure.

This rule looks a bit abstract, so let me give you an every day experience so you may understand and remember what it means.

Every time I try explaining this to my colleagues I use the example of the sliding doors at our work building so here we go:

At work we have a pair of sliding doors when we tap our access card to the sensor, they open. Now, imagine Jon Snow just joined the company, before Jon's first day, the building manager goes: okay, there is a new guy Jon, we have to change the sliding doors so his access card can work.

But wait, this is NOT how it turns out, we don't change the sliding doors whenever there is a new employee. Instead, we alter the database of access control to add a new entry for Jon, then we imprint the new access code into the chip. Now the sliding doors are what in Unix refers to as the mechanism - opening/closing operation. On the other hand the access control database is the policy or the place that stores the policy which determines who is allowed or disallowed access. In this case, mechanism and policy are physically separated.

Now, let's look at another example where mechanism and policy are amalgamated. We don't need to look afar -- the front door of our house. If you are renting or have bought a house, you would know or notice landlord may change the lock between tenancy. And most likely, a new set of locks will be put in when you purchase a house. The reason is the pattern of the key blade (policy) is physically built into the lock (mechanism). Because this physical coupling, when policy changes, mechanism changes as well.

Okay, that's fine, but how does it translate to programming? Please read the [Rule Engine Pattern](https://enrose.github.io/c-sharp/rule-engine-pattern) article for further study.

As always thanks for reading please PR me any suggestions appreciated.