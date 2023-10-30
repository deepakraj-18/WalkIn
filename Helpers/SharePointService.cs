﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Discovery;
using Microsoft.SharePoint.News.DataModel;
using System.Globalization;
using System.Linq;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Helpers
{
    public class SharePointService
    {
        private IConfiguration _configuration;
        private readonly ClientContext _clientContext;
        private readonly string _adminList;
        private readonly string _interviewList;
        private readonly string _canditateList;
        private readonly string _questionList;
        private readonly string _answerList;
        private readonly string _roundTwoQuestionList;
        public SharePointService(ClientContext clientContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _clientContext = clientContext;
            _adminList = configuration["adminList"];
            _interviewList = configuration["interviewList"];
            _canditateList = configuration["canditateList"];
            _questionList = configuration["questionList"];
            _answerList = configuration["answerList"];
            _roundTwoQuestionList = configuration["roundTwoQuestionList"];




        }
        public ListItemCollection FetchUsers()
        {
            List userList = _clientContext.Web.Lists.GetByTitle(_adminList);
            CamlQuery query = new()
            {
                ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='IsDeleted' /><Value Type='Text'>0</Value></Eq></Where></Query></View>"
            };
            ListItemCollection items = userList.GetItems(query);
            _clientContext.Load(items);
            _clientContext.ExecuteQuery();
            return items;
        }

        public ListItemCollection GetUserbyMail(string email)
        {
            List userList = _clientContext.Web.Lists.GetByTitle(_adminList);
            CamlQuery query = new CamlQuery
            {
                ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{email}</Value></Eq></Where></Query></View>"
            };
            ListItemCollection user = userList.GetItems(query);
            _clientContext.Load(user);
            _clientContext.ExecuteQuery();
            return user;
        }


        /// <summary>
        /// This method is used to fetch users with their id
        /// </summary>
        /// <param name="model">
        /// id=user id
        /// </param>
        /// <returns>
        /// the specific user details
        /// </returns>
        public ListItemCollection GetUserbyId(int id)
        {
            List userList = _clientContext.Web.Lists.GetByTitle(_adminList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Text'>{id}</Value></Eq></Where></Query></View>";
            ListItemCollection user = userList.GetItems(query);
            _clientContext.Load(user);
            _clientContext.ExecuteQuery();
            return user;
        }

        /// <summary>
        /// This method is used to approve the admin
        /// </summary>
        /// <param name="model">
        /// id = user id
        /// isApproved = approval status
        /// </param>
        /// <returns>
        /// this method returns the updated value of the specific admin user
        /// </returns>
        public ListItem ApproveAdmin(AdminApprovalModel model)
        {

            List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
            ListItem listItem = targetList.GetItemById(model.id);
            listItem["IsApproved"] = model.isApproved;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return listItem;
        }



        /// <summary>
        /// This method is used to delete the admin
        /// </summary>
        /// <param name="model">
        /// id = user id
        /// isDeleted = boolean value to delete the admin
        /// </param>
        /// <returns>
        /// this method returns the updated value of the specific admin user
        /// </returns>
        public bool DeleteAdmin(AdminDeleteModel model)
        {

            List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
            ListItem listItem = targetList.GetItemById(model.id);
            listItem["IsDeleted"] = model.IsDeleted;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return true;
        }




        public dynamic CreateAdmin(AdminRegisterModel model)
        {
            List list = _clientContext.Web.Lists.GetByTitle(_adminList);
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem listItem = list.AddItem(listItemCreationInformation);
            listItem["Title"] = model.Name;
            listItem["Email"] = model.Email;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            listItem["Password"] = hashedPassword;
            listItem["IsDeleted"] = model.IsDeleted;
            listItem["IsApproved"] = model.IsApproved;
            listItem.Update();
            _clientContext.ExecuteQuery();
            var user = GetUserbyMail(model.Email);
            return new AdminModel
            {
                Id = user[0]["ID"].ToString(),
                Name = listItem["Title"].ToString(),
                Email = listItem["Email"].ToString(),
                IsDeleted = bool.Parse(listItem["IsDeleted"].ToString
                ()),
                IsApproved = bool.Parse(listItem["IsApproved"].ToString())

            };


        }

        public ListItemCollection GetAllInterviews()
        {
            List interviewList = _clientContext.Web.Lists.GetByTitle(_interviewList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></Where></Query></View>";
            ListItemCollection Lists = interviewList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;
        }

        public int GetInterviewCount()
        {
            List interviewList = _clientContext.Web.Lists.GetByTitle(_interviewList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View></View>";
            ListItemCollection Lists = interviewList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists.Count();

        }
        /// <summary>
        /// This method is used to get the specificed interview by its Id
        /// </summary>
        /// <param name="InterviewId">GetInterviewByIdModel</param>
        /// <returns>The matched details of the specified InterviewId </returns>
        /// 
        public ListItemCollection GetInterviewById(GetInterviewByIdModel model)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_interviewList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='InterviewId' /><Value Type='Text'>{model.InterviewId}</Value></Eq></Where></Query></View>";
            ListItemCollection Lists = targetList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;
        }
        public ListItemCollection GetInterviewById(string interviewID)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_interviewList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='InterviewId' /><Value Type='Text'>{interviewID}</Value></Eq></Where></Query></View>";
            ListItemCollection Lists = targetList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;
        }

        public ListItemCollection GetInterviewByDate(string date)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_interviewList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='Title' /><Value Type='Text'>{date}</Value></Eq><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></And></Where></Query></View>";
            ListItemCollection Lists = targetList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;
        }
        public ListItem CreateInterview(InterViewRegistrationModel interview)
        {
            List list = _clientContext.Web.Lists.GetByTitle(_interviewList);
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem listItem = list.AddItem(listItemCreationInformation);
            int interviewCount = GetInterviewCount() + 1;
            string interviewId = "INV" + interviewCount.ToString("D4");
            listItem["InterviewId"] = interviewId;
            var parsedDate = DateOnly.ParseExact(interview.Date.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            listItem["Title"] = parsedDate.ToString("dd-MM-yyyy");
            listItem["ScoreOne"] = interview.Scoreone;
            listItem["ScoreTwo"] = interview.Scoretwo;
            listItem["PatternCount"] = interview.PatternCount;
            listItem["IsDeleted"] = false;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return listItem;
        }


        public dynamic EditInterview(InterViewUpdateModel interview)
        {
            List list = _clientContext.Web.Lists.GetByTitle(_interviewList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='InterviewId' /><Value Type='Text'>{interview.ID}</Value></Eq></Where></Query></View>";
            ListItemCollection items = list.GetItems(query);
            _clientContext.Load(items);
            _clientContext.ExecuteQuery();
            if (items.Count == 1)
            {
                ListItem listItem = items[0];
                var parsedDate = DateOnly.ParseExact(interview.Date.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                listItem["Title"] = parsedDate.ToString("dd-MM-yyyy");
                listItem["ScoreOne"] = interview.Scoreone;
                listItem["ScoreTwo"] = interview.Scoretwo;
                //listItem["PatternCount"] = interview.PatternCount;
                //listItem["IsDeleted"] = interview.isDeleted;
                listItem.Update();
                _clientContext.ExecuteQuery();
                return listItem;
            }
            return null;
        }
        public dynamic EditInterview(string interviewId, string patternCount)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_interviewList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='InterviewId' /><Value Type='Text'>{interviewId}</Value></Eq></Where></Query></View>";
            ListItemCollection items = targetList.GetItems(query);
            _clientContext.Load(items);
            _clientContext.ExecuteQuery();
            if (items.Count != 0)
            {
                ListItem item = items[0];
                item["PatternCount"] = patternCount;
                item.Update();
                _clientContext.ExecuteQuery();
                return true;
            }
            return false;
        }

        public Boolean DeleteInterview(InterViewDeleteModel deletemodel)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_interviewList);
            ListItem listItem = targetList.GetItemById(deletemodel.ID);
            listItem["IsDeleted"] = true;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return true;
        }


        public ListItem RegisterCanditate(CanditateRegistrationModel canditate)
        {
            List list = _clientContext.Web.Lists.GetByTitle(_canditateList);
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem listItem = list.AddItem(listItemCreationInformation);
            listItem["Title"] = canditate.Name;
            listItem["Email"] = canditate.Email;
            listItem["PhoneNumber"] = canditate.PhoneNumber;
            listItem["City"] = canditate.City;
            listItem["Institute"] = canditate.Institute;
            listItem["Technology"] = canditate.Technology;
            listItem["Experience"] = canditate.Experience;
            listItem["Certification"] = canditate.Certification;
            listItem["Skills"] = canditate.Skills;
            listItem["Source"] = canditate.Source;
            listItem["OthersReference"] = canditate.Reference;
            listItem["Degree"] = canditate.Degree;
            listItem["Gender"] = canditate.Gender;
            listItem["PatternId"] = canditate.PatternID;
            listItem["InterviewDate"] = canditate.InterviewDate;
            listItem["InterviewID"] = canditate.InterviewID;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return listItem;
        }
        public ListItemCollection GetAllCanditates()
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_canditateList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View/>";
            ListItemCollection list = targetList.GetItems(query);
            _clientContext.Load(list);
            _clientContext.ExecuteQuery();
            return list;
        }

        public Boolean VerifyCandidate(string email, string interviewDate)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_canditateList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='Email' /><Value Type='Text'>{email}</Value></Eq><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></And><And><Eq><FieldRef Name='InterviewDate' /><Value Type='Text'>{interviewDate}</Value></Eq></And></Where></Query></View>";
            //
            ListItemCollection list = targetList.GetItems(query);
            _clientContext.Load(list);
            _clientContext.ExecuteQuery();
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        public dynamic GetCanditatesByInteviewId(string interviewId, string interviewDate)
        {
            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_canditateList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='InterviewID' /><Value Type='Text'>{interviewId}</Value></Eq><Eq><FieldRef Name='InterviewDate' /><Value Type='Text'>{interviewDate}</Value></Eq></Where></Query></View>";
                ListItemCollection list = targetList.GetItems(query);
                _clientContext.Load(list);
                _clientContext.ExecuteQuery();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        #region //Question Section


        #region //Add Question
        public Boolean AddQuestion(QuestionModel question, string InterviewId)
        {

            try
            {
                List list = _clientContext.Web.Lists.GetByTitle(_questionList);
                ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                ListItem questionItem = list.AddItem(listItemCreationInformation);
                var questionCount = GetAllQuestionsCount() + 1;
                questionItem["InterviewID"] = InterviewId;
                questionItem["Pattern"] = question.PatternType;
                questionItem["QuestionId"] = "QW" + questionCount.ToString("D4");
                questionItem["Question"] = question.QuestionText;
                questionItem["OptionOne"] = question.Options[0].Option1;
                questionItem["OptionTwo"] = question.Options[0].Option2;
                questionItem["OptionThree"] = question.Options[0].Option3;
                questionItem["OptionFour"] = question.Options[0].Option4;
                questionItem["Answer"] = question.Answer;
                questionItem["HasMultipleChoice"] = question.HasMultipleChoice;
                questionItem["IsDeleted"] = question.IsDeleted;
                questionItem.Update();
                _clientContext.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }


        }
        #endregion 


        #region //Add Round Two Question
        public Boolean AddRoundTwoQuestion(RoundTwoQuestionModel question, string InterviewId)
        {

            try
            {
                List list = _clientContext.Web.Lists.GetByTitle(_roundTwoQuestionList);
                ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                ListItem questionItem = list.AddItem(listItemCreationInformation);
                var questionCount = GetRoundTwoQuestionsCount() + 1;
                questionItem["Title"] = InterviewId + "QW" + questionCount.ToString("D4");
                questionItem["InterviewID"] = InterviewId;
                questionItem["QuestionId"] = "QW" + questionCount.ToString("D4");
                questionItem["Question"] = question.QuestionText;
                questionItem["Pattern"] = question.PatternType;
                questionItem["IsDeleted"] = question.IsDeleted;
                questionItem.Update();
                _clientContext.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }


        }
        #endregion



        #region
        public dynamic editQuestion(EditQuestionModel model)
        {

            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_questionList);

                foreach (var qws in model.Questions)
                {
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='QuestionId' /><Value Type='Text'>{qws.QuestionNumber}</Value></Eq></Where></Query></View>";
                    ListItemCollection list = targetList.GetItems(query);
                    _clientContext.Load(list);
                    _clientContext.ExecuteQuery();
                    if (list.Count == 0)
                    {
                        AddQuestion(qws, model.InterviewID);
                    }
                    else if(qws.IsDeleted)
                    {
                        var questionItem = list[0];
                        questionItem["IsDeleted"] = qws.IsDeleted;
                        questionItem.Update();
                        _clientContext.ExecuteQuery();

                    }
                    else
                    {

                        var questionItem = list[0];
                        questionItem["Question"] = qws.QuestionText;
                        questionItem["OptionOne"] = qws.Options[0].Option1;
                        questionItem["OptionTwo"] = qws.Options[0].Option2;
                        questionItem["OptionThree"] = qws.Options[0].Option3;
                        questionItem["OptionFour"] = qws.Options[0].Option4;
                        questionItem["Answer"] = qws.Answer;
                        questionItem["IsDeleted"] = qws.IsDeleted;
                        questionItem["HasMultipleChoice"] = qws.HasMultipleChoice;
                        questionItem.Update();
                        _clientContext.ExecuteQuery();
                    }
                }
                return "Questions updated successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion



        #region // Edit Round Two Question
        /// <summary>
        /// Edit quesitons of round Two
        /// </summary>
        /// <param name="model">EditQuestionModel</param>
        /// <returns>String</returns>
        /// <exception cref="Exception"></exception>
        public dynamic EditRoundTwoQuestion(EditRoundTwoQuestionModel model)
        {

            try
            {
                List roundTwoTargetList = _clientContext.Web.Lists.GetByTitle(_roundTwoQuestionList);

                foreach (var qws in model.RoundTwoQuestions)
                {
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='QuestionId' /><Value Type='Text'>{qws.QuestionNumber}</Value></Eq></Where></Query></View>";
                    ListItemCollection list = roundTwoTargetList.GetItems(query);
                    _clientContext.Load(list);
                    _clientContext.ExecuteQuery();
                    if (list.Count == 0)
                    {
                        AddRoundTwoQuestion(qws, model.InterviewID);
                    }
                    else
                    {

                        var questionItem = list[0];
                        questionItem["Question"] = qws.QuestionText;
                        questionItem["IsDeleted"] = qws.IsDeleted;
                        questionItem.Update();
                        _clientContext.ExecuteQuery();
                    }
                }
                return "Questions edited successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion


        public ListItemCollection GetQuestionForInterview(GetInterviewQuestionModel model)
        {
            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_questionList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='InterviewID' /><Value Type='Text'>{model.InterviewId}</Value></Eq><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></And></Where></Query></View>";
                ListItemCollection list = targetList.GetItems(query);
                _clientContext.Load(list);
                _clientContext.ExecuteQuery();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ListItemCollection GetRoundTwoQuestions(GetInterviewQuestionModel model)
        {
            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_roundTwoQuestionList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='InterviewID' /><Value Type='Text'>{model.InterviewId}</Value></Eq></Where></Query></View>";
                ListItemCollection list = targetList.GetItems(query);
                _clientContext.Load(list);
                _clientContext.ExecuteQuery();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region
        public dynamic GetQuestionById(string QuestionID)
        {
            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_questionList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='QuestionId' /><Value Type='Text'>{QuestionID}</Value></Eq></Where></Query></View>";
                ListItemCollection list = targetList.GetItems(query);
                _clientContext.Load(list);
                _clientContext.ExecuteQuery();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region
        public dynamic GetRoundTwoQuestionById(string QuestionID)
        {
            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_roundTwoQuestionList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='QuestionId' /><Value Type='Text'>{QuestionID}</Value></Eq></Where></Query></View>";
                ListItemCollection list = targetList.GetItems(query);
                _clientContext.Load(list);
                _clientContext.ExecuteQuery();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion


        #region
        public int GetAllQuestionsCount()
        {
            List interviewList = _clientContext.Web.Lists.GetByTitle(_questionList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View/>";
            ListItemCollection Lists = interviewList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists.Count
                ();
        }
        #endregion
        #region
        public int GetRoundTwoQuestionsCount()
        {
            List interviewList = _clientContext.Web.Lists.GetByTitle(_roundTwoQuestionList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View/>";
            ListItemCollection Lists = interviewList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists.Count
                ();
        }
        #endregion



        #region //Examination


        #region
        public string ValidateAnswers(ExaminationModel model)
        {
            var interview = GetInterviewById(model.InterviewId);
            if (interview == null)
            {
                return ("Interview Id is Invalid");
            }
            var roundOne = Convert.ToInt64
                (interview[0]["ScoreOne"]);
            var roundTwo = Convert.ToInt64(interview[0]["ScoreOne"]);
            var score = 0;
            if(model.Answer!=null&&model.Answer.Count()>0)
            {
                foreach (var ans in model.Answer)
                {
                    var answer = GetQuestionById(ans.QuestionId);
                    if (answer == null)
                    {
                        return ("Invalid QuestionID");
                    }
                    if (ans.Answer == answer[0]["Answer"])
                    {
                        score++;
                    }
                }
            UpdateScores(model.CanditateEmail, score.ToString(), score >= roundOne ? true : false);
            return "Score updated successfully";
            }
            else
            {
                return "Answers are empty and score is updated";
            }
            

        }
        #endregion



        #region
        public dynamic UpdateScores(string mail, string score, Boolean result)
        {
            if (score == null)
                return null;
            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_canditateList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{mail}</Value></Eq></Where></Query></View>";
                ListItemCollection list = targetList.GetItems(query);
                _clientContext.Load(list);
                _clientContext.ExecuteQuery();
                if (list.Count > 0)
                {
                    ListItem listItem = list[0];
                    listItem["ScoreOne"] = score;
                    listItem["Result"] = result;
                    listItem.Update();
                    _clientContext.ExecuteQuery();
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #endregion

        #region
        /// <summary>
        /// Change the Result Status of the Canditate by Admin
        /// </summary>
        /// <param name="model.Result">Result - Boolean</param>
        /// <param name="model.CanditateEmail">CanditateEmail</param>
        /// <returns>Canditate Details</returns>
        /// <exception cref="Exception"></exception>
        public ListItemCollection ChangeResultStatus(ChangeResultStatusModel model)
        {
            try
            {
                List targetLists = _clientContext.Web.Lists.GetByTitle(_canditateList);
                CamlQuery query = new();
                query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='Email' /><Value Type='Text'>{model.CanditateEmail}</Value></Eq><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></And></Where></Query></View>";
                ListItemCollection Lists = targetLists.GetItems(query);
                _clientContext.Load(Lists);
                _clientContext.ExecuteQuery();
                if (Lists.Count > 0)
                {
                    ListItem listItem = Lists[0];
                    listItem["Result"] = model.Result;
                    listItem.Update();
                    _clientContext.ExecuteQuery();
                    return Lists;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion



        #region
        /// <summary>
        /// Submit scores for round two
        /// </summary>
        /// <param name="model.Score">Score - Int</param>
        /// <param name="model.CanditateEmail">CanditateEmail</param>
        /// <returns>Canditate Details</returns>
        /// <exception cref="Exception"></exception>
        public ListItemCollection UpdateRoundTwoScore(RoundTwoScoreModel model)
        {
            try
            {
                List targetLists = _clientContext.Web.Lists.GetByTitle(_canditateList);
                CamlQuery query = new();
                query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='Email' /><Value Type='Text'>{model.CanditateEmail}</Value></Eq><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></And></Where></Query></View>";
                ListItemCollection Lists = targetLists.GetItems(query);
                _clientContext.Load(Lists);
                _clientContext.ExecuteQuery();
                if (Lists.Count > 0)
                {
                    ListItem listItem = Lists[0];
                    listItem["ScoreTwo"] = model.Score.ToString();
                    listItem.Update();
                    _clientContext.ExecuteQuery();
                    var response = GetInterviewById(listItem["InterviewID"].ToString());
                    if (response.Count != 0)
                    {
                        var roundTwoEligiblityScore = response[0]["ScoreTwo"].ToString();
                        var status = Convert.ToInt32(roundTwoEligiblityScore) <= model.Score ? true : false;
                        var result = new ChangeResultStatusModel
                        {
                            CanditateEmail = listItem["Email"].ToString(),
                            Result = status,
                        };
                        ChangeResultStatus(result);
                    }
                    return Lists;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public dynamic GetQuestionsForExamination(string InterviewID, string patternId)
        {
            List targetLists = _clientContext.Web.Lists.GetByTitle(_questionList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='InterviewID' /><Value Type='Text'>{InterviewID}</Value></Eq><Eq><FieldRef Name='Pattern' /><Value Type='Text'>{patternId}</Value></Eq></And></Where></Query></View>";
            ListItemCollection Lists = targetLists.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;

        }
        public dynamic GetQuestionsForExaminationByID(string InterviewID)
        {
            List targetLists = _clientContext.Web.Lists.GetByTitle(_questionList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='InterviewID' /><Value Type='Text'>{InterviewID}</Value></Eq><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></And></Where></Query></View>";
            ListItemCollection Lists = targetLists.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;

        }

        #region
        public dynamic GetRoundTwoQuestionsForExamination(string InterviewID)
        {
            List targetLists = _clientContext.Web.Lists.GetByTitle(_roundTwoQuestionList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='InterviewID' /><Value Type='Text'>{InterviewID}</Value></Eq><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></And></Where></Query></View>";
            ListItemCollection Lists = targetLists.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;

        }
        #endregion




        public dynamic GetPatternCount()
        {
            return null;
        }



        #region
        public dynamic SaveAnswer(ExaminationModel model)
        {
            try
            {
                var canditate = getCanditateByEmail(model.CanditateEmail);
                if (canditate == null)
                {
                    return "Canditate doesn't exists";

                }

                List targetLists = _clientContext.Web.Lists.GetByTitle(_answerList);
                ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                if (model.Answer != null && model.Answer.Count > 0)
                {

                    foreach (var item in model.Answer)
                    {
                        var question = GetQuestionById(item.QuestionId);
                        string qw = question[0]["Question"];
                        string ans = "";
                        var answerValue = question[0]["Answer"];
                        switch (answerValue)
                            {
                                case "1":
                                    ans = question[0]["OptionOne"]?.ToString();
                                    break;
                                case "2":
                                    ans = question[0]["OptionTwo"]?.ToString();
                                    break;
                                case "3":
                                    ans = question[0]["OptionThree"]?.ToString();
                                    break;
                                case "4":
                                    ans = question[0]["OptionFour"]?.ToString();
                                    break;

                            }
                        var canditateAnswer = "";
                        var submittedValue = item.Answer.ToString();

                        switch (submittedValue)
                        {
                            case "1":
                                canditateAnswer = question[0]["OptionOne"]?.ToString();
                                break;
                            case "2":
                                canditateAnswer = question[0]["OptionTwo"]?.ToString();
                                break;
                            case "3":
                                canditateAnswer = question[0]["OptionThree"]?.ToString();
                                break;
                            case "4":
                                canditateAnswer = question[0]["OptionFour"]?.ToString();
                                break;

                        }

                        ListItem questionItem = targetLists.AddItem(listItemCreationInformation);
                        questionItem["Title"] = model.InterviewId + item.QuestionId;
                        questionItem["CanditateId"] = canditate[0]["Email"];
                        questionItem["InterviewId"] = model.InterviewId.ToString();
                        questionItem["QuestionId"] = item.QuestionId.ToString();
                        questionItem["Question"] = qw;
                        questionItem["Answer"] = ans;
                        questionItem["SubmittedAnswer"] = canditateAnswer;
                        questionItem.Update();
                        _clientContext.ExecuteQuery();

                    }
                }
                if (model.RoundTwoAnswer != null && model.RoundTwoAnswer.Count > 0)
                {

                    foreach (var item in model.RoundTwoAnswer)
                    {
                        var question = GetRoundTwoQuestionById(item.QuestionId);
                        string qw = question[0]["Question"];
                        ListItem questionItem = targetLists.AddItem(listItemCreationInformation);
                        questionItem["Title"] = model.InterviewId + item.QuestionId;
                        questionItem["CanditateId"] = canditate[0]["Email"];
                        questionItem["InterviewId"] = model.InterviewId.ToString();
                        questionItem["QuestionId"] = item.QuestionId.ToString();
                        questionItem["Question"] = qw;
                        questionItem["SubmittedAnswer"] = item.Answer.ToString();
                        questionItem.Update();
                        _clientContext.ExecuteQuery();

                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #endregion


        #region
        public dynamic getCanditateByEmail(string email)
        {
            List targetLists = _clientContext.Web.Lists.GetByTitle(_canditateList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{email}</Value></Eq></Where></Query></View>";
            ListItemCollection Lists = targetLists.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            if (Lists == null)
                return null;
            return Lists;
        }
        #endregion


        #region
        public dynamic getCanditateByID(string id)
        {
            List targetLists = _clientContext.Web.Lists.GetByTitle(_canditateList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Text'>{id}</Value></Eq></Where></Query></View>";
            ListItemCollection Lists = targetLists.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            if (Lists == null && Lists.Count() == 0)
                return null;
            return Lists;
        }
        #endregion



        #region
        public dynamic getAnswers(string email, string interviewId)
        {
            List targetLists = _clientContext.Web.Lists.GetByTitle(_answerList);
            CamlQuery query = new();
            query.ViewXml = $@"<View><Query><Where><And><Eq><FieldRef Name='CanditateId' /><Value Type='Text'>{email}</Value></Eq><Eq><FieldRef Name='InterviewId' /><Value Type='Text'>{interviewId}</Value></Eq></And></Where></Query></View>";
            ListItemCollection Lists = targetLists.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            if (Lists == null && Lists.Count() == 0)
                return null;
            return Lists;
        }
        #endregion



        #endregion





    }



}
