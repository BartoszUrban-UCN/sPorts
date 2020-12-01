using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebApplication.BusinessLogic.Shared
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession iSession, string key, T data)
        {
            string serializedData = JsonConvert.SerializeObject(data);
            iSession.SetString(key, serializedData);
        }
        public static T Get<T>(this ISession iSession, string key)
        {
            var data = iSession.GetString(key);
            return data != null ? JsonConvert.DeserializeObject<T>(data) : default(T);
        }
    }
}
