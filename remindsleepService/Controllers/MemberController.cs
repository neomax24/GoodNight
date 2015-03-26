using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using remindsleepService.DataObjects;
using remindsleepService.Models;

namespace remindsleepService.Controllers
{
    public class MemberController : TableController<Member>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            remindsleepServiceContext context = new remindsleepServiceContext();
            DomainManager = new EntityDomainManager<Member>(context, Request, Services);
        }

        // GET tables/Member
        public IQueryable<Member> GetAllMember()
        {
            return Query(); 
        }

        // GET tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Member> GetMember(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Member> PatchMember(string id, Delta<Member> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Member
        public async Task<IHttpActionResult> PostMember(Member item)
        {
            Member current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMember(string id)
        {
             return DeleteAsync(id);
        }

    }
}