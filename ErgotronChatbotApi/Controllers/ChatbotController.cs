using ErgotronChatbotApi.BAL.Interfaces;
using ErgotronChatbotApi.Model;

using Microsoft.AspNetCore.Mvc;

namespace ErgotronChatbotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        private readonly IQuestionAnswerAIService _questionAnswerAIService;

        public ChatbotController(IChatbotService chatbotService, IQuestionAnswerAIService questionAnswerAIService)
        {
            _chatbotService = chatbotService;
            _questionAnswerAIService = questionAnswerAIService;
        }

        [HttpPost("get-question-answer")]
        public async Task<IActionResult> GetQuestionAnswerAsync(int? siteId, string? userId, string? question)
        {
            var response = new ResponseModel<string>();
            var answerResponse = await _questionAnswerAIService.AIPostAsync(question);
            if (!string.IsNullOrEmpty(answerResponse.Response.answer))
            {
                answerResponse.Response.query = question;
                response = await _chatbotService.GetQuestionResponseAsync(siteId, userId, answerResponse.Response);
            }
            else
            {
                response.Message = answerResponse.Message;
            }
            return Ok(response);

            //QuestionAnswerResponse questionAnswerResponse = new()
            //{
            //    answer = "dbo.PrcGetWorkstationUsers",
            //    query = question,
            //    metaData = { { "is_count_user", "false" }, { "is_siteid", "true" }, { "is_userid", "true" } }

            //};

            //var response = await _chatbotService.GetQuestionResponseAsync(siteId, userId, questionAnswerResponse);

            //return Ok(response);
        }
    }
}
