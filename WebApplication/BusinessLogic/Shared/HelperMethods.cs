using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace WebApplication.BusinessLogic.Shared
{
    public class HelperMethods
    {
        public static string Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }
    }
}
