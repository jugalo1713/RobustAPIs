using Microsoft.AspNetCore.Authorization;

namespace RobusAPI.WebApi.Attributes
{
    public class BasicAuthorizationAttribute: AuthorizeAttribute
    {
        public BasicAuthorizationAttribute()
        {
            Policy = "BasicAuthentication";
        }
    }
}
