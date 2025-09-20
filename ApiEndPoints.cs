namespace Nash_Manassas;

public class ApiEndPoints
{
    private const string ApiBase = "api";


    public static class Users
    {
        public const string Base = $"{ApiBase}/users";
        
        public const string LOGIN_URL_USER_CONSTANT = $"login";
        
        public const string LOGOUT_URL_USER_CONSTANT = $"logout";
        
        public const string GET_URL_USER_CONSTANT = $"{{id:guid}}";
    }
}