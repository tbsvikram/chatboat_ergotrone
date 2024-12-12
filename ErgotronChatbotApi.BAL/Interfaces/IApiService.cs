using Newtonsoft.Json.Linq;

namespace ErgotronChatbotApi.BAL.Interfaces
{
    public interface IApiService
    {
        public Task<JArray> PostAsync(string storedProc, Dictionary<string, string> parameters);
    }
}
