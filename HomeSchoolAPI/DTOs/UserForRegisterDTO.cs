namespace HomeSchoolAPI.DTOs
{
    public class UserForRegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserCode { get; set; }
        public int UserRole { get; set; }
    }
}