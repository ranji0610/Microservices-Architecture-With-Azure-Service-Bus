using Mirchi.Web.Models;
using Mirchi.Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace Mirchi.Web.Services
{
    public class BaseService : IBaseService
    {
        public ResponseDTO ResponseModel { get; set; }

        public IHttpClientFactory HttpClientFactory { get; set; }

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            ResponseModel = new ResponseDTO();
            HttpClientFactory = httpClientFactory;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = HttpClientFactory.CreateClient("MirchiAPI");
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Headers.Add("Accept", "application/json");
                httpRequestMessage.RequestUri = new Uri(apiRequest.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                if (apiRequest.Data != null)
                {
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage httpResponseMessage = null;
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        {
                            httpRequestMessage.Method = HttpMethod.Post;
                            break;
                        }

                    case SD.ApiType.PUT:
                        {
                            httpRequestMessage.Method = HttpMethod.Put;
                            break;
                        }

                    case SD.ApiType.DELETE:
                        {
                            httpRequestMessage.Method = HttpMethod.Delete;
                            break;
                        }

                    default:
                        httpRequestMessage.Method = HttpMethod.Get;
                        break;
                }
                httpResponseMessage = await client.SendAsync(httpRequestMessage);
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<T>(content);
                return responseDto;
            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO
                {
                    DisplayMessage = "Error",
                    ErrorMessages = new List<string> { ex.Message },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var apiResponseDto = JsonConvert.DeserializeObject<T>(res);
                return apiResponseDto;
            }
        }
    }
}
