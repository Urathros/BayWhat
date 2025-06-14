using BlackCoat;
using BlackCoat.UI;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BayWhat
{
    internal class GameOverMenu : UIContainer
    {
        private const string CONTAINER_BUTTONS_NAME = "Container Buttons";
        private readonly FloatRect TEXT_PADDING = new(6, 4, 6, 4);

        private Label _scoreLabel;

        private string _playerName;

        private uint _score;

        private Vector2f _buttonContainerPosition;


        public Vector2f ButtonContainerPosition
        {
            get => _buttonContainerPosition;
            set
            {
                _buttonContainerPosition = value / 2;
            }
        }


        public uint Score
        {
            get => _score; 
            set 
            { 
                _score = value;
                _scoreLabel.Text = $"Score: {_score}";
            }
        }


        public GameOverMenu(Core core, Input input, params UIComponent[] components) : base(core, components)
        {
            Name = nameof(GameOverMenu);

            Input = new UIInput(input, true);
            BackgroundColor = Color.Red;

            ButtonContainerPosition = core.DeviceSize;
            var nameText = new TextBox(_Core);
            nameText.TextChanged += HandleTextUpdate;

            _scoreLabel = new(core, "Score:");

            Init = new UIComponent[]
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
                                new Label(core, "They died..."),
                                new Label(core, ""),
                                new OffsetContainer(_Core, Orientation.Horizontal, 10)
                                {
                                    Init = new UIComponent[]
                                    {
                                        nameText,
                                        new Button(core, null, new Label(core, "Save Score") {Padding = TEXT_PADDING})
                                        {
                                            Name = "Button Save",
                                            BackgroundColor = Color.Blue,
                                            InitReleased = HandleScoreWrite,
                                            InitFocusGained = HandleFocusGained,
                                            InitFocusLost = HandleFocusLost
                                        },

                                    }
                                },
                                _scoreLabel,
                                new Button(core, null, new Label(core, "To Menu") {Padding = TEXT_PADDING})
                                {
                                    Name = "Button Menu",
                                    BackgroundColor = Color.Blue,
                                    InitReleased = HandleMenuSceneChange,
                                    InitFocusGained = HandleFocusGained,
                                    InitFocusLost = HandleFocusLost
                                },

                                new Button(core, null, new Label(core, "Exit") {Padding = TEXT_PADDING})
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

            _Core.DeviceResized += HandleResize;
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

        private void HandleTextUpdate(Label text)
        {
            _playerName = text.Text;

        }

        //TODO: Alex bitte noch mal drüber schauen, dass das sauber ist!
        private void HandleScoreWrite(Button btn)
        {
            string root = "Data";
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            try
            {
                using (var stream = new FileStream($"Data\\Score.json", FileMode.Create))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var data = new ScoreData { PlayerName = _playerName, Score = Score };
                        var json = JsonSerializer.Serialize(data);
                        writer.Write(json);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        private void HandleResize(Vector2f size)
        {
            if (Disposed)
            {
                _Core.DeviceResized -= HandleResize;
                return;
            }

            ButtonContainerPosition = size;

            foreach (var comp in GetAll<UIComponent>())
            {
                foreach (var innerComp in comp.GetAll<UIComponent>())
                {
                    if (innerComp.Name == CONTAINER_BUTTONS_NAME) comp.Position = ButtonContainerPosition;
                }
                
            }

        }
    }
}
