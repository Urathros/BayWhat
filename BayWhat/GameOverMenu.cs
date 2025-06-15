using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Animation;
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



        private Graphic _bgScreen;

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


		public uint Score
		{
			get => _score;
			set
			{
				_score = value;
				_scoreLabel.Text = $"Score: {_score}";
			}
		}


		public GameOverMenu(Core core, Input input, TextureLoader textureLoader, params UIComponent[] components) : base(core, components)
		{
			Name = nameof(GameOverMenu);

			Input = new UIInput(input, true);
			

			ButtonContainerPosition = core.DeviceSize;
			var nameText = new TextBox(_Core, new(200,20), 32);
			nameText.TextChanged += HandleTextUpdate;

			_scoreLabel = Game.GetPixelLabel(core, "Score:");

			_defaultFrame = textureLoader.Load($"BeachMorning\\NewLevelSequence.0000");

            _bgScreen = new Graphic(_Core, _defaultFrame);

            var scale = MathF.Min(_Core.DeviceSize.X / _defaultFrame.Size.X, _Core.DeviceSize.Y / _defaultFrame.Size.Y);
            _bgScreen.Scale = new(scale, scale);
            Add(_bgScreen);

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
							BackgroundColor = Color.Black,
							BackgroundAlpha = 0.5f,
							Init = new UIComponent[]
							{
								Game.GetPixelLabel(core, "They died...", 40),
								new Label(core, ""),
								new OffsetContainer(_Core, Orientation.Horizontal, 10)
								{
									Init = new UIComponent[]
									{
										nameText,
										new Button(core, null, Game.GetPixelLabel(core, "Save Score"))
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
								new Button(core, null, Game.GetPixelLabel(core, "To Menu"))
								{
									Name = "Button Menu",
									BackgroundColor = Color.Blue,
									InitReleased = HandleMenuSceneChange,
									InitFocusGained = HandleFocusGained,
									InitFocusLost = HandleFocusLost
								},

								new Button(core, null, Game.GetPixelLabel(core, "Exit"))
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
			HandleResize(_Core.DeviceSize);
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

		private void HandleScoreWrite(Button btn)
		{
			string root = "Data";
			string path = Path.Combine(root, "Score.json");
			try
			{
				//var data = JsonSerializer.Deserialize<ScoreData>(File.ReadAllText(path)) ?? new();
				//data.Add(new ScoreData { PlayerName = _playerName, Score = Score });
				File.WriteAllText(path, JsonSerializer.Serialize(new ScoreData { PlayerName = _playerName, Score = Score }));
			}
			catch (IOException ioex)
			{
				Log.Error(ioex);
			}
		}

		//TODO: Resize Bug, i'm tired
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
						innerComp.Position = ButtonContainerPosition - innerComp.InnerSize /2;
					}
				}
			}
		}
	}
}
