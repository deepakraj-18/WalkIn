using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechnorucsWalkInAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : Controller
    {


        [HttpGet]
        [Route("GetAllInterviews")]
        public dynamic GetAllInterviews()
        {

            return"All Inteviews";
        }
    }
}
