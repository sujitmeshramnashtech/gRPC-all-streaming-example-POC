using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServerStreamingExample;

class Program
{
    static async Task Main(string[] args)
    {
        // The address of the gRPC server
        var serverAddress = "https://localhost:7045";

        // Create a gRPC channel to communicate with the server
        using var channel = GrpcChannel.ForAddress(serverAddress);

        // Create a client for the Weather service
        var client = new Weather.WeatherClient(channel);

        // Create a request for weather updates for a specific city
        var request = new WeatherRequest { City = "New York" };

        Console.WriteLine($"Requesting weather updates for {request.City}...");

        // Call the GetWeatherUpdates method on the client
        using var streamingCall = client.GetWeatherUpdates(request);

        try
        {
            // Read and process each response from the server stream
            await foreach (var update in streamingCall.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Weather Update: {update.Description}, " +
                                  $"Temperature: {update.Temperature}, " +
                                  $"Timestamp: {update.Timestamp}");
            }
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
        {
            Console.WriteLine("Weather updates cancelled.");
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Status}");
        }
        Console.ReadLine();
    }
}
