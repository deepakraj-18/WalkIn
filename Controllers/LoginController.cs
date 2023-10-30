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
    public class LoginController : ControllerBase
    {
        private readonly SharePointService _sharePointService;
        private readonly Utilites _utilites;
        public LoginController( SharePointService sharePointService, Utilites utilites)
        {
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
                if (users.Count==0)
                {
                    return BadRequest( "Un-Registered User");
                }
                ListItem user = users[0];
                var isDeleted = Boolean.Parse(user["IsDeleted"].ToString());
                var isApproved = _utilites.VerifyApproved(Boolean.Parse(user["IsApproved"].ToString()));
                var isValidPassword = _utilites.VerifyPassword(model.Password, user["Password"].ToString());
                if(isDeleted)
                {
                    return BadRequest("Deleted User");
                }
                if (isValidPassword)
                {
                    if (isApproved)
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
                            IsSuperAdmin = user["IsSuperAdmin"].ToString(),
                            Token = token
                        });
                    }
                    else
                    {
                        return BadRequest("Unapproved");
                    }
                }
                else
                {
                    return BadRequest("Password Incorrect");
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
