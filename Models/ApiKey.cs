namespace ResuMeAPI.Models
{
    public class ApiKey : BaseEntity
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public bool IsExpired { get; set; }
        public bool IsRefreshed { get; set; }
    }
}
