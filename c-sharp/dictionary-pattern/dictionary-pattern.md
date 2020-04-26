# Dictionary Pattern

## Motivation

The idea of this pattern came to me when I first read Robert Martin's book Clean Code. In there it explains why if statements are considered harmful and suggests when there are an awful amount of if statements in our code, it usually indicates the underlying nature of the problem we try to solve is polymorphic.

I am not going to discuss why if statements considered harmful in this post, you can find an excellent post by Jetbrains down below in Reference section. I do see the point as in why excessive amount of if statements are a terrible situation to be in. I don't however, quite agree that polymorphism underpins the problem at hand. Sometimes we could indeed refactor out of this if-inception by polymorphism -- subtype or inclusion polymorphism to be specific. Other times I feel this is too heavy handed because some scenarios are just too simple to warrant subtype polymorphism which is one of the strongest couplings in OO and it is not hard to get wrong! 

Let's take a look at this example, not real but not far from real, I have seen code exactly like this.

```C#
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

```C#
private string GetQueryParameterBy(string token)
{
    var comparer = StringComparer.OrdinalIgnoreCase;

    var superHeroMap = new Dictionary<string, Func<string>>(comparer) {
        { QueryStringConstants.SpiderMan, GetSpiderMan },
        { QueryStringConstants.Thanos, GetThanos },
        { QueryStringConstants.IronMan, GetIronMan },
        { QueryStringConstants.CaptainAmerica, GetCaptainAmerica },
        { QueryStringConstants.WandaMaximoff, GetWandaMaximoff },
        { QueryStringConstants.Hulk, GetHulk },
        { QueryStringConstants.Deadpool, GetDeadpool },
    };
    
    if (superHeroMap.TryGetValue(token, out var paramValue))
    {
        return paramValue();
    }
    
    return null;
}
```

We fold knowledge into a clever data structure so our program can be as dumb as possible. We can use any data structure that fits our purpose. Quite often, I find myself using a dictionary to encapsulate knowledge therefore I call it Dictionary Pattern. 

Dictionary pattern fundamentally is about correlation, it relates two things together, i.e. if it is A then I want X, if it is B then I want Y, etc. 

Let's look at another example which again is based off some real production code I have seen. 

I have seen some blog posts comparing Rest API vs Graphql claims Rest API has one endpoint per resource whereas Graphql only has one endpoint for all. This is simply not true. To start with, Graphql has two endpoints: query and mutation. Query for Get and mutation for both put and post as Rest equivalence. As for Rest API, indeed, most people would have three endpoints(GET/PUT/POST) for each every ressource, so if we have three resources, we could have up to 9 endpoints. However, we don't have to, we can use Dictionary Pattern to reduce the amount of endpoints to maximum 3, one for get, one for put and one for post.

Here is what a verbose Rest controller would look like. Every single resource has its own GET route.

```C#
using Microsoft.Practices.Unity;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Barin.API.DictionaryPattern
{
    [RoutePrefix("fans/{userId:Guid}/marvel-characters")]
    [JwtAuthorizationFilter(Roles = "Users")]
    public class MarvelController : ApiController
    {
        private readonly IMarvelService marvelService;

        public MarvelController(IMarvelService s)
        {
            marvelService = s;
        }

        [HttpGet, Route(template: "spider-man", Name="spider-man")]
        public async Task<IHttpActionResult> SpiderMan(Guid userId)
        {
            return Ok(await marvelService.GetSpiderMan(Guid userId));
        }

        [HttpGet, Route(template: "thanos", Name="thanos")]
        public async Task<IHttpActionResult> Thanos(Guid userId)
        {
            return Ok(await marvelService.GetThanos(Guid userId));
        }

        [HttpGet, Route(template: "iron-man", Name="iron-man")]
        public async Task<IHttpActionResult> IronMan(Guid userId)
        {
            return Ok(await marvelService.GetIronMan(Guid userId));
        }

        [HttpGet, Route(template: "captain-america", Name="captain-america")]
        public async Task<IHttpActionResult> CaptainAmerica(Guid userId)
        {
            return Ok(await marvelService.GetCaptainAmerica(Guid userId));
        }

        [HttpGet, Route(template: "wanda-maximoff", Name="wanda-maximoff")]
        public async Task<IHttpActionResult> WandaMaximoff(Guid userId)
        {
            return Ok(await marvelService.GetWandaMaximoff(Guid userId));
        }

        [HttpGet, Route(template: "thor", Name="thor")]
        public async Task<IHttpActionResult> Thor(Guid userId)
        {
            return Ok(await marvelService.GetThor(Guid userId));
        }
    }
}
```

The service that serves up the controller. Quite often we need to do some prerequisite check before we serve up the data e.g. check if customer is under aged, check if customer subscription has expired, etc. In this approach, which I have seen a lot in real production code, we need to repeat this prerequisite check for each every one of the endpoints we serve. Like I mentioned earlier if we have 3 resources we do GET/PUT/POST on, we could have up to 9 endpoints, so we could repeat this check up to 9 times! This is a huge waste should be reduced!

```C#
namespace Barin.API.DictionaryPattern
{
    public interface IVerboseService
    {
        Task<SpiderManDto> GetSpiderMan(Guid userId);
        Task<ThanosDto> GetThanos(Guid userId);
        Task<IronManDto> GetIronMan(Guid userId);
        Task<CaptainAmericaDto> GetCaptainAmerica(Guid userId);
        Task<WandaMaximoffDto> GetWandaMaximoff(Guid userId);
        Task<ThorDto> GetThor(Guid userId);
    }

    public class VerboseService : IVerboseService
    {
        private readonly ICore core;
        
        public VerboseService(ICore core)
        {
            this.core = core;
        }

        public async Task<SpiderManDto> GetSpiderMan(Guid userId)
        {
            var user = await core.User.GetBy(userId).ConfigureAwait(false);

            var eligibility = Eligibility.Check(user.Dob, user.Subscription);

            if (eligibility != UserEligibility.Ok)
            {
                core.Analytics.ReportUserStatus(
                    "GetSpiderMan",
                    eligibility
                );

                return new SpiderManDto
                {
                    UserEligibility = eligibility
                };
            }

            var spiderMan = await core
              .Repository.MarvelCharactors
              .Get(Superhero.SpiderMan).ConfigureAwait(false);

            var weapons = await core
              .Weaponary.Earth.StarkIndustries
              .GetFor(Superhero.SpiderMan).ConfigureAwait(false);

            spiderMan.StarkSuit = weapons.StarkSuit;

            return new SpiderManDto
            {
                UserEligibility = eligibility,
                SpiderMan = spiderMan
            };
        }

        public async Task<ThanosDto> GetThanos(Guid userId)
        {
            var user = await core.User.GetBy(userId).ConfigureAwait(false);

            var eligibility = Eligibility.Check(user.Dob, user.Subscription);

            if (eligibility != UserEligibility.Ok)
            {
                core.Analytics.ReportUserStatus(
                    "GetThanos",
                    eligibility
                );

                return new ThanosDto
                {
                    UserEligibility = eligibility
                };
            }

            var thanos = await core
              .Repository.MarvelCharactors
              .Get(SuperVillains.Thanos).ConfigureAwait(false);

            var weapons = await core
              .Weaponary.Titan
              .MakeFrom(Substance.Adamantium).ConfigureAwait(false);

            thanos.DoubleEdgedSword = weapons.DoubleEdgedSword;

            return new ThanosDto
            {
                UserEligibility = eligibility,
                Thanos = thanos
            };
        }

        public async Task<IronManDto> GetIronMan(Guid userId)
        {
            var user = await core.User.GetBy(userId).ConfigureAwait(false);

            var eligibility = Eligibility.Check(user.Dob, user.Subscription);

            if (eligibility != UserEligibility.Ok)
            {
                core.Analytics.ReportUserStatus(
                    "GetIronMan",
                    eligibility
                );

                return new IronManDto
                {
                    UserEligibility = eligibility
                };
            }

           var ironMan = await core
              .Repository.MarvelCharactors
              .Get(Superhero.IronMan).ConfigureAwait(false);

            var classicGrayArmor = await core
              .Weaponary.Earth.StarkIndustries
              .GetArmor(
                Superhero.IronMan, 
                Superhero.IronMan.Era.ClassicGrayArmor).ConfigureAwait(false);

            ironMan.Armor = classicGrayArmor;

            return new IronManDto
            {
                UserEligibility = eligibility,
                IronMan = ironMan
            };
        }

        public async Task<CaptainAmericaDto> GetCaptainAmerica(Guid userId)
        {
            var user = await core.User.GetBy(userId).ConfigureAwait(false);

            var eligibility = Eligibility.Check(user.Dob, user.Subscription);

            if (eligibility != UserEligibility.Ok)
            {
                core.Analytics.ReportUserStatus(
                    "GetCaptainAmerica",
                    eligibility
                );

                return new CaptainAmericaDto
                {
                    UserEligibility = eligibility
                };
            }

           var captainAmerica = await core
              .Repository.MarvelCharactors
              .Get(Superhero.CaptainAmerica).ConfigureAwait(false);

            var shield = await core
              .Weaponary.Earth
              .MakeFrom(Substance.VibraniumSteel).ConfigureAwait(false);

            captainAmerica.Shield = shield;

            return new CaptainAmericaDto
            {
                UserEligibility = eligibility,
                CaptainAmerica = captainAmerica
            };
        }

        public async Task<WandaMaximoffDto> GetWandaMaximoff(Guid userId)
        {
            var user = await core.User.GetBy(userId).ConfigureAwait(false);

            var eligibility = Eligibility.Check(user.Dob, user.Subscription);

            if (eligibility != UserEligibility.Ok)
            {
                core.Analytics.ReportUserStatus(
                    "GetWandaMaximoff",
                    eligibility
                );

                return new WandaMaximoffDto
                {
                    UserEligibility = eligibility
                };
            }

           var wandaMaximoff = await core
              .Repository.MarvelCharactors
              .Get(Superhero.WandaMaximoff).ConfigureAwait(false);

            var chaosMagic = await core
              .Weaponary.Earth.Mutant
              .Get(Power.ChaosMagic).ConfigureAwait(false);

            wandaMaximoff.ChaosMagic = chaosMagic;

            return new WandaMaximoffDto
            {
                UserEligibility = eligibility,
                WandaMaximoff = wandaMaximoff
            };
        }

        public async Task<ThorDto> GetThor(Guid userId)
        {
            var user = await core.User.GetBy(userId).ConfigureAwait(false);

            var eligibility = Eligibility.Check(user.Dob, user.Subscription);

            if (eligibility != UserEligibility.Ok)
            {
                core.Analytics.ReportUserStatus(
                    "GetThor",
                    eligibility
                );

                return new ThorDto
                {
                    UserEligibility = eligibility
                };
            }

           var thor = await core
              .Repository.MarvelCharactors
              .Get(Superhero.Thor).ConfigureAwait(false);

            var stormbreaker = await core
              .Weaponary.Nidavellir
              .Get(Substance.Uru).ConfigureAwait(false);

            thor.Stormbreaker = stormbreaker;

            return new ThorDto
            {
                UserEligibility = eligibility,
                Thor = thor
            };
        }
    }
}
```

Let's apply Dictionary Pattern. I only demonstrate GET, but the same principle goes for PUT and POST.

```C#
namespace Barin.API.DictionaryPattern
{
    [RoutePrefix("users/{userId:Guid}/marvel-characters")]
    [JwtAuthorizationFilter(Roles = "Users")]
    public class MarvelController : ApiController
    {
        private readonly IQueryService queryService;
        
        public ApplicationController(IQueryService getService)
        {
            this.queryService = getService;
        }

        [SwaggerResponse(HttpStatusCode.OK, type: typeof(DataChunk))]
        [HttpGet, Route(template: "query/{character}", Name="Query")]
        public async Task<IHttpActionResult> Query(Guid userId, string character)
        {
            return Ok(await getService.Query(userId, character));
        }
    }
}
```

In service, we create a Resolvers of dictionary<Enum, Func> type. Its key is the string enum of a route name, and the value is a function delegate that serves that particular route.

We are able to reduce controller code quite a bit. And although the amount of code in the service has been reduced but not as significant as in controller. The reason is, just as in graphql's resolvers, each graphql resolver has to be specifically written out to serve a particular schema query or mutation, so does Rest API. There is no free lunch, someone, and in somewhere has to do the work!

```C#

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barin.API.DictionaryPattern
{
    public enum QueryRoute
    {
        SpiderMan,
        Thanos,
        IronMan,
        CaptainAmerica,
        WandaMaximoff,
        Thor
    }

    public interface IQueryService
    {
        Task<Data> Query(Guid userId, string queryRoute);
    }

    public class QueryService : BaseService, IQueryService
    {
        public Dictionary<QueryRoute, Func<Task<Data>>> Resolvers =>
            new Dictionary<QueryRoute, Func<Task<Data>>> {
                { QueryRoute.SpiderMan, GetSpiderMan },
                { QueryRoute.Thanos, GetThanos },
                { QueryRoute.IronMan, GetIronMan },
                { QueryRoute.CaptainAmerica, GetCaptainAmerica },
                { QueryRoute.WandaMaximoff, GetWandaMaximoff },
                { QueryRoute.Thor, GetThor },
            };

        public async Task<Data> Query(Guid userId, string queryRoute)
        {
            var eligibility = await CheckElligibility(userId).ConfigureAwait(false);

            if (eligibility != Validity.OK) {
                Analytics.LogUserStatus(
                    queryRoute,
                    eligibility
                );
               
                return new Data { Eligibility = eligibility };
            }

            var resovlerFunc = new QueryResolver<Data, QueryRoute>()
              .TryGetResolver(
                queryRoute, Resolvers
            );

            var data = await resovlerFunc().ConfigureAwait(false);

            data.Eligibility = CustomerValidity.OK;

            return data;
        }

        public async Task<Data> GetSpiderMan()
        {
            var spiderMan = await core
              .Repository.MarvelCharactors
              .Get(Superhero.SpiderMan).ConfigureAwait(false);

            var weapons = await core
              .Weaponary.Earth.StarkIndustries
              .GetFor(Superhero.SpiderMan).ConfigureAwait(false);

            spiderMan.StarkSuit = weapons.StarkSuit;

            return new Data {
                SpiderMan = spiderMan
            };
        }

        public async Task<Data> GetThanos()
        {
            var thanos = await core
              .Repository.MarvelCharactors
              .Get(SuperVillains.Thanos).ConfigureAwait(false);

            var weapons = await core
              .Weaponary.Titan
              .MakeFrom(Substance.Adamantium).ConfigureAwait(false);

            thanos.DoubleEdgedSword = weapons.DoubleEdgedSword;

            return new Data
            {
                Thanos = thanos
            };
        }

        public async Task<Data> GetIronMan()
        {
            var ironMan = await core
              .Repository.MarvelCharactors
              .Get(Superhero.IronMan).ConfigureAwait(false);

            var classicGrayArmor = await core
              .Weaponary.Earth.StarkIndustries
              .GetArmor(
                Superhero.IronMan,
                Superhero.IronMan.Era.ClassicGrayArmor).ConfigureAwait(false);

            ironMan.Armor = classicGrayArmor;

            return new Data
            {
                IronMan = ironMan
            };
        }

        public async Task<Data> GetCaptainAmerica()
        {
            var captainAmerica = await core
              .Repository.MarvelCharactors
              .Get(Superhero.CaptainAmerica).ConfigureAwait(false);

            var shield = await core
              .Weaponary.Earth
              .MakeFrom(Substance.VibraniumSteel).ConfigureAwait(false);

            captainAmerica.Shield = shield;

            return new Data
            {
                CaptainAmerica = captainAmerica
            };
        }

        public async Task<Data> GetWandaMaximoff()
        {
            var wandaMaximoff = await core
              .Repository.MarvelCharactors
              .Get(Superhero.WandaMaximoff).ConfigureAwait(false);

            var chaosMagic = await core
              .Weaponary.Earth.Mutant
              .Get(Power.ChaosMagic).ConfigureAwait(false);

            wandaMaximoff.ChaosMagic = chaosMagic;

            return new Data
            {
                WandaMaximoff = wandaMaximoff
            };
        }

        public async Task<Data> GetThor()
        {
            var thor = await core
              .Repository.MarvelCharactors
              .Get(Superhero.Thor).ConfigureAwait(false);

            var stormbreaker = await core
              .Weaponary.Nidavellir
              .Get(Substance.Uru).ConfigureAwait(false);

            thor.Stormbreaker = stormbreaker;

            return new Data
            {
                Thor = thor
            };
        }
    }
}
```

With Dictionary Pattern applied however, what we get:

A. much cleaner, smaller controller.

B. a clean separate between route relaying and computation logic in the service layer.

C. a gateway that we can apply common logics or patterns e.g. security check, data sanitisation, customer eligibility check, etc.

D. noises have been moved to gateway therefore computation units/methods now are more focused.    

```C#
// A gateway to all query routes:
public async Task<Data> Query(Guid userId, string queryRoute)
{
    var eligibility = await CheckElligibility(userId).ConfigureAwait(false);

    if (eligibility != Validity.OK) {
        Analytics.LogUserStatus(
            queryRoute,
            eligibility
        );

        return new Data { Eligibility = eligibility };
    }

    var resovlerFunc = new QueryResolver<Data, QueryRoute>()
      .TryGetResolver(
        queryRoute, Resolvers
    );

    var data = await resovlerFunc().ConfigureAwait(false);

    data.Eligibility = CustomerValidity.OK;

    return data;
}
```


## Reference

Code Smells: If Statements[https://blog.jetbrains.com/idea/2017/09/code-smells-if-statements/]

Polymorphism without Inheritance[https://stackoverflow.com/questions/11732422/is-polymorphism-possible-without-inheritance]

On Understanding Types, Data Abstraction, and Polymorphism[http://lucacardelli.name/Papers/OnUnderstanding.A4.pdf]
