using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;
using BlackCoat;
using BlackCoat.InputMapping;
using BlackCoat.AssetHandling;
using BlackCoat.Entities;

namespace BayWhat
{
    static class Game
    {
        private static readonly Vector2f _ASPECT_RATIO = new Vector2f(9f / 16f, 16f / 9f);
        public const string TITLE = "BayWhat?!?";

        private static Core _Core;
        private static Device _Device;
        private static bool _isRunning;
        private static FontLoader _font = new("Assets");

        public static Vector2f FullHD;

        public static View View { get; private set; }
        public static Input Input { get; private set; }

        public static bool IsRunning 
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                if (_isRunning) Unpaused.Invoke();
                else Paused.Invoke();
            }
        }

        public static event Action Paused = () => { };
        public static event Action Unpaused = () => { };

        internal static void Initialize(Core core, Device device)
        {
            _Core = core;
            _Device = device;
            Input = new Input(_Core, true, true, true);

            // View
            FullHD = new Vector2f(1920, 1080);
            View = new View(FullHD / 2, FullHD);
            _Core.DeviceResized += HandleDeviceResized;
            HandleDeviceResized(_Core.DeviceSize);
        }

        public static SimpleInputMap<GameAction> CreateInput(uint player = 0)
        {
            var map = new SimpleInputMap<GameAction>(Input, player);
            if (player == 0)
            {
                map.AddKeyboardMapping(Keyboard.Key.W, GameAction.Up);
                map.AddKeyboardMapping(Keyboard.Key.A, GameAction.Left);
                map.AddKeyboardMapping(Keyboard.Key.S, GameAction.Down);
				map.AddKeyboardMapping(Keyboard.Key.D, GameAction.Right);
                map.AddKeyboardMapping(Keyboard.Key.F, GameAction.Act);
            }
            else
            {
                map.AddKeyboardMapping(Keyboard.Key.Up, GameAction.Up);
                map.AddKeyboardMapping(Keyboard.Key.Left, GameAction.Left);
                map.AddKeyboardMapping(Keyboard.Key.Down, GameAction.Down);
				map.AddKeyboardMapping(Keyboard.Key.Right, GameAction.Right);
                map.AddKeyboardMapping(Keyboard.Key.RControl, GameAction.Act);
            }
            map.AddJoystickButtonMapping(0, GameAction.Act);
            map.AddJoystickButtonMapping(1, GameAction.Act);
            map.AddJoystickButtonMapping(2, GameAction.Act);
			map.AddJoystickMovementMapping(Joystick.Axis.PovX, 10f, GameAction.Right);
			map.AddJoystickMovementMapping(Joystick.Axis.PovX, -10f, GameAction.Left);
			map.AddJoystickMovementMapping(Joystick.Axis.PovY, 10f, GameAction.Up);
			map.AddJoystickMovementMapping(Joystick.Axis.PovY, -10f, GameAction.Down);
			map.AddJoystickMovementMapping(Joystick.Axis.X, 10f, GameAction.Right);
			map.AddJoystickMovementMapping(Joystick.Axis.X, -10f, GameAction.Left);
			map.AddJoystickMovementMapping(Joystick.Axis.Y, 10f, GameAction.Up);
			map.AddJoystickMovementMapping(Joystick.Axis.Y, -10f, GameAction.Down);
			return map;
        }


        public static Vector2f MapToPixel(Vector2f pos, View view) => _Device.MapCoordsToPixel(pos, view).ToVector2f();
        public static Vector2f MapToCoords(Vector2f pos, View view) => _Device.MapPixelToCoords(pos.ToVector2i(), view);
        public static void HandleDeviceResized(Vector2f size)
        {
            var corrected = size.MultiplyBy(_ASPECT_RATIO);
            var port = new FloatRect(0, 0, 1, 1);
            if (size.X > corrected.Y)
            {
                Log.Debug(size.X, corrected.X);
                port.Width = corrected.Y / size.X;
                port.Left = (1 - port.Width) / 2;
            }
            else if (size.Y > corrected.X)
            {
                Log.Debug(size.Y, corrected.Y);
                port.Height = corrected.X / size.Y;
                port.Top = (1 - port.Height) / 2;
            }
            View.Viewport = port;
        }


		public static IEnumerable<IntRect> CalcFrames(int width, int FramesInAnimation, int height = 0, int start = 0, int row = 0)
		{
			height = height == 0 ? width : height;
			for (int i = start; i < FramesInAnimation + start; i++)
			{
				yield return new IntRect(i * width, row * height, width, height);
			}
		}

		public static IEnumerable<IntRect> CalcFrames(Texture tex, Vector2u frame, uint start = 0, uint end = 0)
		{
            uint cols = tex.Size.X / frame.X;
            uint rows = tex.Size.Y / frame.Y;
            if (end == 0) end = cols * rows;
			for (uint i = start; i < end; i++)
			{
                uint y = i / cols;
                uint x = i - y * cols;
				yield return new IntRect((int)(x * frame.X), (int)(y * frame.Y), (int)frame.X, (int)frame.Y);
			}
		}

        public static TextItem GetPixelText(Core core, String text = "", uint characterSize = 16)
        {
            var font = _font.Load("PixelifySans-Regular");
            return new TextItem(_Core, "Pugs", 32, font);
        }
	}
}