namespace TechnorucsWalkInAPI.Models
{
    public class OptionModel
    {
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
    }

    public class QuestionModel
    {
        public string QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string PatternType { get; set; }

        public List<OptionModel> Options { get; set; }
        public string Answer { get; set; }
        public Boolean IsDeleted { get; set; }
        public Boolean HasMultipleChoice { get; set; }
    }

    public class QuestionsModel
    {
        public string InterviewID { get; set; }
        public string PatternCount { get; set; }
        public List<QuestionModel> Questions { get; set; }
    }
    public class EditQuestionModel
    {
        public string QuestionId { get; set; }
        public string PatternCount { get; set; }
    }
    public class GetInterviewQuestionModel
    {
        public string InterviewId { get; set; }
    }
}
