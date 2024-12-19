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
        public static Dictionary<string, string> ExtractTime(string query)
        {
            // Initialize a dictionary to hold time details
            var timeSpan = new Dictionary<string, string>();

            // Regex to extract time and AM/PM from the query
            var pattern = @"(\d{1,2}:\d{2})(AM|PM)?";
            var match = Regex.Match(query, pattern, RegexOptions.IgnoreCase);

            // Populate dictionary if a match is found
            if (match.Success)
            {
                timeSpan["time"] = match.Groups[1].Value;
                timeSpan["am/pm"] = match.Groups[2].Success ? match.Groups[2].Value : string.Empty;
            }

            return timeSpan;
        }
        public static string GetValue(Dictionary<string, string> dict, string key)
        {
            // Safely get value from dictionary
            return dict.TryGetValue(key, out var value) ? value : "false";
        }

        public static string ExtractDate(string query)
        {
            string pattern = @"\d{2}/\d{2}/\d{4}";
            Match match = Regex.Match(query, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[0].Value : string.Empty;
        }
    }
}
