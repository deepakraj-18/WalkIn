using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.News.DataModel;
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

        public InterviewController(SharePointService sharePointService)
        {
            _sharePointService = sharePointService;

        }


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
            if (interviews == null)
            {
                return BadRequest("Please add a interview");
            }
            List<InterViewRegistrationModel> interviewList = interviews.Select(x => new InterViewRegistrationModel
            {
                ID = x["ID"].ToString(),
                Date = DateOnly.Parse(x["Title"].ToString()),
                Scoreone = x["ScoreOne"].ToString(),
                Scoretwo = x["ScoreTwo"].ToString()

            }).ToList();
            return interviewList;
        }
        #endregion



        #region // Create a interview
        [HttpPost]
        [Route("CreateInterview")]
        public dynamic CreateInterview([FromBody] InterViewRegistrationModel model)
        {
            ListItem interviewItems = _sharePointService.CreateInterview(model);

            List<InterViewRegistrationModel> interviews = new()
            {
    new InterViewRegistrationModel
    {
        ID = interviewItems["InterviewId"].ToString(),
        Date = DateOnly.Parse(interviewItems["Title"].ToString()),
        Scoreone = interviewItems["ScoreOne"].ToString(),
        Scoretwo = interviewItems["ScoreTwo"].ToString()
    },
};
            return interviews;
        }
        #endregion




        #region //Update Interview
        [HttpPost(Name = "Edit")]
        public List<InterViewUpdateModel> EditInterview([FromBody] InterViewUpdateModel model)
        {
            ListItem editedInterview = _sharePointService.EditInterview(model);
            List<InterViewUpdateModel> response = new()
            {
    new InterViewUpdateModel
    {
        Date = DateOnly.Parse(editedInterview["Title"].ToString()),
        Scoreone = editedInterview["ScoreOne"].ToString(),
        Scoretwo = editedInterview["ScoreTwo"].ToString()
    },
};
            return response;
        }
        #endregion


        #region//Delete Interview
        [HttpPost(Name ="Delete")]
        public string DeleteInteview()
        {
            return null;
        }
        #endregion



    }
}
