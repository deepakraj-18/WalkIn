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
        private readonly SharePointService _sharePointService;
        private readonly string _adminList;
        private readonly JwtBearer _jwtBearer;
        private readonly Utilites _utilites;
        public LoginController(IConfiguration configuration, ClientContext clientContext, JwtBearer jwtBearer, SharePointService sharePointService, Utilites utilites)
        {
            _configuration = configuration;
            _clientContext = clientContext;
            _adminList = configuration["adminList"];
            _jwtBearer = jwtBearer;
            _sharePointService = sharePointService;
            _utilites = utilites;
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

                var users = _sharePointService.GetUserbyMail(model.Email);
                ListItem user = users[0];
                var isApproved = _utilites.VerifyApproved(Boolean.Parse(user["IsApproved"].ToString()));
                var isValidPassword = _utilites.VerifyPassword(model.Password, user["Password"].ToString());
                if (isApproved)
                {
                    if (isValidPassword)
                    {
                        var token = _utilites.GetToken(user["Title"].ToString());
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
                        return BadRequest("Password Incorrect");
                    }
                }
                else
                {
                    return BadRequest("Unapproved");
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
