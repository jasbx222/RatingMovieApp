using System;
using System.Threading.Tasks;

namespace MovieRatingAPI.Interface
{
    public interface ICacheService
    {
        Task<T?> GetData<T>(string key);

        Task SetData<T>(
            string key,
            T data,
            TimeSpan expiration);

        Task RemoveData(string key);
    }
}