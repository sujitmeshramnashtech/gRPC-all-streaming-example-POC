using Grpc.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using GrpcServerStreamingExample;

namespace GrpcServerStreamingExample.Services
{
    public class WeatherService : Weather.WeatherBase
    {
        // mimicing the weather data
        private readonly List<WeatherResponse> _weatherData = new List<WeatherResponse>
        {
            new WeatherResponse { City = "New York", Description = "Sunny", Temperature = 25, Timestamp = "2024-07-01T10:00:00Z" },
            new WeatherResponse { City = "New York", Description = "Cloudy", Temperature = 23, Timestamp = "2024-07-01T11:00:00Z" },
            new WeatherResponse { City = "New York", Description = "Rainy", Temperature = 21, Timestamp = "2024-07-01T12:00:00Z" },
            new WeatherResponse { City = "New York", Description = "Sunny", Temperature = 29, Timestamp = "2024-07-01T8:00:00Z" },
            new WeatherResponse { City = "New York", Description = "Rainy", Temperature = 21, Timestamp = "2024-07-01T9:00:00Z" },


        };

        public override async Task GetWeatherUpdates(WeatherRequest request, IServerStreamWriter<WeatherResponse> responseStream, ServerCallContext context)
        {
            foreach (var weather in _weatherData)
            {
                // Check for cancellation
                if (context.CancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // Simulate delay
                await Task.Delay(5000);

                // Send the weather update
                await responseStream.WriteAsync(weather);
            }
        }
    }
}
