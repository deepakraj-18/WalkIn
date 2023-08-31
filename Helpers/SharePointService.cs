using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.News.DataModel;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Helpers
{
    public class SharePointService
    {
        private readonly IConfiguration _configuration;
        private readonly ClientContext _clientContext;
        private readonly string _adminList;
        private readonly string _interviewList;
        public SharePointService(ClientContext clientContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _clientContext = clientContext;
            _adminList = configuration["adminList"];
            _interviewList = configuration["interviewList"];
        }
        public ListItemCollection FetchUsers()
        {
            List userList = _clientContext.Web.Lists.GetByTitle(_adminList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"<View/>";
            ListItemCollection items = userList.GetItems(query);
            _clientContext.Load(items);
            _clientContext.ExecuteQuery();
            return items;
        }

        public ListItemCollection GetUserbyMail(string email)
        {
            List userList = _clientContext.Web.Lists.GetByTitle(_adminList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{email}</Value></Eq></Where></Query></View>";
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
            query.ViewXml = "<View/>";
            ListItemCollection Lists = interviewList.GetItems(query);
            _clientContext.Load(Lists);
            _clientContext.ExecuteQuery();
            return Lists;
        }
        public ListItem CreateInterview(InterViewRegistrationModel interview)
        {
            List list = _clientContext.Web.Lists.GetByTitle(_interviewList);
            ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
            ListItem listItem = list.AddItem(listItemCreationInformation);
            //var interviewCount = GetAllInterviews().Count().ToString();
            //listItem["ID"] = "INV"+ interviewCount;
            listItem["Title"] = interview.Date;
            listItem["ScoreOne"] = interview.Scoreone;
            listItem["ScoreTwo"] = interview.Scoretwo;
            listItem["IsDeleted"] = false;
            listItem.Update();
            _clientContext.ExecuteQuery();
            return listItem;
        }
    }
}
