using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;
using System.Reflection.PortableExecutable;

[ApiController]
[Route("api/[controller]")]
public class ProductSyncController : ControllerBase
{
    [HttpGet("GetAllProduct")]
    public IActionResult GetAllProduct()

    {
        var res2 = AuthenticateAsync();
        return BadRequest("Tenant ID is missing in the headers.");
    }
    static async Task<string> AuthenticateAsync()
    {
        using (var client = new HttpClient())
        {
            var authenticationApiUrl = "https://stg-zero.propertyproplus.com.au/api/TokenAuth/Authenticate";
            client.DefaultRequestHeaders.Add("Abp.TenantId", "10");
            var requestBody = new
            {
                userNameOrEmailAddress = "asif",
                password = "password1"
            };
            var jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(authenticationApiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<AuthenticationResponse>(responseContent);
                var accessToken = jsonResponse?.result?.accessToken;
                var ttt = GetAllpost(accessToken, 10);
                //var ttt4 = GetAllProducts(accessToken, 10);
                return jsonResponse?.result?.accessToken;
            }
            else
            {
                return null;
            }
        }
    }


    static async Task GetAllpost(string accessToken, int tenantId)
    {
        using (HttpClient client = new HttpClient())
        {
            // Set the Authorization header with the access token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Add Tenant ID to headers
            client.DefaultRequestHeaders.Add("Abp.TenantId", "10");

            // Replace the API URL with the actual endpoint
            string apiUrl = "https://stg-zero.propertyproplus.com.au/api/services/app/ProductSync/CreateOrEdit";

            // Replace 'yourRequestBody' with the actual request body in JSON format
            string requestBody = @"{
                ""tenantId"": 10,
                ""name"": ""Product 01"",
                ""description"": ""Product 01 description"",
                ""isAvailable"": false,
                ""id"": 22
            }";

            // Convert the request body to a StringContent
            StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // Make the POST or PATCH request
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read and parse the response content
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }


    static async Task GetAllProducts(string accessToken, int tenantId)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Add("Abp.TenantId", "10");
                string apiUrl = "https://stg-zero.propertyproplus.com.au/api/services/app/ProductSync/GetAllproduct";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }


    // DTO for deserializing authentication response
    public class AuthenticationResponse
    {
        public ResultObject result { get; set; }
        public string TargetUrl { get; set; }
        public bool Success { get; set; }
        public object Error { get; set; }
        public bool UnAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }

    public class ResultObject
    {
        public string accessToken { get; set; }
        public string encryptedAccessToken { get; set; }
        public int expireInSeconds { get; set; }
        public bool shouldResetPassword { get; set; }
        public object passwordResetCode { get; set; }
        public int userId { get; set; }
        public bool requiresTwoFactorVerification { get; set; }
        public object twoFactorAuthProviders { get; set; }
        public object twoFactorRememberClientToken { get; set; }
        public object returnUrl { get; set; }
        public string refreshToken { get; set; }
        public int refreshTokenExpireInSeconds { get; set; }
    }
}


