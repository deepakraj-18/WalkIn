using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    //[Authorize(Roles = "Admin")]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
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
        [HttpPost]
        [Route("AddQuestion")]
        public dynamic AddQuestion([FromBody] InterviewModel questions)
        {
            var result = false;
            foreach (var question in questions.Questions)
            {
                result = _sharePointService.AddQuestion(question,questions.InterviewID,questions.PatternType);
                if(!result)
                {
                    return BadRequest("Failed");
                }
            }

            return Ok("Questions Added Succesfully");
        }
        #endregion


        #region //Get Columns
        [HttpGet]
        [Route("GetColumns")]
        public dynamic GetListColumns()
        {
            var response=_sharePointService
                .GetListColumns();
            return response;
        }
        #endregion


        #region //
        #endregion
    }
}
