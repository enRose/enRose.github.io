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

            return new Data() {
                SpiderMan = spiderMan
            };
        }

    }
}
