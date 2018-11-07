using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Vouchers.IntegrationTests
{
    public class WebApiClient : HttpClient
    {
        public string BaseUrl { get; } = "http://vouchers-web-api:5000/api/vouchers/";
        private HttpClient client;

        public WebApiClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<HttpResponseMessage> CreateVoucher(string json)
        {
            var httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(BaseUrl), httpContent);

            return response;
        }

        public async Task<HttpResponseMessage> GetVoucher(string voucherCode)
        {
            var response = await client.GetAsync(new Uri(BaseUrl + voucherCode));

            return response;
        }

        public async Task<HttpResponseMessage> PayWithVoucher(string json)
        {
            var httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(BaseUrl + "payment"), httpContent);

            return response;
        }
    }
}
