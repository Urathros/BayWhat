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
            var device = Device.Demo;
            using var core = new Core(device);
            core.Debug = true;

            Game.Initialize(core, device);

            var scene = new BeachScene(core);
            core.SceneManager.ChangeScene(scene);
            core.Run();
        }
    }
}