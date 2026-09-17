using WebServerSimulation;

public static class Program {
    public static void Main()
    {
        Console.WriteLine("Server is running. Type 'exit-soft' to stop while processing remaining message. Type 'exit-hard' to stop immediately.");

        IWebServer server = new SimulatedWebServerV2(4);
        server.Run();

        while(true)
        {
            string? message = Console.ReadLine();

            if (string.IsNullOrEmpty(message))
            {
                continue;
            }

            (bool stop, bool? hard) = message.ToLower() switch
            {
                "exit-soft" => (true, false),
                "exit-hard" => (true, true),
                _ => (false, (bool?)null)
            };

            try
            {
                if (stop)
                {
                    server.Stop(hard!.Value);
                    break;
                }

                server.AcceptRequest(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}