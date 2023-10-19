namespace TechnorucsWalkInAPI.Models
{
    public class CanditateRegistrationModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Institute { get; set; }
        public string Technology { get; set; }
        public string Experience { get; set; }
        public string Certification { get; set; }
        public string Skills { get; set; }
        public string Source { get; set; }
        public string Reference { get; set; }
        public string Degree { get; set; }
        public string Gender { get; set; }
        public string PatternID { get; internal set; }
        public string InterviewDate { get; internal set; }
        public string InterviewID { get; internal set; }

    }
    public class CanditatesList
    {
        public List<ViewCanditateModel> Canditates { get; set; }
    }
    public class ViewCanditateModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ScoreOne { get; set; }
        public string ScoreTwo { get; set; }
        public Boolean Result { get; set; }


    }
    public class RegistrationResponse
    {
        public string Status { get; set; }
        public string InterviewId { get; set; }
        public string CanditateEmail { get; set; }
        public List<ExaminationQuestionModel> Questions { get; set; }
    }
    public class GetCanditateByEmailModel
    {
        public string Email { get; set; }
    }
    public class GetCanditateByIdModel

    {
        public string Id { get; set; }
    }


    public class ViewCandidateModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ScoreOne { get; set; }
        public string ScoreTwo { get; set; }
        public string City { get; set; }
        public string Institute { get; set; }
        public string Technology { get; set; }
        public string Experience { get; set; }
        public string Certification { get; set; }
        public string Skills { get; set; }
        public string Source { get; set; }
        public string Reference { get; set; }
        public string Degree { get; set; }
        public string Gender { get; set; }
        public string InterviewID { get; set; }
        public bool Result { get; set; }
        public List<ViewAnswerModel> Answers { get; set; }
    }
}
