using System.Runtime.Serialization;

namespace RobusAPI.Integrations.CoursesAndStudentsApiV2
{
    public class AuthorizationException: ApiException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public AuthorizationException()
        {
        }

        public AuthorizationException(string message) : base(message)
        {
        }

        public AuthorizationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AuthorizationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
