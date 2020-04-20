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
