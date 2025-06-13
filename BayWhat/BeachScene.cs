using BlackCoat;
using SquareRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayWhat
{
	internal class BeachScene : Scene
	{
		private CollisionObject[] _Collisions = [];

		public BeachScene(Core core) : base(core, nameof(BeachScene), "Assets")
		{
		}

		protected override bool Load()
		{
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
