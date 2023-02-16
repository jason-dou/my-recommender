using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using System.Web.Http;

namespace My.Function
{
    public static class Recommendation
    {
        [FunctionName("recommendation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("Processing a request for recommendation");
            try
            {
                var isAuthorized = await IsAuthorized(req, log);
                if (isAuthorized)
                {
                    var path = System.IO.Path.Combine(context.FunctionAppDirectory, "movies.pred.tsv");

                    // Read the TSV file into a list of dictionaries
                    List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string[] headers = reader.ReadLine().Split('\t');
                        while (!reader.EndOfStream)
                        {
                            string[] fields = reader.ReadLine().Split('\t');
                            Dictionary<string, string> row = new Dictionary<string, string>();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                if (Constants.moviesHeaders.Contains(headers[i]))
                                {
                                    var field = headers[i] == Constants.Columns.GENRES ? fields[i].Replace("\"", "") : fields[i];
                                    row.Add(headers[i], field);
                                }

                            }
                            data.Add(row);
                        }
                    }

                    // Convert the list of dictionaries to JSON
                    string response = JsonConvert.SerializeObject(data, Formatting.Indented);

                    return new OkObjectResult(response);
                }
                else
                {
                    return (ActionResult)new UnauthorizedResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return (ActionResult)new InternalServerErrorResult();
            }


        }

        private static async Task<Boolean> IsAuthorized(HttpRequest req, ILogger log)
        {
            var role = Constants.RequiredRoles.RECOMMENDATION_READ;
            var accessToken = GetAccessToken(req);
            var claimsPrincipal = await ValidateAccessToken(accessToken, log);

            return claimsPrincipal != null && claimsPrincipal.IsInRole(role);
        }

        private static string GetAccessToken(HttpRequest req)
        {
            var authorizationHeader = req.Headers?["Authorization"];
            string[] parts = authorizationHeader?.ToString().Split(null) ?? new string[0];
            if (parts.Length == 2 && parts[0].Equals("Bearer"))
                return parts[1];
            return null;
        }

        private static async Task<ClaimsPrincipal> ValidateAccessToken(string accessToken, ILogger log)
        {
            var audience = Constants.audience;
            var clientID = Constants.clientID;
            var tenant = Constants.tenant;
            var tenantid = Constants.tenantid;
            var aadInstance = Constants.aadInstance;
            var authority = Constants.authority;
            var validIssuers = Constants.validIssuers;

            // Debugging purposes only, set this to false for production
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            ConfigurationManager<OpenIdConnectConfiguration> configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());

            OpenIdConnectConfiguration config = null;
            config = await configManager.GetConfigurationAsync();

            ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();

            // Initialize the token validation parameters
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                // App Id URI and AppId of this service application are both valid audiences.
                ValidAudiences = new[] { audience, clientID },

                // Support Azure AD V1 and V2 endpoints.
                ValidIssuers = validIssuers,
                IssuerSigningKeys = config.SigningKeys
            };

            try
            {
                SecurityToken securityToken;
                var claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out securityToken);
                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
            }
            return null;
        }
    }
}
