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
        Task<Data> Get(Guid userId, string queryRoute);
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

            var func = new QueryResolver<Data, QueryRoute>()
              .TryGetResolver(
                queryRoute, Resolvers
            );

            var data = await func().ConfigureAwait(false);

            data.Eligibility = CustomerValidity.OK;

            return data;
        }

        public async Task<Data> GetSpiderMan()
        {
            return new Data
            {
                CardProduct = await CardProducts.GetVBR(Config.KeepInCacheInMinutes).ConfigureAwait(false)
            };
        }
    }
}
