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
using System.Linq;

namespace TechnorucsWalkInAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly SharePointService _sharePointService;

        public AdminController(SharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }

        #region Approve Admin
        [HttpPost]
        [Route("ApproveAdmin")]
        public dynamic ApproveAdmin([FromBody] AdminApprovalModel model)
        {

            try
            {
                var users = _sharePointService.GetUserbyId(model.id);
                if (users.Count != 0)
                {
                    var user = users[0];
                    var response = _sharePointService.ApproveAdmin(model);
                    if (model.isApproved)
                    {
                        return Ok("The admin is Approved");
                    }
                    else
                    {
                        return Ok("The admin is Unapproved");
                    }
                }
                else
                {
                    return BadRequest("User Not Found");
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
                var users = _sharePointService.GetUserbyId(model.id);
                if (users.Count != 0)
                {
                    var user = users[0];
                    var response = _sharePointService.DeleteAdmin(model);
                    if (response)
                    {
                        return new
                        {
                            status = "The admin is Deleted"
                        };
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion


        #region
        [HttpGet]
        [Route("GetAllAdmin")]
        public dynamic GetAllAdmin()
        {
            try
            {
                ListItemCollection admins = _sharePointService.FetchUsers();
                if (admins == null)
                {

                    return new List<AdminModel>();
                }

                List<AdminModel> adminList = admins.Select(item => new AdminModel
                {
                    Id = item["ID"].ToString(),
                    Name = item["Title"].ToString(),
                    Email = item["Email"].ToString(),
                    IsApproved = Boolean.Parse(item["IsApproved"].ToString()),
                    IsDeleted = Boolean.Parse(item["IsDeleted"].ToString())
                }).ToList();
                return adminList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion





    }
}

