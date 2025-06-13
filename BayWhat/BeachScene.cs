using BlackCoat;
using BlackCoat.Entities.Shapes;
using BlackCoat.InputMapping;
using SFML.Graphics;

namespace BayWhat
{
	internal class BeachScene : Scene
	{
		private SimpleInputMap<GameAction> _InputMapPlayer1;
		private SimpleInputMap<GameAction> _InputMapPlayer2;
		private Player _Player1; // Beachbunny
		private Player _Player2; // Shark
		private CollisionObject[] _Collisions = [];

		public BeachScene(Core core) : base(core, nameof(BeachScene), "Assets")
		{
		}

		protected override bool Load()
		{
			// View
			Layer_Background.View = Game.View;
			Layer_Game.View = Game.View;

			// Tile Map
			var tex = TextureLoader.Load("tileset");
			var mapData = new MapData();
			mapData.Load(_Core, $"Assets\\test.tmx");
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
				Position = _Collisions.Where(c => c.Type == CollisionType.Start).First().Shape.Position,
			};
			Layer_Game.Add(_Player1);
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
