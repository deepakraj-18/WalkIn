using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    //[Authorize(Roles = "Admin")]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class CanditateController : ControllerBase
    {
        private readonly SharePointService _sharePointService;

        public CanditateController(SharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }

        #region
        [HttpGet]
        [Route("GetAllCanditates")]
        public dynamic GetCanditates()
        {
            var canditateList = _sharePointService.GetAllCanditates();
            List<ViewCanditateModel> canditates = new List<ViewCanditateModel>();

            foreach (var c in canditateList)
            {
                string id = c["ID"].ToString();
                string name = c["Title"].ToString();
                string email = c["Email"].ToString();
                string phoneNumber = c["PhoneNumber"].ToString();
                string scoreOne = c["ScoreOne"].ToString() ?? "0";
                string scoreTwo = c["ScoreTwo"].ToString() ?? "0";
                var result = c["Result"];

                canditates.Add(new ViewCanditateModel()
                {
                    ID = id,
                    Name = name,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    ScoreOne = scoreOne,
                    ScoreTwo = scoreTwo,
                    Result=Convert.ToBoolean(result)
                });
            }

            return Ok(canditates);
        }

        #endregion


        #region
        [HttpPost]
        [Route("GetCanditatesByInteviewID")]
        public dynamic GetCanditatesByInteviewId([FromBody] GetCanditateByInteviewIdModel model)
        {
            if (model.InterviewId == null)
            {
                return BadRequest("Interview Id is mandatory");
            }
            var response = _sharePointService.GetCanditatesByInteviewId(model.InterviewId, model.InterviewDate);
            var canditateList = new System.Collections.Generic.List<CanditateRegistrationModel>();
            foreach (var c in response)
            {
                var canditates = new CanditateRegistrationModel
                {
                    Name = c["Name"].ToString(),
                    Email = c["Email"].ToString(),
                    PhoneNumber = c["PhoneNumber"].toString(),
                    City = c["City"].toString(),
                    Institute = c["Institute"].toString(),
                    Technology = c["Technology"].toString(),
                    Experience = c["Experience"].toString(),
                    Certification = c["Certification"].toString(),
                    Skills = c["Skills"].toString(),
                    Source = c["Source"].toString(),
                    Reference = c["Reference"].toString(),
                    Degree = c["Degree"].toString(),
                    Gender = c["Gender"].toString(),

                };
                canditateList.Add(canditates);

            }

            return canditateList;

        }
        #endregion


        #region
        [HttpPost]
        [Route("GetCanditateByEmail")]
        public dynamic GetCanditateByEmail([FromBody] GetCanditateByEmailModel model)
        {
            if (model == null || model.Email == null)
            {
                return BadRequest("Email is mandatory");
            }
            var response = _sharePointService.getCanditateByEmail(model.Email);
            if (response == null)
            {
                return "Canditate doesn't exists";
            }
            var data = response[0];
            var cantitate = new ViewCandidateModel
            {
                ID = data["ID"] ? data["ID"] : "",
                Name = data["Title"] ? data["Title"] : "",
                Email = data["Email"] ? data["Email"] : "",
                PhoneNumber = data["PhoneNumber"] ? data["PhoneNumber"] : "",
                ScoreOne = data["ScoreOne"] ? data["SCoreOne"] : "",
                ScoreTwo = data["ScoreTwo"] ? data["ScoreTwo"] : ""

            };

            return cantitate;
        }
        #endregion

        #region
        [HttpPost]
        [Route("GetCanditateById")]
        public dynamic GetCanditateById([FromBody] GetCanditateByIdModel model)
        {
            if (model == null || model.Id == null)
            {
                return BadRequest("Canditate is mandatory");
            }
            var response = _sharePointService.getCanditateByID(model.Id);
            if (response.Count != 0)
            {
                var data = response[0];
                var answer = _sharePointService.getAnswers(data["Email"], data["InterviewID"]);
                if (response == null || response.Count == 0)
                {
                    return BadRequest("Canditate doesn't exists");
                }
                var cantitate = new ViewCandidateModel();
                List<ViewAnswerModel> answersList = new();

                cantitate.ID = data["ID"] != null ? Convert.ToInt32(data["ID"]) : 0;

                //cantitate.ID = data["ID"] ? Convert.ToInt32(data["ID"]) : 0;
                cantitate.Name = data["Title"] != null ? Convert.ToString(data["Title"]) : "";
                cantitate.Email = data["Email"] != null ? Convert.ToString(data["Email"]) : "";
                cantitate.PhoneNumber = data["PhoneNumber"] != null ? Convert.ToString(data["PhoneNumber"]) : "";
                cantitate.ScoreOne = data["ScoreOne"] != null ? Convert.ToString(data["ScoreOne"]) : "";
                cantitate.ScoreTwo = data["ScoreTwo"] != null ? Convert.ToString(data["ScoreTwo"]) : "";
                cantitate.City = data["City"] != null ? Convert.ToString(data["City"]) : "";
                cantitate.Institute = data["Institute"] != null ? Convert.ToString(data["Institute"]) : "";
                cantitate.Technology = data["Technology"] != null ? Convert.ToString(data["Technology"]) : "";
                cantitate.Experience = data["Experience"] != null ? Convert.ToString(data["Experience"]) : "";
                cantitate.Certification = data["Certification"] != null ? Convert.ToString(data["Certification"]) : "";
                cantitate.Skills = data["Skills"] != null ? Convert.ToString(data["Skills"]) : "";
                cantitate.Source = data["Source"] != null ? Convert.ToString(data["Source"]) : "";
                cantitate.Reference = data["OthersReference"] != null ? Convert.ToString(data["OthersReference"]) : "";
                cantitate.Degree = data["Degree"] != null ? Convert.ToString(data["Degree"]) : "";
                cantitate.Gender = data["Gender"] != null ? Convert.ToString(data["Gender"]) : "";
                cantitate.InterviewID = data["InterviewID"] != null ? Convert.ToString(data["InterviewID"]) : "";
                cantitate.Result = data["Result"] != null ? data["Result"] : false;
                if (answer!=null)
                {
                    foreach (var a in answer)
                    {
                        var ans = new ViewAnswerModel();
                        ans.QuestionId = a["QuestionId"] != null ? a["QuestionId"] : "";
                        ans.Question = a["Question"] != null ? a["Question"] : "";
                        ans.Answer = a["Answer"] != null ? a["Answer"] : "";
                        ans.SubmittedAnswer = a["SubmittedAnswer"] != null ? a["SubmittedAnswer"] : "";
                        answersList.Add(ans);
                    }
                }
                cantitate.Answers = answersList;
                return Ok(cantitate);
            }
            else
            {
                return BadRequest("Canditate doesn't exists");
            }

        }
        #endregion

    }
}
