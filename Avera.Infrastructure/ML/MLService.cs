using System.Net.Http.Json;
using System.Net.Mime;
using Avera.Application.Abstractions.ML;
using Avera.Application.ML.Health;
using Avera.Application.ML.Process;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Avera.Infrastructure.ML
{
    public class MLService : IMLService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MLService> _logger;

        public MLService(HttpClient httpClient, IConfiguration configuration, ILogger<MLService> logger)
        {
            httpClient.BaseAddress = new Uri(configuration["MLApi:Uri"]!);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);

            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GetMLHealthResponse> GetMLHealthAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<GetMLHealthResponse>(cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize ML health response.");
        }

        public async Task<ProcessMLResponse> ProcessAsync(ProcessRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sending process request for Case with Name: {CaseName}", request.CaseName);
            
            var response = await _httpClient.PostAsJsonAsync("/process", request, cancellationToken);

            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Received process response for Case with Name: {CaseName} with status: {Status}", request.CaseName, response.StatusCode);
            return await response.Content.ReadFromJsonAsync<ProcessMLResponse>(cancellationToken) ?? 
                   throw new InvalidOperationException("Failed to deserialize ML process response.");
        }
    }
}