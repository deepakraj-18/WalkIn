using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    //[Authorize(Roles ="Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CanditateController : ControllerBase
    {
        private readonly SharePointService _sharePointService;

        public CanditateController(SharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }

        #region
        [HttpGet]
        [Route("GetAllCanditates")]
        public dynamic GetCanditates()
        {
            var canditateList = _sharePointService.GetAllCanditates();
            List<ViewCanditateModel> canditates = new List<ViewCanditateModel>();

            foreach (var c in canditateList)
            {
                string id = c["ID"].ToString();
                string name = c["Title"].ToString();
                string email = c["Email"].ToString();
                string phoneNumber = c["PhoneNumber"].ToString();
                string scoreOne = c["ScoreOne"].ToString() ?? "0";
                string scoreTwo = c["ScoreTwo"].ToString() ?? "0";

                canditates.Add(new ViewCanditateModel()
                {
                    ID = id,
                    Name = name,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    ScoreOne = scoreOne,
                    ScoreTwo = scoreTwo
                });
            }

            return Ok(canditates);
        }

        #endregion


        #region
        [HttpPost]
        [Route("GetCanditatesByInteviewID")]
        public dynamic GetCanditatesByInteviewId([FromBody] GetCanditateByInteviewIdModel model)
        {
            if(model.InterviewId == null)
            {
                return BadRequest("Interview Id is mandatory");
            }
            var response = _sharePointService.GetCanditatesByInteviewId(model.InterviewId, model.InterviewDate);
            var canditateList=new System.Collections.Generic.List<CanditateRegistrationModel>();   
            foreach(var c in response)
            {
                var canditates = new CanditateRegistrationModel
                {
                    Name = c["Name"].ToString(),
                    Email = c["Email"].ToString(),
                    PhoneNumber = c["PhoneNumber"].toString(),
                    City = c["City"].toString(),
                    Institute = c["Institute"].toString(),
                    Technology = c["Technology"].toString(),
                    Experience = c["Experience"].toString(),
                    Certification = c["Certification"].toString(),
                    Skills = c["Skills"].toString(),
                    Source = c["Source"].toString(),
                    Reference = c["Reference"].toString(),
                    Degree = c["Degree"].toString(),
                    Gender = c["Gender"].toString(),

                };
                canditateList.Add(canditates);

            }

            return canditateList    ;

        }
        #endregion


        #region
        [HttpPost]
        [Route("GetCanditateByEmail")]
        public dynamic GetCanditateByEmail([FromBody] GetCanditateByEmailModel model)
        {
            if(model == null||model.Email==null)
            {
                return BadRequest("Email is mandatory");
            }
            var response = _sharePointService.getCanditateByEmail(model.Email);
            if(response == null)
            {
                return "Canditate doesn't exists";
            }
            var data = response[0];
            var cantitate = new ViewCandidateModel
            {
                ID = data["ID"],
                Name = data["Title"],
                Email = data["Email"],
                PhoneNumber = data["PhoneNumber"],
                ScoreOne = data["ScoreOne"],
                ScoreTwo = data["ScoreTwo"]

            };

            return cantitate;
        }
        #endregion

        #region
        [HttpPost]
        [Route("GetCanditateById")]
        public dynamic GetCanditateById([FromBody] GetCanditateByIdModel model)
        {
            if (model == null || model.Id == null)
            {
                return BadRequest("Email is mandatory");
            }
            var response = _sharePointService.getCanditateByID(model.Id);
            if (response == null&&response.Count==0)
            {
                return "Canditate doesn't exists";
            }
            var data = response[0];
            var cantitate = new ViewCandidateModel
            {
                ID = data["ID"] ,
                Name = data["Title"],
                Email = data["Email"],
                PhoneNumber = data["PhoneNumber"],
                ScoreOne = data["ScoreOne"],
                ScoreTwo = data["ScoreTwo"]

            };
          
            return cantitate;
        }
        #endregion

    }
}
