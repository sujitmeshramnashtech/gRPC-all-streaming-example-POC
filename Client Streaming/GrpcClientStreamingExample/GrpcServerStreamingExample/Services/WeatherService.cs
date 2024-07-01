using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GrpcClientStreamingExample;

namespace GrpcClientStreamingExample
{

    public class WeatherService : Weather.WeatherBase
    {
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(ILogger<WeatherService> logger)
        {
            _logger = logger;
        }

        public override async Task<TemperatureSummary> RecordTemperatures(IAsyncStreamReader<TemperatureReading> requestStream, ServerCallContext context)
        {
            var temperatures = new List<int>();
            string city = null;

            await foreach (var reading in requestStream.ReadAllAsync())
            {
                if (city == null)
                {
                    city = reading.City;
                }

                temperatures.Add(reading.Temperature);

                _logger.LogInformation($"Received temperature reading for {reading.City}: {reading.Temperature}°C");
            }

            var averageTemperature = temperatures.Count > 0 ? temperatures.Average() : 0;

            _logger.LogInformation($"Calculating average temperature for {city}: {averageTemperature}°C");

            return new TemperatureSummary
            {
                City = city,
                AverageTemperature = (float)averageTemperature
            };
        }
    }
}
