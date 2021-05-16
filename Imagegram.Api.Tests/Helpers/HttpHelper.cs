using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Imagegram.Api.Tests
{
    public static class HttpHelper
    {
        public static Task<HttpResponseMessage> PostJson<T>(this HttpClient httpClient, string requestUri, T payload, Guid accountId)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var content = new StringContent(
               JsonSerializer.Serialize(payload, options),
               Encoding.UTF8,
               "application/json"
               );
            httpClient.DefaultRequestHeaders.Add("X-Account-Id", accountId.ToString());
            return httpClient.PostAsync(requestUri, content);
        }

        public static Task<HttpResponseMessage> PostJson<T>(this HttpClient httpClient, string requestUri, T payload)
        {
            return PostJson<T>(httpClient, requestUri, payload, Guid.Empty);
        }

        public static Task<HttpResponseMessage> Get(this HttpClient httpClient, string requestUri, Guid accountId)
        {
            httpClient.DefaultRequestHeaders.Add("X-Account-Id", accountId.ToString());
            return httpClient.GetAsync(requestUri);
        }

        public static Task<HttpResponseMessage> PostImage(
            this HttpClient httpClient, 
            string requestUri, 
            Stream imageData, 
            Guid accountId)
        {            
            var content = new StreamContent(imageData);            
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                Name = "file",
                FileName = "file"
            };
            httpClient.DefaultRequestHeaders.Add("X-Account-Id", accountId.ToString());            

            return httpClient.PostAsync(requestUri, content);
        }

        public static async Task<T> GetBody<T>(this HttpResponseMessage responseMessage)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<T>(content, options);
        }
    }

}
