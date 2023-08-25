using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint.Client;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ClientContext _clientContext;
        private readonly string _adminList;
        private readonly JwtBearer _jwtBearer;
        public LoginController(IConfiguration configuration, ClientContext clientContext, JwtBearer jwtBearer)
        {
            _configuration = configuration;
            _clientContext = clientContext;
            _adminList = configuration["adminList"];
            _jwtBearer = jwtBearer;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// 
        /// </returns>
        /// <exception cref="Exception"></exception>
        /// 

        #region Login
        [HttpPost]
        [Route("Login")]
        public dynamic Login([FromBody] AdminLoginModel model)
        {
            try
            {
                ListItemCollection users = FetchUsers(model.Email.ToString());
                if (users.Count == 1)
                {
                    ListItem user = users[0];
                    string password = user["Password"].ToString();
                    bool isMatch = VerifyPassword(password);
                    if (isMatch)
                    {
                        var token = GetToken(user["Title"].ToString());
                        bool hasAccess = Boolean.Parse(user["IsApproved"].ToString());
                        if (hasAccess)
                        {
                            return Ok(new
                            {
                                status = "Login success",
                                id = user["ID"].ToString(),
                                Name = user["Title"].ToString(),
                                Email = user["Email"].ToString(),
                                IsApproved = user["IsApproved"].ToString(),
                                IsDeleted = user["IsDeleted"].ToString(),
                                token = token
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

                }
                else
                {
                    return BadRequest("User not found");
                }
                return null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion



        private dynamic FetchUsers(string email)
        {
            List targetList = _clientContext.Web.Lists.GetByTitle(_adminList);
            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{email}</Value></Eq></Where></Query></View>";
            ListItemCollection items = targetList.GetItems(query);
            _clientContext.Load(items);
            _clientContext.ExecuteQuery();
            return items;
        }

        private dynamic VerifyPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            bool isMatch = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            return isMatch;
        }
        private dynamic GetToken(string name)
        {
            TokenModel tokenModel = new TokenModel();
            tokenModel.Name = name;
            tokenModel.Role = "Admin";
            var Token = _jwtBearer.GenerateToken(tokenModel);
            return Token;
        }
    }
}
