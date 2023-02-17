using System.Globalization;
using System.Collections.Generic;

namespace My.Function
{
    // demo code, usually want to pull these from key vault or config etc.
    internal static class Constants
    {
        internal static string audience = "api://6a4fbbdc-8cca-4132-8810-4b1cadbe56e8/recommendation_read"; // Get this value from the expose an api, audience uri section example https://appname.tenantname.onmicrosoft.com
        internal static string clientID = "6a4fbbdc-8cca-4132-8810-4b1cadbe56e8"; // this is the client id, also known as AppID. This is not the ObjectID
        internal static string tenant = "jasondou25gmail.onmicrosoft.com"; // this is your tenant name
        internal static string tenantid = "f9c475e1-86b1-42dc-853b-201d5f6820ce"; // this is your tenant id (GUID)

        // rest of the values below can be left as is in most circumstances
        internal static string aadInstance = "https://login.microsoftonline.com/{0}/v2.0";
        internal static string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        internal static List<string> validIssuers = new List<string>()
        {
            $"https://login.microsoftonline.com/{tenant}/",
            $"https://login.microsoftonline.com/{tenant}/v2.0",
            $"https://login.windows.net/{tenant}/",
            $"https://login.microsoft.com/{tenant}/",
            $"https://sts.windows.net/{tenantid}/"
        };

        internal static class Columns
        {
            public static readonly string NUM_VOTES = "numVotes";
            public static readonly string TITLE_TYPE = "titleType";
            public static readonly string PRIMARY_TITLE = "primaryTitle";
            public static readonly string SCORE = "Score";
            public static readonly string START_YEAR = "startYear";
            public static readonly string RUNTIME_MINUTES = "runtimeMinutes";
            public static readonly string GENRES = "genres";
        }
        internal static HashSet<string> moviesHeaders = new HashSet<string>()
        {
            Columns.NUM_VOTES,
            Columns.TITLE_TYPE,
            Columns.PRIMARY_TITLE,
            Columns.SCORE,
            Columns.START_YEAR,
            Columns.RUNTIME_MINUTES,
            Columns.GENRES,
        };

        internal static class RequiredRoles
        {
            public static readonly string RECOMMENDATION_READ = "Recommendation.Read";
        }
    }
}