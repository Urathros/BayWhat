using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Animation;
using BlackCoat.UI;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BayWhat
{
    internal class MenuScene : Scene
    {
        private const string MENU_SCENE_NAME = "Menu Scene";
        private const string CONTAINER_TITLE_NAME = "Container Title";
        private const string CONTAINER_BUTTONS_NAME = "Container Buttons";
        private const string CONTAINER_CREDITS_TEXT_NAME = "Container Credits Text";
        private const string CONTAINER_CREDITS_BUTTONS_NAME = "Container Credits Buttons";
        private const string CONTAINER_SCORE_NAME = "Container Score";

        private readonly FloatRect TEXT_PADDING = new(6, 4, 6, 4);
        private Canvas _uiRoot;
        private Canvas _uiCredits;
        private FrameAnimation _bgScreen;
        private Graphic _creditsBgGfx;
        private UIGraphic _title;
        private Button _StartBtn;
        private Button _CreditsBtn;
        private Button _ExitBtn;

        /// <summary>
        /// BG Screen Frame for Resizing
        /// </summary>
        private Texture _defaultFrame;
        private Texture _creditsTexture;

        private string _scoreText;

        private Vector2f _titleContainerPosition;
        private Vector2f _buttonContainerPosition;
        private Vector2f _creditsTextContainerPosition;
        private Vector2f _creditsButtonContainerPosition;
        private Vector2f _scoreTextContainerPosition;

        public Vector2f TitleContainerPosition
        {
            get => _titleContainerPosition; 
            set 
            {
                _titleContainerPosition.X = value.X / 2;
                _titleContainerPosition.Y = value.Y / 2 - 300;
            }
        }



        public Vector2f ButtonContainerPosition
        {
            get => _buttonContainerPosition;
            set
            {
                _buttonContainerPosition = value / 2;
            }
        }

        public Vector2f CreditsTextContainerPosition
        {
            get => _creditsTextContainerPosition; 
            set 
            {
                _creditsTextContainerPosition.X = value.X - 300;
                _creditsTextContainerPosition.Y = value.Y / 4f; 
            }
        }

        public Vector2f CreditsButtonContainerPosition
        {
            get => _creditsButtonContainerPosition;
            set
            {
                _creditsButtonContainerPosition.X = 0;
                _creditsButtonContainerPosition.Y = value.Y - 200;
            }
        }


        public Vector2f ScoreTextContainerPosition
        {
            get => _scoreTextContainerPosition; 
            set { _scoreTextContainerPosition = new(20, value.Y - 50); }
        }




        public MenuScene(Core core) : base(core, MENU_SCENE_NAME, "Assets")
        {
            Name = nameof(MenuScene);
        }

        private void HandleFocusGained(UIComponent comp)
        {
            comp.BackgroundColor = SFML.Graphics.Color.Red;
        }

        private void HandleFocusLost(UIComponent comp)
        {
            comp.BackgroundColor = SFML.Graphics.Color.Blue;
        }

        private void HandleResize(Vector2f size)
        {
            _uiRoot.Resize(size);
            _uiCredits.Resize(size);


            TitleContainerPosition = size;
            ButtonContainerPosition = size;
            CreditsButtonContainerPosition = size;
            ScoreTextContainerPosition = size;


            //var aspectRatio = CalcAspectRatio(size);

            var scale = MathF.Min(size.X / _defaultFrame.Size.X, size.Y / _defaultFrame.Size.Y);
            _bgScreen.Scale = new(scale, scale);

            var creditsScale = MathF.Min(size.X / _creditsTexture.Size.X, size.Y / _creditsTexture.Size.Y);
            _creditsBgGfx.Scale = new(creditsScale, creditsScale);

            var titleScale = MathF.Min(size.X / _title.Texture.Size.X, size.Y / _title.Texture.Size.Y) *.6f;
            _title.Scale = new(titleScale , titleScale); 

            foreach (var comp in _uiRoot.GetAll<UIComponent>())
            {
                if (comp.Name == CONTAINER_TITLE_NAME) comp.Position = new(TitleContainerPosition.X, comp.Position.Y);
                if (comp.Name == CONTAINER_BUTTONS_NAME) comp.Position = ButtonContainerPosition;
                if (comp.Name == CONTAINER_SCORE_NAME) comp.Position = ScoreTextContainerPosition;
            }

            foreach (var comp in _uiCredits.GetAll<UIComponent>())
            {
                if (comp.Name == CONTAINER_CREDITS_TEXT_NAME) comp.Position = CreditsTextContainerPosition;
                if (comp.Name == CONTAINER_CREDITS_BUTTONS_NAME) comp.Position = CreditsButtonContainerPosition;
            }
        }

        void ReadScoreText()
        {
            string root = "Data";
            string path = Path.Combine(root, "Score.json");
            if (!File.Exists(path)) return;

            try
            {
                var json = File.ReadAllText(path);
                var data = JsonSerializer.Deserialize<ScoreData>(json);
                _scoreText = $"{data!.PlayerName} Score: {data.Score}";
            }
            catch (IOException ioEx)
            {
                Log.Error(ioEx);
            }
        }

        private float CalcAspectRatio(Vector2f scale)
        {
            if (scale.Y == 0f) throw new DivideByZeroException("Error: Divisor haven't be zero!");

            return scale.X / scale.Y;
        }

        protected override bool Load()
        {
            TitleContainerPosition = _Core.DeviceSize;
            ButtonContainerPosition = _Core.DeviceSize;
            CreditsTextContainerPosition = _Core.DeviceSize;
            CreditsButtonContainerPosition = _Core.DeviceSize;
            ScoreTextContainerPosition = _Core.DeviceSize;

           
            var frames = Enumerable.Range(1, 119).Select(i => TextureLoader.Load($"BeachNight\\NewLevelSequence.{i:0000}")).ToArray();
            _defaultFrame = frames[0];
            _bgScreen = new FrameAnimation(_Core, .075f, frames);

            var scale = MathF.Min(_Core.DeviceSize.X / _defaultFrame.Size.X, _Core.DeviceSize.Y / _defaultFrame.Size.Y);
            _bgScreen.Scale = new(scale, scale);

            // Credits
            _creditsTexture = TextureLoader.Load("BeachNoon");
            _creditsBgGfx = new Graphic(_Core, _creditsTexture) { Scale = new(scale, scale) };


			var uiInput = new UIInput(Input, true);

            ReadScoreText();

            _uiRoot = new(_Core, _Core.DeviceSize)
            {
                Input = uiInput,

                Init = new UIComponent[]
               {
                   new OffsetContainer(_Core, Orientation.Vertical, 10)
                   {
                       Name = CONTAINER_TITLE_NAME,
                       Position = new(TitleContainerPosition.X, 0),
                       Init = new UIComponent[]
                       {
                           _title = new UIGraphic(_Core, TextureLoader.Load($"baywhat_icon.png"))
                       }
                   },
                   new OffsetContainer(_Core, Orientation.Vertical, 10)
                   {
                       Name = CONTAINER_BUTTONS_NAME,
                       Position = ButtonContainerPosition,
                       Init = new[]
                       {
                           _StartBtn = new Button(_Core, null, Game.GetPixelLabel(_Core, "Start"))
                           {
                               Name = "Button Start",
                               BackgroundColor = SFML.Graphics.Color.Blue,
                               InitReleased = b => _Core.SceneManager.ChangeScene(new BeachScene(_Core)),
                               InitFocusGained = HandleFocusGained,
                               InitFocusLost = HandleFocusLost
                           },

                           _CreditsBtn = new Button(_Core, null, Game.GetPixelLabel(_Core, "Credits"))
                           {
                               Name = "Button Credits",
                               BackgroundColor = SFML.Graphics.Color.Blue,
                               InitReleased = b => 
                               {
                                   Layer_Game.Add(_uiCredits);
                                   Layer_Background.Add(_creditsBgGfx);
                                   Layer_Overlay.Remove(_uiRoot);
                                   Layer_Background.Remove(_bgScreen);
                               },
                               InitFocusGained = HandleFocusGained,
                               InitFocusLost = HandleFocusLost
                           },

                           _ExitBtn = new Button(_Core, null, Game.GetPixelLabel(_Core, "Exit"))
                           {
                               Name = "Button Exit",
                               BackgroundColor = SFML.Graphics.Color.Blue,
                               InitReleased = b => _Core.Exit(),
                               InitFocusGained = HandleFocusGained,
                               InitFocusLost = HandleFocusLost
                           }
                       }
                   },

                   new OffsetContainer(_Core, Orientation.Vertical, 10)
                   {
                       Name = CONTAINER_SCORE_NAME,
                       Position = ScoreTextContainerPosition,
                       Init = new[]
                       {
                            new Label(_Core, _scoreText)
                            {
                                BackgroundAlpha = 0.5f,
                                BackgroundColor = SFML.Graphics.Color.Blue
                            }
                       }
                   }
               }
            };


            var titleScale = MathF.Max(800 / _title.Texture.Size.X, 600 / _title.Texture.Size.Y) ;
            _title.Scale = new(titleScale, titleScale);



            _uiCredits = new(_Core, _Core.DeviceSize)
            {
                Input = uiInput,
                Init = new[]
                {
                    new OffsetContainer(_Core, Orientation.Vertical, 10)
                    {
                        Name = CONTAINER_CREDITS_TEXT_NAME,
                        Position = CreditsTextContainerPosition,
                        BackgroundColor = SFML.Graphics.Color.Black,
                        BackgroundAlpha = 0.6f,
						Init =
						[
						   Game.GetPixelLabel(_Core, "Credits"),
                           Game.GetPixelLabel(_Core),
                           Game.GetPixelLabel(_Core, "Alexander Schwahl"),
                           Game.GetPixelLabel(_Core, "Monika Zagorac"),
                           Game.GetPixelLabel(_Core, "Jochen Köhler"),
                           Game.GetPixelLabel(_Core, "Louis Friedl"),
                           Game.GetPixelLabel(_Core, "Marcus Schaal")
                        ]
                    },

                    new OffsetContainer(_Core, Orientation.Vertical, 10)
                    {
                        Name = CONTAINER_CREDITS_BUTTONS_NAME,
                        Position = CreditsButtonContainerPosition,
                       Init = new[]
                       {
                           new Button(_Core, null, Game.GetPixelLabel(_Core, "Return"))
                           {
                               Name = "Button Return",
                               BackgroundColor = SFML.Graphics.Color.Blue,
                               InitReleased = b =>
                               {
                                   Layer_Game.Remove(_uiCredits);
                                   Layer_Background.Remove(_creditsBgGfx);
                                   Layer_Overlay.Add(_uiRoot);
                                   Layer_Background.Add(_bgScreen);
                               },
                               InitFocusGained = HandleFocusGained,
                               InitFocusLost = HandleFocusLost
                           }
                       }
                    }
                }
            };

            Layer_Overlay.Add(_uiRoot);
            Layer_Background.Add(_bgScreen);
            _Core.DeviceResized += HandleResize;
            

            return true;
        }

        protected override void Update(float deltaT)
        {
        }
        protected override void Destroy()
        {
            _Core.DeviceResized -= HandleResize;
        }

    }
}
