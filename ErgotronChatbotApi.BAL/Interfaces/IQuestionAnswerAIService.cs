﻿using ErgotronChatbotApi.Model;

namespace ErgotronChatbotApi.BAL.Interfaces
{
    public interface IQuestionAnswerAIService
    {
        public Task<ResponseModel<QuestionAnswerResponse>> AzureAIServiceAsync(string? question);
    }
}