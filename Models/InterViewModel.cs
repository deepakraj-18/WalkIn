namespace TechnorucsWalkInAPI.Models
{
    public class InterViewModel
    {

    }
    public class InterViewRegistrationModel
    {
        public string ID { get; internal set; }
        public string InterviewId { get; internal set; }
        public DateOnly Date { get; set; }
        public string Scoreone { get; set; }
        public string Scoretwo { get; set; }
        public string PatternCount { get; set; }    
        public Boolean isDeleted { get; internal set; } = false;


    }
    public class InterViewUpdateModel
    {
        public string ID { get; set; }
        public DateOnly Date { get; set; }
        public string Scoreone { get; set; }
        public string Scoretwo { get; set; }
        public Boolean isDeleted { get; internal set; } = false;


    }
    public class InterViewDeleteModel
    {
        public string ID { get; set; }
        public Boolean isDeleted { get; internal set; } = false;


    }
    public class GetInterviewByIdModel
    {
        public string InterviewId { get; set; }
    }
    public class GetInterviewByIdRespoonseModel
    {
        public string InterviewId { get; set; }
        public string ID { get; set; }
        public DateOnly Date { get; set; }
        public string Scoreone { get; set; }
        public string Scoretwo { get; set; }
        public Boolean isDeleted { get;  set; }
    }

}
