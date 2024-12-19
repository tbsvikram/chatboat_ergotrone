using ErgotronChatbotApi.Model;

namespace ErgotronChatbotApi.BAL.Interfaces
{
    public interface IChatbotService
    {
        public Task<ResponseModel<string>> GetQuestionResponseAsync(int? siteId, string? userId, QuestionAnswerResponse questionAnswerResponse);
    }
}
