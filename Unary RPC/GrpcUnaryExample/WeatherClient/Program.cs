using Grpc.Core;
using Grpc.Net.Client;
using GrpcUnaryExample;
using System;

namespace GrpcUnaryExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // For development, trust all certificates (e.g., self-signed)
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var channel = GrpcChannel.ForAddress("https://localhost:5289", new GrpcChannelOptions
            {
                HttpHandler = handler
            });

            var client = new Weather.WeatherClient(channel);

            // Example of Unary RPC
            var response = client.GetWeather(new WeatherRequest { City = "New York" });

            Console.WriteLine($"Received weather update for {response.City}: {response.Temperature}°C at {response.Timestamp}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
