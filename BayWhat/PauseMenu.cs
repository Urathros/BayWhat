﻿using BlackCoat;
using BlackCoat.Entities.Animation;
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


        private FrameAnimation _bgScreen;

        /// <summary>
        /// BG Screen Frame for Resizing
        /// </summary>
        private Texture _defaultFrame;




        public Vector2f ButtonContainerPosition
        {
            get => _buttonContainerPosition;
            set
            {
                _buttonContainerPosition = value / 2;
            }
        }

        public PauseMenu(Core core/*, params UIComponent[] components*/, Input input, TextureLoader textureLoader) : base(core, null)
        {
            Name = "Pause Menu";
            Input = new UIInput(input, true);
            //BackgroundColor = Color.Red;

            ButtonContainerPosition = core.DeviceSize;

            string count;
            var frames = new Texture[120];
            for (int i = 0; i < frames.Count(); i++)
            {
                frames[i] = textureLoader.Load($"BeachAfternoon\\NewLevelSequence.{i:0000}");
            }
            _defaultFrame = frames[0];

            _bgScreen = new FrameAnimation(_Core, .075f, frames);

            var scale = MathF.Min(_Core.DeviceSize.X / _defaultFrame.Size.X, _Core.DeviceSize.Y / _defaultFrame.Size.Y);
            _bgScreen.Scale = new(scale, scale);
            Add(_bgScreen);
            
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
                                Game.GetPixelLabel(_Core, "Pause", 40),
                                new Label(_Core, ""),
                                new Button(_Core, null, Game.GetPixelLabel(_Core, "Continue"))
                                {
                                    Name = "Button Continue",
                                    InitReleased = HandleContinue,
                                    InitFocusGained = HandleFocusGained,
                                    InitFocusLost = HandleFocusLost
                                },
                                new Button(_Core, null, Game.GetPixelLabel(_Core, "To Menu"))
                                {
                                    Name = "Button Menu",
                                    InitReleased = HandleMenuSceneChange,
                                    InitFocusGained = HandleFocusGained,
                                    InitFocusLost = HandleFocusLost
                                },
                                
                                new Button(_Core, null, Game.GetPixelLabel(_Core, "Exit"))
                                {
                                    Name = "Button Exit",
                                    InitReleased = b => _Core.Exit(),
                                    InitFocusGained = HandleFocusGained,
                                    InitFocusLost = HandleFocusLost
                                }
                            }
                        }
                    }
                }
            };

            _Core.DeviceResized += HandleResize;
        }


        private void HandleFocusGained(UIComponent comp)
        {
            comp.BackgroundColor = new Color(0,200,0,15);
        }

        private void HandleFocusLost(UIComponent comp)
        {
            comp.BackgroundColor = Color.Transparent;
        }

        private void HandleContinue(Button btn)
        {
            Visible = false;
            Game.IsRunning = true;
        }

        private void HandleMenuSceneChange(Button btn)
        {
            Game.IsRunning = false;
            _Core.SceneManager.ChangeScene(new MenuScene(_Core));
        }


        private void HandleResize(Vector2f size)
        {
            if (Disposed)
            {
                _Core.DeviceResized -= HandleResize;
                return;
            }

            ButtonContainerPosition = size;


            var scale = MathF.Min(size.X / _defaultFrame.Size.X, size.Y / _defaultFrame.Size.Y);
            _bgScreen.Scale = new(scale, scale);


            foreach (var comp in GetAll<UIComponent>())
            {
                foreach (var innerComp in comp.GetAll<UIComponent>())
                {
                    if (innerComp.Name == CONTAINER_BUTTONS_NAME)
                    {
                        (comp as Canvas)!.Resize(size);
                        innerComp.Position = ButtonContainerPosition - innerComp.InnerSize / 2;
                    }
                }

            }

        }
    }
}
