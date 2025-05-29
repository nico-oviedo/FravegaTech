using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace SharedKernel.ServiceClients
{
    public class HttpServiceClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<HttpServiceClient> _logger;

        protected HttpServiceClient(HttpClient http, ILogger<HttpServiceClient> logger)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executes http get method
        /// </summary>
        /// <typeparam name="T">Response type object.</typeparam>
        /// <param name="endpointUrl">Endpoint url.</param>
        /// <returns>Response object.</returns>
        protected async Task<T?> GetAsync<T>(string endpointUrl)
        {
            try
            {
                _logger.LogInformation($"Executing GET method to {endpointUrl}.");
                var response = await _http.GetAsync(endpointUrl);
                _logger.LogInformation($"Response with status code: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                if (typeof(T) == typeof(string))
                {
                    var stringResult = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Response content: {stringResult}");
                    return (T)(object)stringResult;
                }

                var objectResult = await response.Content.ReadFromJsonAsync<T>();
                _logger.LogInformation($"Response content: {objectResult}");
                return objectResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to GET to {endpointUrl}. {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Executes http post method
        /// </summary>
        /// <typeparam name="TRequest">Request type object.</typeparam>
        /// <typeparam name="TResponse">Response type object.</typeparam>
        /// <param name="endpointUrl">Endpoint url.</param>
        /// <param name="data">Request data.</param>
        /// <returns>Response object.</returns>
        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpointUrl, TRequest data)
        {
            try
            {
                _logger.LogInformation($"Executing POST method to {endpointUrl}.");
                var response = await _http.PostAsJsonAsync(endpointUrl, data);
                _logger.LogInformation($"Response with status code: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                if (typeof(TResponse) == typeof(string))
                {
                    var stringResult = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Response content: {stringResult}");
                    return (TResponse)(object)stringResult;
                }

                var objectResult = await response.Content.ReadFromJsonAsync<TResponse>();
                _logger.LogInformation($"Response content: {objectResult}");
                return objectResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to POST to {endpointUrl}. {ex.Message}");
                return default;
            }
        }
    }
}
