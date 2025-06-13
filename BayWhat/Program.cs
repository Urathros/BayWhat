using BlackCoat;

namespace BayWhat
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using var core = new Core(Device.Demo);
            core.Debug = true;

            var scene = new BeachScene(core);
            core.SceneManager.ChangeScene(scene);
            core.Run();
        }
    }
}