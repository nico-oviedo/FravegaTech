using System.Net.Http.Json;

namespace SharedKernel.ServiceClients
{
    public class HttpServiceClient
    {
        private readonly HttpClient _http;

        protected HttpServiceClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
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
                var response = await _http.GetAsync(endpointUrl);
                if (!response.IsSuccessStatusCode)
                    return default;

                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
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
                var response = await _http.PostAsJsonAsync(endpointUrl, data);
                if (!response.IsSuccessStatusCode)
                    return default;

                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                return default;
            }
        }
    }
}
