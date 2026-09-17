namespace WebServerSimulation
{
    public interface IWebServer
    {
        void Run();
        void Stop(bool hard);
        void AcceptRequest(string message);
    }
}
