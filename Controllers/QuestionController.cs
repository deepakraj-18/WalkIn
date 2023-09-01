using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;
using TechnorucsWalkInAPI.Helpers;

namespace TechnorucsWalkInAPI.Controllers
{
    //[Authorize(Roles = "Admin")]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : Controller
    {
        private readonly SharePointService _sharePointService;

        public QuestionController(SharePointService sharePointService)
        {
            _sharePointService = sharePointService;

        }

        #region //Add Question
        ///<summary>
        ///This method is to add question 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("AddQuestion")]
        public dynamic AddQuestion()
        {
            ListItemCollection questions =_sharePointService.AddQuestion();
            return questions;
        }
        #endregion


        #region //
        #endregion
    }
}
