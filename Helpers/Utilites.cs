using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint.Client;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Helpers
{
    public class Utilites
    {
        private readonly JwtBearer _jwtBearer;
        private readonly SharePointService _sharePointService;


        public Utilites(JwtBearer jwtBearer, SharePointService sharePointService)
        {
            _jwtBearer = jwtBearer;
            _sharePointService = sharePointService;
        }



        public bool VerifyApproved(bool approvalstatus)
        {
            if (approvalstatus)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerifyPassword(string loginPassword, string storedPassword)
        {
            bool isMatch = BCrypt.Net.BCrypt.Verify(loginPassword, storedPassword);
            return isMatch;
        }


        public string GetToken(string name)
        {
            TokenModel tokenModel = new TokenModel();
            tokenModel.Name = name;
            tokenModel.Role = "Admin";
            var Token = _jwtBearer.GenerateToken(tokenModel);
            return Token;
        }
    }
}
