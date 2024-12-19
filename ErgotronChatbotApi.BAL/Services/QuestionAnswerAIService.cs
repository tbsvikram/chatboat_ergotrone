using ErgotronChatbotApi.BAL.Interfaces;
using ErgotronChatbotApi.Common.Constant;
using ErgotronChatbotApi.Common.Constants;
using ErgotronChatbotApi.Common.Utility;
using ErgotronChatbotApi.Model;

using Microsoft.Extensions.Logging;

using System.Text;
using System.Text.Json;

/// <summary>
/// Provides a concrete implementation of the <see cref="IQuestionAnswerAIService"/> interface for handling AI-based question and answer processing.
/// </summary>
/// <remarks>.
/// This class facilitates interactions with AI services, such as processing user queries and fetching relevant answers. 
/// It also integrates logging to capture information or errors during the question-answer operations.
/// </remarks>
public class QuestionAnswerAIService : IQuestionAnswerAIService
{
    private readonly HttpClient _httpClient; // Used for sending HTTP requests to AI-based APIs
    private readonly ILogger<QuestionAnswerAIService> _logger; // Logs activities and errors for AI question-answer operations

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionAnswerAIService"/> class.
    /// Configures the HTTP client with the base address and subscription key.
    /// </summary>
    /// <param name="logger">The logger instance for logging errors and debug information.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
    public QuestionAnswerAIService(ILogger<QuestionAnswerAIService> logger)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Urls.azureAIUrl)
        };
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AppSettings.AzureServiceKey);

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    /// <summary>
    /// Sends a question to the AI service and retrieves the best possible answer.
    /// Handles dynamic URL construction, request payload creation, and response processing.
    /// Logs errors and handles exceptions gracefully.
    /// </summary>
    /// <param name="question">The question to be sent to the AI service.</param>
    /// <returns>A string containing the AI-generated answer or an error message.</returns
    public async Task<ResponseModel<QuestionAnswerResponse>> AIPostAsync(string question)
    {
        ResponseModel<QuestionAnswerResponse> responseModel = new();
        QuestionAnswerResponse questionAnswerResponse = new();
        HttpResponseMessage response = new();
        string responseBody = string.Empty;

        if (string.IsNullOrWhiteSpace(question))    
        {
            questionAnswerResponse.answer = string.Empty;

            responseModel.Response = questionAnswerResponse;
            responseModel.ResponseCode = (int)Enums.StatusCode.NotFound;
            responseModel.Message = $"Question cannot be null or empty";

            return responseModel;
        }

        // Define the request body
        var requestBody = new
        {
            top = 3,
            question,
            includeUnstructuredSources = true,
            confidenceScoreThreshold = 0.3,
            answerSpanRequest = new
            {
                enable = false,
                topAnswersWithSpan = 1,
                confidenceScoreThreshold = 0.3
            }
        };

        try
        {
            // Construct the dynamic request URL
            string requestUrl = $"language/:query-knowledgebases?projectName={Uri.EscapeDataString(AppSettings.ProjectName)}&api-version={Uri.EscapeDataString(AppSettings.ApiVersion)}&deploymentName={Uri.EscapeDataString(AppSettings.DeploymentName)}";

            // Send the HTTP POST request
            response = await _httpClient.PostAsync(
                requestUrl,
                new StringContent(JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                MediaType.jsonType)
                );

            string responseContent = await response.Content.ReadAsStringAsync();

            // Process the response
            if (response.IsSuccessStatusCode)
            {

                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (result.TryGetProperty("answers", out JsonElement answers) && answers.GetArrayLength() > 0)
                {
                    responseBody = answers[0].GetProperty("answer").GetString() ?? "No answer found.";

                    if (answers[0].TryGetProperty("metadata", out JsonElement metadata))
                    {
                        if (metadata.ValueKind == JsonValueKind.Array)
                        {
                            foreach (JsonElement meta in metadata.EnumerateArray())
                            {
                                if (meta.TryGetProperty("key", out JsonElement key))
                                {
                                    questionAnswerResponse.metaData.Add(meta.GetProperty("key").GetString(), meta.GetProperty("value").GetString());
                                }
                            }
                        }
                        else if (metadata.ValueKind == JsonValueKind.Object)
                        {
                            foreach (var property in metadata.EnumerateObject())
                            {
                                questionAnswerResponse.metaData.Add(property.Name, property.Value.GetString());
                            }
                        }
                    }
                }
                else
                {
                    responseBody = "No answer found.";
                }

                questionAnswerResponse.answer = responseBody;

                

                responseModel.Response = questionAnswerResponse;
                responseModel.ResponseCode = (int)Enums.StatusCode.OK;
                responseModel.Message = "Success";
            }
            else
            {
                _logger.LogError("Request failed with status code {StatusCode} and content: {ResponseContent}",
                                 response.StatusCode, responseContent);

                questionAnswerResponse.answer = string.Empty;
                responseModel.Response = questionAnswerResponse;
                responseModel.ResponseCode = (int)Enums.StatusCode.NotFound;
                responseModel.Message = $"Request failed with status code {response.StatusCode} and content: {responseContent}";
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed: {Message}", ex.Message);
            questionAnswerResponse.answer = string.Empty;

            responseModel.Response = questionAnswerResponse;
            responseModel.ResponseCode = (int)Enums.StatusCode.BadRequest;
            responseModel.Message = $"HTTP request failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
            questionAnswerResponse.answer = string.Empty;

            responseModel.Response = questionAnswerResponse;
            responseModel.ResponseCode = (int)Enums.StatusCode.InternalError;
            responseModel.Message = $"Unexpected error: {ex.Message}";
            responseModel.Message = "not get Expected Answer how  i can help You";
           
        }

        return responseModel;
    }
}