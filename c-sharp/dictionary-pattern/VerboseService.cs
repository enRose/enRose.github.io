using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
