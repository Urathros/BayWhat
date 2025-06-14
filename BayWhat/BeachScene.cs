using BlackCoat;
using BlackCoat.Entities.Shapes;
using BlackCoat.InputMapping;
using SFML.Graphics;
using SFML.System;

namespace BayWhat
{
	internal class BeachScene : Scene
	{
		private Player _Player1 = default!; // Beachbunny
		private Player _Player2 = default!; // Shark
		private CollisionObject[] _Collisions = [];
		private View _View;
		private Vector2i _MapSize;
		private IntRect _ViewBounds;
        private NPCManager _npcs;
        private Rectangle _ocean;
        private PauseMenu _pause;

        public View VVV => _View;

		public BeachScene(Core core) : base(core, nameof(BeachScene), "Assets")
		{
		}

		protected override bool Load()
		{
			// View
			_View = new View();
			Layer_Background.View = _View;
			Layer_Game.View = _View;
			_Core.DeviceResized += HandleDeviceResize;

			// Tile Map
			var tex = TextureLoader.Load("tileset");
			var mapData = new MapData();
			mapData.Load(_Core, $"Assets\\test.tmx");
			_MapSize = mapData.TotalPixelSize;
			foreach (var layer in mapData.Layer)
			{
				var mapRenderer = new MapRenderer(_Core, mapData.MapSize, tex, mapData.TileSize);
				for (int i = 0; i < layer.Tiles.Length; i++)
				{
					mapRenderer.AddTile(i * 4, layer.Tiles[i].Position, layer.Tiles[i].Coordinates);
				}
				Layer_Background.Add(mapRenderer);
			}
			// Collision Layer
			_Collisions = mapData.Collisions;
			// Players
			_Player1 = new Player(_Core, Game.CreateInput(0), TextureLoader)
			{
				Position = _Collisions.Where(c => c.Type == CollisionType.P1Start).First().Shape.Position,
			};
			
			Layer_Game.Add(_Player1);
			HandleDeviceResize(_Core.DeviceSize);

            // NPC
            _ocean = new(_Core, _Core.DeviceSize, Color.Blue);
            _ocean.Position = new(0, _Core.DeviceSize.Y - 1);

			Game.IsRunning = true;
			_npcs = new (_Core, new(new(0f, 0f), _Core.DeviceSize), 10f, _ocean);
			_npcs.AddEntities(50);
			Layer_Game.Add(_npcs);
			Layer_Game.Add(_ocean);

			//// Pause
			_pause = new(_Core, Input);
			Game.IsRunning = false; // when pause menu is opened
			Layer_Game.Add(_pause);
			return true;
		}

		private void HandleDeviceResize(Vector2f deviceSize)
		{
			_View.Size = deviceSize;
			Vector2i half = deviceSize.ToVector2i() / 2;
			_ViewBounds = new IntRect(half, _MapSize - half);
		}

		protected override void Update(float deltaT)
		{
			_View.Center = new()
			{
				X = _Player1.Position.X < _ViewBounds.Left ? _ViewBounds.Left :
					_Player1.Position.X > _ViewBounds.Width ? _ViewBounds.Width : _Player1.Position.X,
				Y = _Player1.Position.Y < _ViewBounds.Top ? _ViewBounds.Top :
					_Player1.Position.Y > _ViewBounds.Height ? _ViewBounds.Height : _Player1.Position.Y
			};
		}

		protected override void Destroy()
		{
		}
	}
}
