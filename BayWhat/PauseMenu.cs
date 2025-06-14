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
    internal class PauseMenu : UIContainer
    {
        private const string CONTAINER_BUTTONS_NAME = "Container Buttons";
        private readonly FloatRect TEXT_PADDING = new(6, 4, 6, 4);


        private Vector2f _buttonContainerPosition;


        public Vector2f ButtonContainerPosition
        {
            get => _buttonContainerPosition;
            set
            {
                _buttonContainerPosition = value / 2;
            }
        }

        public PauseMenu(Core core/*, params UIComponent[] components*/, Input input) : base(core, null)
        {
            Name = "Pause Menu";
            Input = new UIInput(input, true);
            BackgroundColor = Color.Red;

            ButtonContainerPosition = core.DeviceSize;

            Init = new[]
            {
                new Canvas(core, core.DeviceSize)
                {
                    Init = new[]
                    {
                        new OffsetContainer(_Core, Orientation.Vertical, 10)
                        {
                            Name = CONTAINER_BUTTONS_NAME,
                            Position = ButtonContainerPosition,
                            Init = new UIComponent[]
                            {
                                new Label(_Core, "Pause"),
                                new Label(_Core, ""),
                                new Button(_Core, null, new Label(_Core, "Continue") {Padding = TEXT_PADDING})
                                {
                                    Name = "Button Continue",
                                    BackgroundColor = Color.Blue,
                                    InitReleased = HandleContinue,
                                    InitFocusGained = HandleFocusGained,
                                    InitFocusLost = HandleFocusLost
                                },
                                new Button(_Core, null, new Label(_Core, "To Menu") {Padding = TEXT_PADDING})
                                {
                                    Name = "Button Menu",
                                    BackgroundColor = Color.Blue,
                                    InitReleased = HandleMenuSceneChange,
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
                }
            };
        }


        private void HandleFocusGained(UIComponent comp)
        {
            comp.BackgroundColor = Color.Red;
        }

        private void HandleFocusLost(UIComponent comp)
        {
            comp.BackgroundColor = Color.Blue;
        }

        private void HandleContinue(Button btn)
        {
            Visible = false;
            Game.IsRunning = true;
        }

        private void HandleMenuSceneChange(Button btn)
        {
            _Core.SceneManager.ChangeScene(new MenuScene(_Core));
            Game.IsRunning = true;
        }

    }
}
