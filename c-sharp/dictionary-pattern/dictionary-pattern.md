# Dictionary Pattern

## Motivation

The idea of this pattern came to me when I first read Robert Martin's book Clean Code. In there it explains why if statements are considered harmful and suggests when there are an awful amount of if statements in our code, it usually indicates the underlying nature of the problem we try to solve is polymorphic.

I am not going to discuss why if statements considered harmful in this post, you can find an excellent post by Jetbrains down below in Reference section. I do see the point as in why excessive amount of if statements are a terrible situation to be in. I don't however, quite agree that polymorphism underpins the problem at hand. Sometimes we could indeed refactor out of this if-inception by polymorphism -- subtype or inclusion polymorphism to be specific. Other times I feel this is too heavy handed because some scenarios are just too simple to warrant subtype polymorphism which is one of the strongest couplings in OO and it is not hard to get wrong! 

Let's take a look at this example, not real but not far from real, I have seen code exactly like this.

```
private string GetQueryParameterBy(string token)
{
    string paramValue = null;

    if (string.Equals(token, QueryStringConstants.Thanos,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetThanos();
    }
    else if (string.Equals(token, QueryStringConstants.SpiderMan,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetSpiderMan();
    }
    else if (string.Equals(token, QueryStringConstants.IronMan,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetIronMan();
    }
    else if (string.Equals(token, QueryStringConstants.CaptainAmerica,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetCaptainAmerica();
    }
    else if (string.Equals(token, QueryStringConstants.WandaMaximoff,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetWandaMaximoff();
    }
    else if (string.Equals(token, QueryStringConstants.Thor,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetThor();
    }
    else if (string.Equals(token, QueryStringConstants.Hulk,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetHulk();
    }
    else if (string.Equals(token, QueryStringConstants.Deadpool,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetDeadpool();
    }

    return paramValue;
}
```

It is not that bad and simple to understand, but can we do better? 

## Dictionary Pattern

Yes we can! It is dictionary!

The same code above can be rewritten as below and see how small it is!

```
private string GetQueryParameterBy(string token)
{
    var superHeroMap = new Dictionary<string, Func<string>> {
        { QueryStringConstants.SpiderMan.ToLower(), GetSpiderMan },
        { QueryStringConstants.Thanos.ToLower(), GetThanos },
        { QueryStringConstants.IronMan.ToLower(), GetIronMan },
        { QueryStringConstants.CaptainAmerica.ToLower(), GetCaptainAmerica },
        { QueryStringConstants.WandaMaximoff.ToLower(), GetWandaMaximoff },
        { QueryStringConstants.Hulk.ToLower(), GetHulk },
        { QueryStringConstants.Deadpool.ToLower(), GetDeadpool },
    };
    
    superHeroMap.TryGetValue(token, out var paramValue)
    
    return paramValue;
}
```


## Reference

Code Smells: If Statements[https://blog.jetbrains.com/idea/2017/09/code-smells-if-statements/]

Polymorphism without Inheritence[https://stackoverflow.com/questions/11732422/is-polymorphism-possible-without-inheritance]

On Understanding Types, Data Abstraction, and Polymorphism[http://lucacardelli.name/Papers/OnUnderstanding.A4.pdf]
