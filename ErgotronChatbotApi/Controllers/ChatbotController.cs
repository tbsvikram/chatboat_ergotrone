using ErgotronChatbotApi.BAL.Interfaces;
using ErgotronChatbotApi.Common.Constant;
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

        /// <summary>
        /// Handles the retrieval of an AI-generated answer to a given question and returns a chatbot response.
        /// </summary>
        /// <param name="siteId">The optional site ID associated with the query.</param>
        /// <param name="userId">The optional user ID associated with the query.</param>
        /// <param name="question">The question to be answered.</param>
        /// <returns>
        /// An `IActionResult` containing the response model with the AI-generated answer 
        /// or an appropriate message if no answer is found.
        /// </returns>
        [HttpPost("get-question-answer")]
        public async Task<IActionResult> GetQuestionAnswerAsync([FromForm] int? siteId, [FromForm] string? userId, [FromForm] string? question)
        {
            var response = new ResponseModel<string>();

            // Get answer from AI service
            var answerResponse = await _questionAnswerAIService.AzureAIServiceAsync(question);

            // Check if answer exists
            if (string.IsNullOrEmpty(answerResponse.Response.answer))
            {
                response.Response = answerResponse.Message;
                return Ok(response);
            }

            // Set query and get chatbot response
            answerResponse.Response.query = question ?? string.Empty;
            response = await _chatbotService.GetQuestionResponseAsync(siteId, userId, answerResponse.Response);

            // Check if chatbot response is empty
            if (string.IsNullOrEmpty(response.Response))
            {
                response.Response = "No answer found. Try another prompt.";
                response.ResponseCode = (int)Enums.StatusCode.OK;
            }

            return Ok(response);
        }

    }
}
