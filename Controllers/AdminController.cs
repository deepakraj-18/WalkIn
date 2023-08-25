using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.News.DataModel;
using TechnorucsWalkInAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechnorucsWalkInAPI.Helpers;

namespace TechnorucsWalkInAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ClientContext _clientContext;
        private readonly string _adminList;
        private readonly JwtBearer _jwtBearer;

        public AdminController(IConfiguration configuration, ClientContext clientContext, JwtBearer jwtBearer)
        {
            _configuration = configuration;
            _clientContext = clientContext;
            _adminList = configuration["adminList"];
            _jwtBearer = jwtBearer;
        }
        #region AdminLogin
        [AllowAnonymous]
        [HttpPost]
        [Route("AdminLogin")]
        public dynamic Login([FromBody] AdminLoginModel model)
        {
            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{model.Email}</Value></Eq></Where></Query></View>";
                ListItemCollection items = targetList.GetItems(query);
                _clientContext.Load(items);
                _clientContext.ExecuteQuery();
                if (items.Count == 1)
                {
                    ListItem item = items[0];
                    string password = item["Password"].ToString();
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    bool isMatch = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    if (isMatch)
                    {
                        TokenModel tokenModel = new TokenModel();
                        tokenModel.Name = item["Title"].ToString();
                        tokenModel.Role = "Admin";
                        var Token =  _jwtBearer.GenerateToken(tokenModel);
                        bool hasAccess = Boolean.Parse(item["IsApproved"].ToString());
                        if (hasAccess)
                        {
                            return Ok(new
                            {
                                status = "Login success",
                                id = item["ID"].ToString(),
                                Name = item["Title"].ToString(),
                                Email = item["Email"].ToString(),
                                IsApproved = item["IsApproved"].ToString(),
                                IsDeleted = item["IsDeleted"].ToString(),
                                token = Token
                            });
                        }
                        else
                        {
                            return BadRequest(new
                            {
                                status = "You don't have access",
                            });
                        }

                    }
                    else
                    {
                        return BadRequest("Invalid Password");
                    }
                }
                else
                {
                    return BadRequest("User not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion

        #region Admin creation
        [AllowAnonymous]
        [HttpPost]
        [Route("CreateAdminUser")]
        public dynamic CreateAdminUser([FromBody] AdminRegisterModel model)
        {
            if (model == null || model.Email == null || model.Password == null || model.Name == null)
            {
                return "Please give   all the required details";
            }

            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
                CamlQuery query = new CamlQuery();
                query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{model.Email}</Value></Eq></Where></Query></View>";
                ListItemCollection items = targetList.GetItems(query);
                _clientContext.Load(items);
                _clientContext.ExecuteQuery();
                if (items.Count == 0)
                {
                    List list = _clientContext.Web.Lists.GetByTitle(_adminList);
                    ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                    ListItem listItem = list.AddItem(listItemCreationInformation);
                    listItem["Title"] = model.Name;
                    listItem["Email"] = model.Email;
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    listItem["Password"] = hashedPassword;
                    listItem.Update();
                    _clientContext.ExecuteQuery();
                    return new
                    {
                        status = "Registered successfully",
                        id = listItem["ID"].ToString(),
                        Name = listItem["Title"].ToString(),
                        Email = listItem["Email"].ToString(),
                        IsApproved = listItem["IsApproved"].ToString(),
                        IsDeleted = listItem["IsDeleted"].ToString()
                    };
                }
                else
                {
                    return new
                    {
                        status = "User Already Exists",
                        id = items[0]["ID"].ToString(),
                        Name = items[0]["Title"].ToString(),
                        Email = items[0]["Email"].ToString(),
                        IsApproved = items[0]["IsApproved"].ToString(),
                        IsDeleted = items[0]["IsDeleted"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #endregion


        #region Approve Admin

        [HttpPost]
        [Route("ApproveAdmin")]
        public dynamic ApproveAdmin([FromBody] AdminApprovalModel model)
        {

            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
                ListItem listItem = targetList.GetItemById(model.id);
                listItem["IsDeleted"] = model.isApproved;
                listItem.Update();
                _clientContext.ExecuteQuery();
                if (model.isApproved)
                {
                    return new
                    {
                        status = "The admin is Deleted"
                    };
                }
                else
                {
                    return new
                    {
                        status = "The admin is Deleted"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion


        #region Delete Admin

        [HttpPost]
        [Route("DeleteAdmin")]
        public dynamic DeleteAdmin([FromBody] AdminDeleteModel model)
        {
            try
            {
                List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
                ListItem listItem = targetList.GetItemById(model.id);
                listItem["IsApproved"] = model.IsDeleted;
                listItem.Update();
                _clientContext.ExecuteQuery();
                if (model.IsDeleted)
                {
                    return new
                    {
                        status = "The admin is Approved"
                    };
                }
                else
                {
                    return new
                    {
                        status = "The admin is UnApproved"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion




        
    }
}

