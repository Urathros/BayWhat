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
    internal class GameOverMenu : UIContainer
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

        public GameOverMenu(Core core, Input input, params UIComponent[] components) : base(core, components)
        {
            Name = nameof(GameOverMenu);

            Input = new UIInput(input, true);
            BackgroundColor = Color.Red;

            ButtonContainerPosition = core.DeviceSize;
            var nameText = new TextBox(_Core);
            //nameText.TextChanged

            Init = new[]
            {

                new Canvas(core, core.DeviceSize)
                {
                    Init = new UIComponent[]
                    {
                        new OffsetContainer(_Core, Orientation.Vertical, 10)
                        {

                            Name = CONTAINER_BUTTONS_NAME,
                            Position = ButtonContainerPosition,
                            Init = new UIComponent[]
                            {
                                new Label(_Core, "They died..."),
                                new Label(_Core, ""),
                                nameText,
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



        private void HandleMenuSceneChange(Button btn)
        {
            Game.IsRunning = false;
            _Core.SceneManager.ChangeScene(new MenuScene(_Core));
        }
    }
}
