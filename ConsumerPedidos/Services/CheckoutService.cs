using ConsumerPedidos.Models.Requests;
using ConsumerPedidos.Models.Responses;
using ConsumerPedidos.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ConsumerPedidos.Services
{
    public class CheckoutService
    {
        private readonly IOptions<CheckoutSettings>? _checkoutSettings;
        public CheckoutService(IOptions<CheckoutSettings>? checkoutSettings)
        {
            _checkoutSettings = checkoutSettings;
        }

        public async Task<CheckoutResponseModel> Post
                                        (CheckoutRequestModel request, string accessToken)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request),
                                                                Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new 
                                    AuthenticationHeaderValue("Bearer", accessToken);
                var result = await httpClient.PostAsync
                                        ($"{_checkoutSettings.Value.ApiUrl}/checkout", content);
                var builder = new StringBuilder();
                using (var r = result.Content)
                {
                    Task<string> task = r.ReadAsStringAsync();
                    builder.Append(task.Result);
                }
                return JsonConvert.DeserializeObject
                <CheckoutResponseModel>(builder.ToString());
            }
        }
    }
}