 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnorucsWalkInAPI.Models;
using TechnorucsWalkInAPI.Helpers;
using Microsoft.SharePoint.Client;
using System;

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
            //var isCanditateExists = _sharePointService.VerifyCandidate(model.Email);
            //if (!isCanditateExists)
            //{
            //    return Ok("Registered Successfully");
            //}
            var interviewId = interview[0]["InterviewId"].ToString();
            model.InterviewDate = formattedDate;
            Random random = new Random();
            var patternCount = int.Parse(interview[0]["PatternCount"].ToString());
            model.PatternID = random.Next(1, patternCount).ToString();
            model.InterviewID = interviewId;

            var canditate = _sharePointService.RegisterCanditate(model);
            var examinationQuestions = _sharePointService.GetQuestionsForExamination(interviewId, model.PatternID);
            List<ExaminationQuestionModel> questions = new();
            foreach (var ques in examinationQuestions)
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
            var response = new RegistrationResponse
            {
                Status = "Register successfully",
                Questions = questions
            };

            return response;
        }
        #endregion

        #region
        [HttpPost]
        [Route("GetCanditates")]
        public dynamic GetCanditates()
        {
            var canditateList = _sharePointService.GetAllCanditates();
            List<CanditatesList> canditates = new();
            foreach (var c in canditateList)
            {
                List<ViewCanditateModel> canditate=new List<ViewCanditateModel>();
                string name = c["Title"].ToString();
                string email = c["Email"].ToString();
                string phoneNumber = c["PhoneNumber"].ToString();
                string scoreOne = c["ScoreOne"].ToString() != null ? c["ScoreOne"].ToString():"";
                string scoreTwo = c["ScoreTwo"].ToString()!=null? c["ScoreTwo"].ToString() : "";
                canditate.Add(new ViewCanditateModel()
                {
                    Name=name,
                    Email=email,
                    PhoneNumber=phoneNumber,
                    ScoreOne=scoreOne,
                    ScoreTwo=scoreTwo
                });
                canditates.Add(new ()
                {
                    Canditates=canditate
                });
            }
            return Ok(canditates);
        }
        #endregion

    }
}
