using Grpc.Core;
using Grpc.Net.Client;
using GrpcBidirectionalStreamingExample;
using System;
using System.Threading.Tasks;

namespace GrpcBidirectionalStreamingExample
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

            // Example of bidirectional streaming
            using var streamingCall = client.GetWeatherUpdates();

            // Background task to read responses from the server
            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var response in streamingCall.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine($"Received weather update for {response.City}: {response.Temperature}°C at {response.Timestamp}");
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Stream cancelled.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading response: " + ex);
                }
            });

            // Send requests through request stream
            foreach (var city in new[] { "New York", "London", "Tokyo" })
            {
                Console.WriteLine($"Requesting weather updates for {city}...");
                await streamingCall.RequestStream.WriteAsync(new WeatherRequest { City = city });
                await Task.Delay(1500); // Mimic delay in sending requests
            }

            Console.WriteLine("Completing request stream");
            await streamingCall.RequestStream.CompleteAsync();
            Console.WriteLine("Request stream completed");

            // Keep the console application running to continue receiving responses
            await Task.Delay(Timeout.Infinite);
        }
    }
}
