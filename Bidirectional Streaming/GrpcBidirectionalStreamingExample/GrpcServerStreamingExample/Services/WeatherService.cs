using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GrpcBidirectionalStreamingExample
{

        public class WeatherService : Weather.WeatherBase
        {
            private readonly ILogger<WeatherService> _logger;

            public WeatherService(ILogger<WeatherService> logger)
            {
                _logger = logger;
            }

            public override async Task GetWeatherUpdates(IAsyncStreamReader<WeatherRequest> requestStream, IServerStreamWriter<WeatherResponse> responseStream, ServerCallContext context)
            {
                var tasks = new List<Task>();

                try
                {
                    await foreach (var request in requestStream.ReadAllAsync())
                    {
                        _logger.LogInformation($"Received request for weather update in {request.City}");

                        // Simulate generating weather updates
                        tasks.Add(Task.Run(async () =>
                        {
                            var rnd = new Random();
                            for (int i = 0; i < 5; i++)
                            {
                                var temperature = rnd.Next(10, 30) + rnd.NextDouble();
                                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                                await responseStream.WriteAsync(new WeatherResponse
                                {
                                    City = request.City,
                                    Temperature = (float)temperature,
                                    Timestamp = timestamp
                                });

                                await Task.Delay(rnd.Next(1000, 3000));
                            }
                        }));
                    }

                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing weather updates");
                    throw;
                }
            }
        }
}

