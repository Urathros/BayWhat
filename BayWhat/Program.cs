using BlackCoat;

namespace BayWhat
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static void Main()
        {
            using var core = new Core(Device.Demo);
            core.Run();
        }
    }
}