using BlackCoat;
using SFML.Window;

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
            var vm = new VideoMode(800, 600);
            var device = Device.Create(vm, "BayWhat?!?", Styles.Default, 0, false, 120);
            using var core = new Core(device);
            core.Debug = true;

            var scene = new TestScene(core);
            var menu = new MenuScene(core);
            core.SceneManager.ChangeScene(menu);
            core.Run();
        }
    }
}