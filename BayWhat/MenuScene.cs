using BlackCoat;
using BlackCoat.UI;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayWhat
{
    internal class MenuScene : Scene
    {
        private readonly FloatRect TEXT_PADDING = new(6, 4, 6, 4);
        private Canvas _uiRoot;
        private Canvas _uiCredits;

        public MenuScene(Core core) : base(core, "Menu")
        {
        }

        private void HandleFocusGained(UIComponent comp)
        {
            comp.BackgroundColor = Color.Red;
        }

        private void HandleFocusLost(UIComponent comp)
        {
            comp.BackgroundColor = Color.Blue;
        }

        protected override bool Load()
        {
            var uiInput = new UIInput(Input, true);
            _uiRoot = new(_Core, _Core.DeviceSize)
            {
                Input = uiInput,
                BackgroundColor = Color.Cyan,
                Init = new []
               {
                   new OffsetContainer(_Core, Orientation.Vertical, 10)
                   {
                       Position = new Vector2f()
                       {
                           X = _Core.DeviceSize.X / 2,
                           Y = _Core.DeviceSize.Y / 2 - 50
                       },
                       Init = new[]
                       {
                           new Label(_Core, "BayWhat?!?")
                           //{
                           //    Padding = new FloatRect(new (0f, 0f), new (100, 30)),
                           //    Scale = 
                           //}
                       }
                   },
                   new OffsetContainer(_Core, Orientation.Vertical, 10)
                   {
                       Position = _Core.DeviceSize / 2,
                       Init = new[]
                       {
                           new Button(_Core, null, new Label(_Core, "Start") {Padding = TEXT_PADDING})
                           {
                               Name = "Button Start",
                               BackgroundColor = Color.Blue,
                               InitReleased = b => _Core.SceneManager.ChangeScene(new BeachScene(_Core)),
                               InitFocusGained = HandleFocusGained,
                               InitFocusLost = HandleFocusLost
                           },

                           new Button(_Core, null, new Label(_Core, "Credits") {Padding = TEXT_PADDING})
                           {
                               Name = "Button Credits",
                               BackgroundColor = Color.Blue,
                               InitReleased = b => Layer_Game.Add(_uiCredits),
                               InitFocusGained = HandleFocusGained,
                               InitFocusLost = HandleFocusLost
                           },

                           new Button(_Core, null, new Label(_Core, "Exit") {Padding = TEXT_PADDING})
                           {
                               Name = "Button Exit",
                               BackgroundColor = Color.Blue,
                               InitReleased = b => _Core.Exit(),
                               InitFocusGained = HandleFocusGained,
                               InitFocusLost = HandleFocusLost
                           }
                       }
                   }

               }
            };

            _uiCredits = new(_Core, _Core.DeviceSize)
            {
                Input = uiInput,
                BackgroundColor = Color.Magenta,
                Init = new[]
                {
                    new OffsetContainer(_Core, Orientation.Vertical, 10)
                    {
                        Position = new Vector2f()
                       {
                           X = 0,
                           Y = _Core.DeviceSize.Y - 20
                       },
                       Init = new[]
                       {
                           new Button(_Core, null, new Label(_Core, "Return") {Padding = TEXT_PADDING})
                           {
                               Name = "Button Return",
                               BackgroundColor = Color.Blue,
                               InitReleased = b => Layer_Game.Remove(_uiCredits),
                               InitFocusGained = HandleFocusGained,
                               InitFocusLost = HandleFocusLost
                           }
                       }
                    }
                }
            };

            Layer_Game.Add(_uiRoot);
            _Core.DeviceResized += _uiRoot.Resize;

            return true;
        }

        protected override void Update(float deltaT)
        {
        }
        protected override void Destroy()
        {
        }

    }
}
