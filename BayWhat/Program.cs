using BlackCoat;
using SFML.Window;

namespace BayWhat
{
	internal static class Program
	{
		[STAThread]
		static void Main()
		{
#if !DEBUG
            var launcher = new Launcher()
            {
                //BannerImage = Image.FromFile("Assets\\banner.png"),
                Text = Game.TITLE
            };
            var device = Device.Create(launcher, Game.TITLE);
            if (device == null) return;
#endif

#if DEBUG
			var vm = new VideoMode(800, 600);
			var device = Device.Create(vm, Game.TITLE, Styles.Default, 0, false, 120);
#endif
			using var core = new Core(device);
#if DEBUG
			core.Debug = true;
#endif
			Game.Initialize(core, device);
			core.SceneManager.ChangeScene(new MenuScene(core));
			core.Run();
		}
	}
}