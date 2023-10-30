using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public dynamic AddQuestion([FromBody] QuestionsModel questions)
        {
            var result = false;
            if (questions.InterviewID == null)
            {
                return BadRequest("Interview Id is Mandatory");
            }
            if (questions.PatternCount == null)
            {
                return BadRequest("Pattern Count is Mandatory");
            }
            var isPatternUpdated = _sharePointService.EditInterview(questions.InterviewID, questions.PatternCount);
            if (!isPatternUpdated)
            {
                return BadRequest("Invalid Interview Id");
            }
            if (questions.Questions.Count > 0)
            {
                foreach (var question in questions.Questions)
                {
                    result = _sharePointService.AddQuestion(question, questions.InterviewID);
                    if (!result)
                    {
                        return BadRequest("Adding Question Failed");
                    }
                }


                return Ok("Questions Added Succesfully");
            }
            else
            {
                return Ok("Please Add questions to the Interview");
            }

        }
        #endregion


        #region
        [HttpPost]
        [Route("AddQuestionsForRoundTwo")]
        public dynamic AddQuestionsForROundTwo([FromBody] RoundTwoQuestionsModel questions)
        {
            var isRoundTwoQuestionsAdded = false;
            if (questions.InterviewID == null)
            {
                return BadRequest("Interview Id is Mandatory");
            }
            if (questions.RoundTwoQuestions.Count > 0)
            {

                foreach (var question in questions.RoundTwoQuestions)
                {
                    isRoundTwoQuestionsAdded = _sharePointService.AddRoundTwoQuestion(question, questions.InterviewID);
                    if (!isRoundTwoQuestionsAdded)
                    {
                        return BadRequest("Adding Question for Round One Failed");

                    }
                }

                return Ok("Questions Added Succesfully");
            }
            else
            {
                return Ok("Please Add questions to the Interview");
            }
        }
        #endregion


        /// <summary>
        /// Read All question for the particular interview
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("GetQuestionForInterview")]
        public dynamic GetQuestionForInterview([FromBody] GetInterviewQuestionModel model)
        {
            var response = _sharePointService.GetQuestionForInterview(model);
            var roundTwoQuestions = _sharePointService.GetRoundTwoQuestions(model);
            var interviewResponse = _sharePointService.GetInterviewById(model.InterviewId);
            string patternCount = interviewResponse?[0]["PatternCount"] != null ? interviewResponse[0]["PatternCount"].ToString() : "0";
            List<QuestionModel> questionList = new List<QuestionModel>();
            List<RoundTwoQuestionModel> roundTwoQuestionsList = new List<RoundTwoQuestionModel>();
            foreach (var ques in response)
            {
                List<OptionModel> options = new List<OptionModel>();
                string id = ques["ID"] != null ? ques["ID"].ToString() : "";
                string num = ques["QuestionId"] != null ? ques["QuestionId"].ToString() : "";
                string question = ques["Question"] != null ? ques["Question"].ToString() : "";
                string patternType = ques["Pattern"] != null ? ques["Pattern"].ToString() : "";
                string answer = ques["Answer"] != null ? ques["Answer"].ToString() : "";
                string optionOne = ques["OptionOne"] != null ? ques["OptionOne"].ToString() : "";
                string optionTwo = ques["OptionTwo"] != null ? ques["OptionTwo"].ToString() : "";
                string optionThree = ques["OptionThree"] != null ? ques["OptionThree"].ToString() : "";
                string optionFour = ques["OptionFour"] != null ? ques["OptionFour"].ToString() : "";


                options.Add(new OptionModel()
                {
                    Option1 = optionOne,
                    Option2 = optionTwo,
                    Option3 = optionThree,
                    Option4 = optionFour,
                });

                questionList.Add(new QuestionModel()
                {
                    QuestionNumber = id,
                    QuestionId = num,
                    QuestionText = question,
                    Answer = answer,
                    Options = options.ToList(),
                    PatternType = patternType
                });
            }
            foreach(var qw in roundTwoQuestions)
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

            List<GetQuestionsModel> interviewModels = new List<GetQuestionsModel>();
            interviewModels.Add(new GetQuestionsModel()
            {
                InterviewID = model.InterviewId,
                Questions = questionList.ToList(),
                RoundTwoQuestions= roundTwoQuestionsList.ToList(),
                PatternCount = patternCount
            });

            return Ok(interviewModels);
        }


        #region //Edit Question
        [HttpPost]
        [Route("EditQuestion")]
        public dynamic EditQuestion([FromBody] EditQuestionModel model)
        {
            if (model.InterviewID == null)
            {
                return BadRequest("Interview Id is mandatory");
            }
            var response = _sharePointService.editQuestion(model);
            return Ok(response);
        }
        #endregion 
        
         #region //Edit  Round Two Question
        [HttpPost]
        [Route("EditRoundTwoQuestion")]
        public dynamic EditRoundTwoQuestion([FromBody] EditRoundTwoQuestionModel model)
        {
            if (model.InterviewID == null)
            {
                return BadRequest("Interview Id is mandatory");
            }
            var response = _sharePointService.EditRoundTwoQuestion(model);
            return Ok(response);
        }
        #endregion



    }
}
