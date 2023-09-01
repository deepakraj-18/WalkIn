namespace TechnorucsWalkInAPI.Models
{
    public class AdminLoginModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class AdminModel
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }      
        public Boolean IsApproved { get;  set; } 
        public Boolean IsDeleted { get;  set; } 

    }
    public class AdminRegisterModel
    {
        
        public string Name { get; set; }    
        public string Email { get; set; }
        public string Password { get; set; }
        public Boolean IsApproved { get;internal set; }=false;
        public Boolean IsDeleted { get;internal  set; } = false;

    }

    public class TokenModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }    
    }
    public class AdminApprovalModel
    {
        public int id { get; set; }
        public bool isApproved { get; set; }

    }
    public class AdminDeleteModel
    {
        public int id { get; set; }
        public bool IsDeleted { get; set; }

    }

}