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

namespace TechnorucsWalkInAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;           
           
            
        }
        #region AdminLogin
        [AllowAnonymous]
        [HttpPost]
        [Route("AdminLogin")]
        public dynamic Login([FromBody] AdminLoginModel model)
        {
            string siteUrl = _configuration["siteurl"];
            string adminList = _configuration["adminList"];
            string appId = _configuration["appId"];
            string appSecret = _configuration["appSecret"];
            try
            {
                using (ClientContext clientContext = new PnP.Framework.AuthenticationManager().GetACSAppOnlyContext(siteUrl, appId, appSecret))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(adminList);
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{model.Email}</Value></Eq></Where></Query></View>";
                    ListItemCollection items = targetList.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();
                    if (items.Count == 1)
                    {
                        ListItem item = items[0];
                        string password = item["Password"].ToString();
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                        bool isMatch = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                        if (isMatch)
                        {
                            AdminTokenModel tokenModel= new AdminTokenModel();
                            tokenModel.Name = item["Title"].ToString();
                            tokenModel.Role = "Admin";
                            var Token = GenerateToken(tokenModel);
                            return new
                            {
                                status = "Login success",
                                id = item["ID"].ToString(),
                                Name = item["Title"].ToString(),
                                Email = item["Email"].ToString(),
                                IsApproved = item["IsApproved"].ToString(),
                                IsDeleted = item["IsDeleted"].ToString(),
                                token=Token
                            };
                        }
                        else
                        {
                            return "Invalid Password";
                        }
                    }
                    else
                    {
                        return "User not found";
                    }
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
            string siteUrl = _configuration["siteurl"];
            string adminList = _configuration["adminList"];
            string appId = _configuration["appId"];
            string appSecret = _configuration["appSecret"];

            try
            {
                using (ClientContext clientContext = new PnP.Framework.AuthenticationManager().GetACSAppOnlyContext(siteUrl, appId, appSecret))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(adminList);
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = $@"<View><Query><Where><Eq><FieldRef Name='Email' /><Value Type='Text'>{model.Email}</Value></Eq></Where></Query></View>";
                    ListItemCollection items = targetList.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();
                    if (items.Count == 0)
                    {
                        List list = clientContext.Web.Lists.GetByTitle(adminList);
                        ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                        ListItem listItem = list.AddItem(listItemCreationInformation);
                        listItem["Title"] = model.Name;
                        listItem["Email"] = model.Email;
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                        listItem["Password"] = hashedPassword;
                        listItem.Update();
                        clientContext.ExecuteQuery();
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
            string siteUrl = _configuration["siteurl"];
            string adminList = _configuration["adminList"];
            string appId = _configuration["appId"];
            string appSecret = _configuration["appSecret"];

            try
            {
                using (ClientContext clientContext = new PnP.Framework.AuthenticationManager().GetACSAppOnlyContext(siteUrl, appId, appSecret))
                {

                    List targetList = clientContext.Web.Lists.GetByTitle(adminList);
                   ListItem listItem=targetList.GetItemById(model.id);
                    listItem["IsDeleted"] =model.isApproved ;
                    listItem.Update();
                    clientContext.ExecuteQuery();
                    if(model.isApproved)
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
            string siteUrl = _configuration["siteurl"];
            string adminList = _configuration["adminList"];
            string appId = _configuration["appId"];
            string appSecret = _configuration["appSecret"];

            try
            {
                using (ClientContext clientContext = new PnP.Framework.AuthenticationManager().GetACSAppOnlyContext(siteUrl, appId, appSecret))
                {

                    List targetList = clientContext.Web.Lists.GetByTitle(adminList);
                    ListItem listItem = targetList.GetItemById(model.id);
                    listItem["IsApproved"] = model.IsDeleted;
                    listItem.Update();
                    clientContext.ExecuteQuery();
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion




        #region Generate JWT token
        private string GenerateToken(AdminTokenModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                    new Claim(JwtRegisteredClaimNames.Aud,_configuration["Jwt:Audience"]),
                    new Claim(JwtRegisteredClaimNames.Iss,_configuration["Jwt:Issuer"]),
                    new Claim(ClaimTypes.Name,model.Name),
                    new Claim(ClaimTypes.Role,model.Role),

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}

