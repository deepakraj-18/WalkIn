using Microsoft.AspNetCore.Mvc;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    [ApiController]
    
    public class ExaminationController : ControllerBase
    {
        private readonly SharePointService _sharePointService;

        public ExaminationController(SharePointService sharePointService)
        {
            _sharePointService = sharePointService;

        }


        [HttpPost("api/[controller]/GetQuestion")]
        public int GetQuestion([FromBody] ExaminationModel model)
        {
            var score = _sharePointService.ValidateAnswers(model);
            return score;
        }
        
    }
}
