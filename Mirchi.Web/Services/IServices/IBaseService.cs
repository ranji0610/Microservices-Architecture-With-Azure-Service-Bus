using Mirchi.Web.Models;

namespace Mirchi.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDTO ResponseModel { get; set; }

        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
