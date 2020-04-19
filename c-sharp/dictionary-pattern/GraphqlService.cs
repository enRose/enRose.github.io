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

        public async Task<Data> GetBusinessDetails(Guid userId)
        {
            var companies = await Customer.GetLinkedCompanies(userId).ConfigureAwait(false);
            var companiesDetails = await Customer.GetCompanyDetails(companies).ConfigureAwait(false);

            return new Data
            {
                CompanyDetails = companiesDetails
            };
        }

        public async Task<Data> GetCustomerDetails(Guid userId)
        {
            var stpAppTask = STPOnline.GetApplication(userId);
            var homeAddressTask = Customer.GetHomeAddress(userId, Config.KeepInCacheInMinutes);
            var directContactTask = Customer.GetDirectContactNumber(userId, Config.KeepInCacheInMinutes);
            var primaryEmailTask = Customer.GetPrimaryEmailAddress(userId, Config.KeepInCacheInMinutes);

            await Task.WhenAll(
                homeAddressTask, 
                directContactTask, 
                primaryEmailTask, 
                stpAppTask
            ).ConfigureAwait(false);

            ApplicationDto app = await stpAppTask.ConfigureAwait(false);

            app.ContactInformation = app.ContactInformation.NullGuard();

            app.ContactInformation.HomeAddress = await homeAddressTask.ConfigureAwait(false);
            app.ContactInformation.DirectContactNumber = await directContactTask.ConfigureAwait(false);
            app.ContactInformation.PrimaryEmail = await primaryEmailTask.ConfigureAwait(false);

            return new Data
            {
                ApplicationDetails = app
            };
        }

        public async Task<Data> GetReferences(Guid userId)
        {
            return new Data
            {
                References = new ReferenceDto()
            };
        }

        public async Task<Data> GetSOP(Guid userId)
        {
            return new Data
            {
                StatementOfPosition = await StatementOfPosition.Get(userId).ConfigureAwait(false)
            };
        }
    }
}
