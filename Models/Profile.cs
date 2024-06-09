namespace ResuMeAPI.Models
{
    public class Profile : BaseEntity
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName 
        { 
            get 
            { 
                return $"{FirstName} {LastName}"; 
            } 
        }
        public string? Email { get; set; }
        public string? Occupation { get; set; }
    }

    public class ProfileDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Occupation { get; set; }
    }
}
