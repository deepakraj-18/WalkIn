using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.News.DataModel;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public ActionResult GetInterviews()
        {
            try
            {

                ListItemCollection interviews = _sharePointService.GetAllInterviews();
                if (interviews == null || interviews.Count == 0)
                {
                    return BadRequest("No interviews found.");
                }

                List<InterViewRegistrationModel> interviewList = new List<InterViewRegistrationModel>();
                foreach (var x in interviews)
                {
                    string interviewID = x["InterviewId"] != null ? x["InterviewId"].ToString() : "";
                    string id = x["ID"] != null ? x["ID"].ToString() : "";
                    string title = x["Title"] != null ? x["Title"].ToString() : "";
                    string scoreOne = x["ScoreOne"] != null ? x["ScoreOne"].ToString() : "";
                    string scoreTwo = x["ScoreTwo"] != null ? x["ScoreTwo"].ToString() : "";


                    interviewList.Add(new InterViewRegistrationModel
                    {
                        ID = id,
                        InterviewId = interviewID,
                        Date = !string.IsNullOrEmpty(title.ToString()) ? DateOnly.ParseExact(title.ToString(), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture) : default,
                        Scoreone = scoreOne,
                        Scoretwo = scoreTwo
                    });

                }

                return Ok(interviewList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion


        #region
        [HttpPost("GetInterviewById")]
        public dynamic GetInterviewById([FromBody] GetInterviewByIdModel model)
        {
            var response = _sharePointService.GetInterviewById
                (model);
            if (response == null || response[0] == null)
            {
                return BadRequest("Interview Not Found");
            }
            List<GetInterviewByIdResponseModel> interviews = new()
            {
                new GetInterviewByIdResponseModel()
                {
                    ID=response[0]["ID"].ToString(),
                    Date =response[0]["Title"].ToString(),
                    Scoreone=(string) response[0]["ScoreOne"],
                    Scoretwo=(string) response[0]["ScoreTwo"],
                    InterviewId=(string)response[0]["InterviewId"]
                }
            };

            return Ok(interviews);
        }
        #endregion



        #region // Create a interview
        [HttpPost]
        [Route("CreateInterview")]
        public dynamic CreateInterview([FromBody] InterViewRegistrationModel model)
        {
            ListItem interviewItems = _sharePointService.CreateInterview(model);

            List<InterViewResponseModel> interviews = new()
            {
                new InterViewResponseModel
                        {
                         ID = interviewItems["InterviewId"].ToString(),
                         Date = interviewItems["Title"].ToString(),
                         Scoreone = interviewItems["ScoreOne"].ToString(),
                         Scoretwo = interviewItems["ScoreTwo"].ToString(),
                },
            };
            return Ok(interviews);
        }
        #endregion




        #region //Update Interview
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Edit")]

        public List<InterViewEditResponseModel> EditInterview([FromBody] InterViewUpdateModel model)
        {
            ListItem editedInterview = _sharePointService.EditInterview(model);
            List<InterViewEditResponseModel> response = new()
            {
                 new InterViewEditResponseModel
                 {
                     Date = editedInterview["Title"].ToString(),
                     Scoreone = editedInterview["ScoreOne"].ToString(),
                     Scoretwo = editedInterview["ScoreTwo"].ToString()
                 },
            };
            return response;
        }
        #endregion


        #region//Delete Interview
        [HttpPost]
        [Route("Delete")]
        public ActionResult<string> DeleteInteview([FromBody] InterViewDeleteModel model)
        {
            var isDeleted = _sharePointService.DeleteInterview(model);
            if (isDeleted)
            {
                return Ok("Interview Delete Successfully");
            }
            return BadRequest("Operation Failed");
        }
        #endregion



    }
}
