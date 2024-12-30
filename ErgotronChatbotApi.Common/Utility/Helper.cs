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
            // Pattern to match MM/DD/YYYY format
            string pattern1 = @"\d{2}/\d{2}/\d{4}";

            // Pattern to match Month DD, YYYY format
            string pattern2 = @"[A-Za-z]+ \d{1,2}, \d{4}";

            // Try to match both patterns
            Match match1 = Regex.Match(query, pattern1, RegexOptions.IgnoreCase);
            if (match1.Success)
                return match1.Groups[0].Value;

            Match match2 = Regex.Match(query, pattern2, RegexOptions.IgnoreCase);
            if (match2.Success)
            {
                // Optional: Convert "Month DD, YYYY" to "MM/DD/YYYY" format if needed
                DateTime dateValue = DateTime.Parse(match2.Groups[0].Value);
                return dateValue.ToString("MM/dd/yyyy");
            }

            return string.Empty;
        }
    }
}
