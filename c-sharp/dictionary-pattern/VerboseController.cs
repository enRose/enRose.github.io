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
