using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Animation;
using BlackCoat.UI;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// BG Screen Frame for Resizing
        /// </summary>
        private Texture _defaultFrame;
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
                _titleContainerPosition.Y = value.Y / 2 - 50;
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
                _creditsTextContainerPosition.X = 32;
                _creditsTextContainerPosition.Y = value.Y / 4f; 
            }
        }

        public Vector2f CreditsButtonContainerPosition
        {
            get => _creditsButtonContainerPosition;
            set
            {
                _creditsButtonContainerPosition.X = 0;
                _creditsButtonContainerPosition.Y = value.Y - 20;
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
            comp.BackgroundColor = Color.Red;
        }

        private void HandleFocusLost(UIComponent comp)
        {
            comp.BackgroundColor = Color.Blue;
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

            foreach (var comp in _uiRoot.GetAll<UIComponent>())
            {
                if (comp.Name == CONTAINER_TITLE_NAME) comp.Position = TitleContainerPosition;
                if (comp.Name == CONTAINER_BUTTONS_NAME) comp.Position = ButtonContainerPosition;
                if (comp.Name == CONTAINER_SCORE_NAME) comp.Position = ScoreTextContainerPosition;
            }

            foreach (var comp in _uiCredits.GetAll<UIComponent>())
            {
                if (comp.Name == CONTAINER_CREDITS_TEXT_NAME) comp.Position = CreditsTextContainerPosition;
                if (comp.Name == CONTAINER_CREDITS_BUTTONS_NAME) comp.Position = CreditsButtonContainerPosition;
            }
        }

        //TODO: Alex bitte noch mal drüber schauen, dass das sauber ist!
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
            catch (Exception ex)
            {

                throw;
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

            string count;
            var frames = new Texture[120];
            for (int i = 0; i < frames.Count(); i++)
            {

                frames[i] = TextureLoader.Load($"BeachNight\\NewLevelSequence.{i:0000}");
            }
            _defaultFrame = frames[0];

            _bgScreen = new FrameAnimation(_Core, .075f, frames);
            //var aspectRatio = CalcAspectRatio(_Core.DeviceSize);
            var scale = MathF.Min(_Core.DeviceSize.X / _defaultFrame.Size.X, _Core.DeviceSize.Y / _defaultFrame.Size.Y);
            _bgScreen.Scale = new(scale, scale);

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
                       Position = TitleContainerPosition,
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
                       Name = CONTAINER_BUTTONS_NAME,
                       Position = ButtonContainerPosition,
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
                                BackgroundColor = Color.Blue
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
                        Name = CONTAINER_CREDITS_TEXT_NAME,
                        Position = CreditsTextContainerPosition,
                        Init = new[]
                        {

                           new Label(_Core, "Credits"),
                           new Label(_Core, ""),
                           new Label(_Core, "Alexander Schwahl"),
                           new Label(_Core, "Monika Zagorac"),
                           new Label(_Core, "Jochen Köhler"),
                           new Label(_Core, "Louis Friedl"),
                           new Label(_Core, "Marcus Schaal")
                        }
                    },

                    new OffsetContainer(_Core, Orientation.Vertical, 10)
                    {
                        Name = CONTAINER_CREDITS_BUTTONS_NAME,
                        Position = CreditsButtonContainerPosition,
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
