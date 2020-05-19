namespace HomeSchoolAPI.DTOs
{
    public class UserForRegisterDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserCode { get; set; }
        public int UserRole { get; set; }
    }
}