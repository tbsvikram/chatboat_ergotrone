namespace ErgotronChatbotApi.Model
{
    public class QuestionAnswerResponse
    {
        public string answer { get; set; } = string.Empty;
        public string query { get; set; } = string.Empty;
        public Dictionary<string, string> metaData { get; set; } = new Dictionary<string, string>();
    }
}
