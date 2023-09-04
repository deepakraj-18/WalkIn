using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.News.DataModel;
using System.Linq;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Helpers
{
    public class SharePointService
    {
        private readonly ClientContext _clientContext;
        private readonly string _adminList;
        private readonly string _interviewList;
        private readonly string _canditateList;
        public SharePointService(ClientContext clientContext)
        {
            _clientContext = clientContext;


        }
        public ListItemCollection FetchUsers()
        {
            List userList = _clientContext.Web.Lists.GetByTitle(_adminList);
            CamlQuery query = new()
            {
                ViewXml = $@"<View/>"
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
            return new AdminModel
            {
                Id = listItem["ID"].ToString(),
                Name = listItem["Title"].ToString(),
                Email = listItem["Email"].ToString(),
                IsDeleted = bool.Parse(listItem["IsDeleted"].ToString
                ()),
                IsApproved = bool.Parse(listItem["IsApproved"].ToString())

            };


        }

        public ListItemCollection GetAllInterviews()
        {
            List interviewList = _clientContext.Web.Lists.GetByTitle("interviewList");
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='IsDeleted' /><Value Type='Boolean'>0</Value></Eq></Where></Query></View>";
            ListItemCollection Lists = interviewList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;
        }
        public ListItem  CreateInterview(InterViewRegistrationModel interview)
        {
            List list = _clientContext.Web.Lists.GetByTitle(_interviewList);
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem listItem = list.AddItem(listItemCreationInformation);
            int interviewCount = GetAllInterviews().Count() + 1;
            string interviewId = "INV" + interviewCount.ToString("D4");
            listItem["InterviewId"] = interviewId;
            listItem["Title"] = interview.Date;
            listItem["ScoreOne"] = interview.Scoreone;
            listItem["ScoreTwo"] = interview.Scoretwo;
            listItem["IsDeleted"] = false;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return listItem;
        }

        public ListItem EditInterview(InterViewUpdateModel interview)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
            ListItem listItem = targetList.GetItemById(interview.ID);
            listItem["Title"] = interview.Date;
            listItem["ScoreOne"] = interview.Scoreone;
            listItem["ScoreTwo"] = interview.Scoretwo;
            listItem["IsDeleted"] = interview.isDeleted;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return listItem;
        }

        public ListItem DeleteInterview(InterViewDeleteModel deletemodel)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
            ListItem listItem = targetList.GetItemById(deletemodel.ID);
            listItem["IsDeleted"] = true;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return listItem;
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



        #region //Question Section


        #region //Add Question
        public dynamic AddQuestion()
        {

            return null;
        }
        #endregion


        #endregion
    }
}
