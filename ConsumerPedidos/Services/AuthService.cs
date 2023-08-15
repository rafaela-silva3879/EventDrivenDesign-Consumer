using ConsumerPedidos.Models.Requests;
using ConsumerPedidos.Models.Responses;
using ConsumerPedidos.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace ConsumerPedidos.Services
{
    public class AuthService
    {
        //ioptions: Para pegar o que está no appsettings.
        private readonly IOptions<CheckoutSettings>? _checkoutSettings;
        public AuthService(IOptions<CheckoutSettings>? checkoutSettings)
        {
            _checkoutSettings = checkoutSettings;
        }

        public async Task<AuthResponseModel> Post(AuthRequestModel request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request),
            Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.PostAsync
                                      ($"{_checkoutSettings.Value.ApiUrl}/auth", content);
                var builder = new StringBuilder();
                using (var r = result.Content)
                {
                    Task<string> task = r.ReadAsStringAsync();
                    builder.Append(task.Result);
                }
                return JsonConvert.DeserializeObject<AuthResponseModel>(builder.ToString());
            }
        }
    }
}
