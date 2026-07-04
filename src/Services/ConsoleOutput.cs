namespace RadarSantista.src.Services
{
    public class ConsoleOutput : IConsoleOutput
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
