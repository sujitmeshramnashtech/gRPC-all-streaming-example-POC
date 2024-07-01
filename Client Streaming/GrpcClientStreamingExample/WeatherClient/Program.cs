using Grpc.Core;
using Grpc.Net.Client;
using GrpcClientStreamingExample;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrpcClientStreamingExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // For development, trust all certificates (e.g., self-signed)
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var channel = GrpcChannel.ForAddress("https://localhost:7045", new GrpcChannelOptions
            {
                HttpHandler = handler
            });

            var client = new Weather.WeatherClient(channel);

            // Example of client streaming
            using var streamingCall = client.RecordTemperatures();

            Console.WriteLine("Sending temperature readings...");

            var temperatures = new[]
            {
                new TemperatureReading { City = "New York", Temperature = 25, Timestamp = "2024-07-01T10:00:00Z" },
                new TemperatureReading { City = "New York", Temperature = 23, Timestamp = "2024-07-01T11:00:00Z" },
                new TemperatureReading { City = "New York", Temperature = 21, Timestamp = "2024-07-01T12:00:00Z" }
            };

            foreach (var temp in temperatures)
            {
                Console.WriteLine($"Sending temperature reading for {temp.City}: {temp.Temperature}°C");
                await streamingCall.RequestStream.WriteAsync(temp);
                await Task.Delay(1500); // Mimic delay in sending requests
            }

            await streamingCall.RequestStream.CompleteAsync();

            Console.WriteLine("Completed sending temperature readings.");

            var response = await streamingCall;

            Console.WriteLine($"Average Temperature for {response.City}: {response.AverageTemperature}°C");

            Console.ReadLine();
        }
    }
}
