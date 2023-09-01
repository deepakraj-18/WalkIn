using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    //[Authorize(Roles = "Admin")]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly SharePointService _sharePointService;

        public InterviewController( SharePointService sharePointService)
        {
            _sharePointService = sharePointService;

        }


        #region Demo
        [HttpGet]
       public string Get() { 
            return "Hi";
        }
        #endregion


        #region //Get All Interviews
        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// This method returns all the interviews in the database except the deleted one.
        /// </returns>
        [HttpGet]
        [Route("GetAllInterviews")]
        public dynamic GetInterviews()
        {
            ListItemCollection interviews = _sharePointService.GetAllInterviews();
            return interviews;
        }
        #endregion



        #region // Create a interview
        [HttpPost]
        [Route("CreateInterview")]
        public ListItem GetInterviews([FromBody] InterViewRegistrationModel model)
        {
            ListItem interview = _sharePointService.CreateInterview(model);
            return interview;
        }
        #endregion



    }
}
