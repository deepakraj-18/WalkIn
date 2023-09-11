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
            var isCanditateExists = _sharePointService.VerifyCandidate(model.Email);
            if (!isCanditateExists)
            {
                var canditate = _sharePointService.RegisterCanditate(model);
                return Ok("Registered Successfully");
            }
            DateTime currentDate = DateTime.Today;
            string formattedDate = currentDate.ToString("dd-MM-yyyy");
            var interview= _sharePointService.GetInterviewByDate(formattedDate);
            var interviewId=interview[0]["InterviewId"].ToString();
            var patternCount =int.Parse(interview[0]["PatternCount"].ToString());
            Random random = new Random();
            var pattern = random.Next(1, patternCount);
            var examinationQuestions = _sharePointService.GetQuestionsForExamination(interviewId, pattern.ToString());
            List<ExaminationQuestionModel> questions = new();
            foreach (var ques in examinationQuestions)
            {
            List<OptionsModel>options = new List<OptionsModel>();
                string questionId = ques["QuestionId"].ToString();
                string question = ques["Question"].ToString();
                string optionOne = ques["OptionOne"].ToString();
                string optionTwo = ques["OptionTwo"].ToString();
                string optionThree = ques["OptionThree"].ToString();
                string optionFour = ques["OptionFour"].ToString();
                options.Add(new OptionsModel()
                {
                    OptionsOne= optionOne,
                    OptionsTwo= optionTwo,
                    OptionsThree= optionThree,
                    OptionsFour= optionFour,
                });
                questions.Add(new ExaminationQuestionModel()
                {
                    Question = question,
                    QuestionId = questionId,
                    Options = options.ToList()

                });
                

            }
            return Ok(questions);

        }
        #endregion

        #region
        [HttpPost]
        [Route("GetCanditates")]
        public dynamic GetCanditates()
        {
            var canditateList = _sharePointService.GetAllCanditates();
            return Ok(canditateList);
        }
        #endregion

    }
}
