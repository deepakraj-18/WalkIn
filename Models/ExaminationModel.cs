namespace TechnorucsWalkInAPI.Models
{
    public class ExaminationModel
    {
        public string InterviewId { get; set; }
        public string CanditateEmail {  get; set; }
        public List<ExamAnswerModel> Answer { get; set; }
    }
    public class ExamAnswerModel
    {
        public string QuestionId { get; set; }
        public string Answer { get; set; }
    }
    public class ExaminationQuestionModel
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public List<OptionsModel> Options { get; set; }

    }

    public class OptionsModel
    {
        public string OptionsOne { get; set; }
        public string OptionsTwo { get; set; }
        public string OptionsThree { get; set; }
        public string OptionsFour { get; set; }

    }
}
