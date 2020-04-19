using Microsoft.Practices.Unity;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Barin.API.DictionaryPattern
{
    [RoutePrefix("users/{userId:Guid}/marvel-characters")]
    [JwtAuthorizationFilter(Roles = "Users")]
    public class MarvelController : ApiController
    {
        private readonly IQueryService queryService;
        private readonly IMutateService mutateService;
        private readonly IPostService postService;

        public ApplicationController(
          IQueryService getService, 
          IMutateService putService, 
          IPostService postService)
        {
            this.queryService = getService;
            this.mutateService = putService;
            this.postService = postService;
        }

        [SwaggerResponse(HttpStatusCode.OK, type: typeof(DataChunk))]
        [HttpGet, Route(template: "query/{character}", Name="Query")]
        public async Task<IHttpActionResult> Query(Guid userId, string character)
        {
            return Ok(await getService.Query(userId, character));
        }

        [SwaggerResponse(HttpStatusCode.NoContent)]
        [HttpPut, Route(template: "mutate/{character}", Name="Mutate")]
        [InputParameterRequiredFilter]
        public async Task<IHttpActionResult> Mutate(
          Guid userId, [FromBody]CharacterDto c)
        {
            await putService.Mutate(userId, c);
            return Ok();
        }

        [SwaggerResponse(HttpStatusCode.Created)]
        [HttpPost, Route(template: "create", Name="Create")]
        public async Task<IHttpActionResult> Create(
          Guid userId, [FromBody]CharacterDto c)
        {
            return Created(await postService.Create(userId, c), "Created");
        }
    }
}
