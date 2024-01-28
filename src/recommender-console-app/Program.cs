// See https://aka.ms/new-console-template for more information
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault.Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var settings = Settings.LoadSettings();

// Scope for Azure Function
var scopes = new[] { $"{settings.ResourceId}/.default" };
var keyVaultUri = new Uri($"https://{settings.KeyVaultName}.vault.azure.net/");

// Retrieve certificate from Azure Key Vault
var credential = new DefaultAzureCredential();
var keyVaultClient = new SecretClient(keyVaultUri, credential);
var secretResponse = await keyVaultClient.GetSecretAsync(settings.CertificateName);
var keyVaultSecret = secretResponse?.Value ?? throw new Exception($"Unable to retrieve secret {settings.CertificateName} from key vault {settings.KeyVaultName}");
var privateKeyBytes = Convert.FromBase64String(keyVaultSecret.Value);
var certificate = new X509Certificate2(privateKeyBytes);

// Retrieve access token
var clientCertificateCredential = new ClientCertificateCredential(settings.TenantId, settings.ClientId, certificate);
var context = new TokenRequestContext(scopes);
var tokenResponse = await clientCertificateCredential.GetTokenAsync(context);

// Call Azure function with HTTP
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

var response = await httpClient.GetAsync($"https://{settings.FunctionName}.azurewebsites.net/api/recommendation");
if (response.IsSuccessStatusCode)
{
    string responseContent = await response.Content.ReadAsStringAsync();

    JArray jsonArray = JArray.Parse(responseContent);
    JArray firstFive = new JArray(jsonArray.Take(5));
    foreach (JObject obj in firstFive)
    {
        Console.WriteLine(obj);
    }
}
else
{
    // TODO: Handle the error
    Console.WriteLine($"Error with http response");
}
