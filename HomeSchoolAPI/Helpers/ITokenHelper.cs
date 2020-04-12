namespace HomeSchoolAPI.Helpers
{
    public interface ITokenHelper
    {
        bool IsValidateToken(string token);
        string GetIdByToken(string token);
    }
}