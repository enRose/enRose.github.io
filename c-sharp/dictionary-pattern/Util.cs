using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barin.API.DictionaryPattern
{
    public class QueryResolver<ResponseData, QueryRoute>
            where QueryRoute : struct, IConvertible where ResponseData : class, new()
    {
        public Func<Task<ResponseData>> TryGetResolver(
            string queryRoute,
            Dictionary<QueryRoute, Func<Task<ResponseData>>> allResolvers
            )
        {
            queryRoute.ThrowErrorIfNil($"query name is empty");

            (Enum.TryParse(queryRoute, out QueryRoute query) == false)
                .ThrowErrorIfTrue($"Query is not supported: {queryRoute}");

            return Collection.TryGetFunc(allResolvers, query);
        }
    }
}
