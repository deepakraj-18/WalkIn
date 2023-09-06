namespace TechnorucsWalkInAPI.Models
{
    public class ExaminationModel
    {
        public List<ExamAnswerModel> Answer { get; set; }
    }
    public class ExamAnswerModel
    {
        public string QuestionId { get; set; }
        public string Answer { get; set; }
    }
}
