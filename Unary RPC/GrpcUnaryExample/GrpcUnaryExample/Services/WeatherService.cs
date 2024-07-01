using Grpc.Core;
using GrpcUnaryExample;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GrpcUnaryExample
{
    public class WeatherService : Weather.WeatherBase
    {
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(ILogger<WeatherService> logger)
        {
            _logger = logger;
        }

        public override Task<WeatherResponse> GetWeather(WeatherRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Received request for weather in {request.City}");

            // Simulate generating weather data
            var rnd = new Random();
            var temperature = rnd.Next(10, 30) + rnd.NextDouble();
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var response = new WeatherResponse
            {
                City = request.City,
                Temperature = (float)temperature,
                Timestamp = timestamp
            };

            _logger.LogInformation($"Sending weather response for {request.City}: {response.Temperature}°C at {response.Timestamp}");

            return Task.FromResult(response);
        }
    }
}
