using System.Text.RegularExpressions;

namespace ErgotronChatbotApi.Common.Utility
{
    public class Helper
    {
        public static string ExtractNo(string query)
        {
            // Use regex to extract serial number from the query
            var match = Regex.Match(query, @"\s+(\d+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
        public static string GetValue(Dictionary<string, string> dict, string key)
        {
            // Safely get value from dictionary
            return dict.TryGetValue(key, out var value) ? value : "false";
        }
    }
}
