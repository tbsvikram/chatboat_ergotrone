using ErgotronChatbotApi.BAL.Interfaces;
using ErgotronChatbotApi.Common.Constants;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net.Http.Headers;
using System.Text;


namespace ErgotronChatbotApi.BAL.Services
{
    /// <summary>
    /// Provides a concrete implementation of the <see cref="IApiService"/> interface for interacting with external APIs.
    /// </summary>
    /// <remarks>
    /// This class manages HTTP client interactions, including sending requests and receiving responses. 
    /// It also integrates logging to capture information or errors during API operations.
    /// </remarks>
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient; // Used for sending HTTP requests
        private readonly ILogger<ApiService> _logger; // Logs API service activities and errors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiService"/> class.
        /// Configures the HTTP client with the base address and default headers.
        /// </summary>
        /// <param name="logger">The logger instance for logging errors and debug information.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        public ApiService(ILogger<ApiService> logger)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(Urls.apiUrl)
            };

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MediaType.jsonType));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends a POST request to the API and retrieves a JSON array response.
        /// </summary>
        /// <param name="storedProc">The stored procedure name to include in the request parameters.</param>
        /// <param name="parameters">The dictionary of parameters to include in the request body.</param>
        /// <returns>A <see cref="JArray"/> containing the API response data, or null if an error occurs.</returns>
        public async Task<JArray> PostAsync(string storedProc, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(storedProc))
            {
                throw new ArgumentException("Stored procedure name cannot be null or empty.", nameof(storedProc));
            }

            JArray? dt = null;

            // Initialize parameters if null
            parameters ??= [];

            try
            {
                parameters.Add("storedProcName", storedProc);

                HttpResponseMessage response = await _httpClient.PostAsync(Urls.requestUrl, new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, MediaType.jsonType));

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content into a JArray
                    string data = await response.Content.ReadAsStringAsync();
                    dt = JArray.Parse(data);
                }
                else
                {
                    _logger.LogError("Request failed with status code {StatusCode} and content: {Content}",
                                     response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
            }
            return dt;
        }

    }
}
