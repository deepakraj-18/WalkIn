using Microsoft.AspNetCore.Mvc;
using TechnorucsWalkInAPI.Helpers;
using TechnorucsWalkInAPI.Models;

namespace TechnorucsWalkInAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ExaminationController : ControllerBase
    {
        private readonly SharePointService _sharePointService;

        public ExaminationController(SharePointService sharePointService)
        {
            _sharePointService = sharePointService;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("SubmitAnswer")]
        public dynamic GetQuestion([FromBody] ExaminationModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(model.Answer==null&&model.Answer.Count==0)
            {
                return BadRequest("Answer can not be empty");
            }
            var isAnswerSaved = _sharePointService.SaveAnswer(model);
            var response = _sharePointService.ValidateAnswers(model);
            return response;
        }


    }
}
