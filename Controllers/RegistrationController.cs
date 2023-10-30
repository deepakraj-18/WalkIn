 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnorucsWalkInAPI.Models;
using TechnorucsWalkInAPI.Helpers;
using Microsoft.SharePoint.Client;
using System;
using Microsoft.Graph;
using System.Globalization;

namespace TechnorucsWalkInAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {

        private readonly SharePointService _sharePointService;
        public RegistrationController(SharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }


        #region //Admin Registration
        [HttpPost]
        [Route("Admin")]
        public dynamic AdminRegistration([FromBody] AdminRegisterModel model)
        {

            if (model == null || model.Email == null || model.Password == null || model.Name == null)
            {
                return "Please give all the required details";
            }
            try
            {
                var isAdminExists = _sharePointService.GetUserbyMail(model.Email);
                if (isAdminExists.Count() == 0 || isAdminExists == null)
                {
                    var admin = _sharePointService.CreateAdmin(model);
                    return Ok(admin);

                }
                else
                {
                    return BadRequest(new
                    {
                        status = "User already Exists",
                        id = isAdminExists[0]["ID"]
                    });
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion


        #region Canditate Registration
        [HttpPost]
        [Route("Canditate")]
        public dynamic Canditate([FromBody] CanditateRegistrationModel model)
        {
            //Check if any interview today
            DateTime currentDate = DateTime.Today;
            string formattedDate = currentDate.ToString("dd-MM-yyyy");
            var interview = _sharePointService.GetInterviewByDate(formattedDate);
            if (interview == null || interview.Count == 0)
            {
                return BadRequest("No interviews today");
            }
            ////Already Registered Canditate
            var isCanditateExists = _sharePointService.VerifyCandidate(model.Email,formattedDate);
            if (isCanditateExists)
            {
                return BadRequest("Canditate is Already Registered for today's Interview");
            }
            var interviewId = interview[0]["InterviewId"].ToString();
            var q = _sharePointService.GetQuestionsForExaminationByID(interviewId);
            Dictionary<string, int> patternCounts = new Dictionary<string, int>();
            foreach (var qw in q)
            {
                if (patternCounts.ContainsKey(qw["Pattern"]))
                {
                    patternCounts[qw["Pattern"]] ++;
                }
                else
                {
                    patternCounts[qw["Pattern"]] = 1;
                }
            }
            model.InterviewDate = formattedDate;
            Random random = new Random();
            //var patternCount = int.Parse(interview[0]["PatternCount"]!=null? interview[0]["PatternCount"].ToString():"0");
            int randomnumber = random.Next(0, patternCounts.Keys.Count());
            model.PatternID = patternCounts.Keys.ElementAt(randomnumber).ToString();
            model.InterviewID = interviewId;

            var canditate =_sharePointService.RegisterCanditate(model);
            if (canditate != null)
            {
                var examinationQuestions = _sharePointService.GetQuestionsForExamination(interviewId, model.PatternID);
                var roundTwoQuestions = _sharePointService.GetRoundTwoQuestionsForExamination(interviewId);
                List<ExaminationQuestionModel> questions = new();
                List<RoundTwoQuestionModel> roundTwoQuestionsList = new List<RoundTwoQuestionModel>();
                foreach (var ques in examinationQuestions)
                {
                    if (ques["IsDeleted"]==false)
                    {
                        List<OptionsModel> options = new List<OptionsModel>();
                        string questionId = ques["QuestionId"].ToString();
                        string question = ques["Question"].ToString();
                        string optionOne = ques["OptionOne"].ToString();
                        string optionTwo = ques["OptionTwo"].ToString();
                        string optionThree = ques["OptionThree"].ToString();
                        string optionFour = ques["OptionFour"].ToString();
                        options.Add(new OptionsModel()
                        {
                            OptionsOne = optionOne,
                            OptionsTwo = optionTwo,
                            OptionsThree = optionThree,
                            OptionsFour = optionFour,
                        });
                        questions.Add(new ExaminationQuestionModel()
                        {
                            Question = question,
                            QuestionId = questionId,
                            Options = options.ToList()

                        });
                    }
                    


                }
                foreach (var qw in roundTwoQuestions)
                {
                    string questionNumber = qw["ID"] != null ? qw["ID"].ToString() : "";
                    string questionId = qw["QuestionId"] != null ? qw["QuestionId"].ToString() : "";
                    string question = qw["Question"] != null ? qw["Question"].ToString() : "";
                    roundTwoQuestionsList.Add(new RoundTwoQuestionModel()
                    {
                        QuestionId = questionId,
                        QuestionNumber = questionNumber,
                        QuestionText = question,
                    });
                }
                var response = new RegistrationResponse
                {
                    Status = "Register successfully",
                    CanditateEmail=model.Email,
                    InterviewId=model.InterviewID,
                    RoundTwoQuestions= roundTwoQuestionsList.ToList(),
                    Questions = questions,

                };

                return response;
            }
            else
            {

                return BadRequest("Error in registration");
            }
            
        }
        #endregion


    }
}
