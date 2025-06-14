using BlackCoat;
using BlackCoat.Collision.Shapes;
using BlackCoat.Entities.Shapes;
using BlackCoat.InputMapping;
using SFML.Graphics;
using SFML.System;
using System.ComponentModel;

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
        private NPCManager _Npcs;
        private PauseMenu _Pause;
		private HUD _hud;
		private uint _deathCounter;

		public BeachScene(Core core) : base(core, nameof(BeachScene), "Assets")
		{
			_deathCounter = 0;
		}

		private void HandleDying()
		{
			_deathCounter++;
			if(_deathCounter >= Game.DeathLimit)
			{

			}
		}

		protected override bool Load()
		{
			// View
			_View = new View();
			Layer_Background.View = _View;
			Layer_Game.View = _View;
			_Core.DeviceResized += HandleDeviceResize;

			// Tile Map
			var tex = TextureLoader.Load("baywhat_tileset");
			var mapData = new MapData();
			mapData.Load(_Core, $"Assets\\Level1.tmx");
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
			_Player1.Act += OnP1Act;
			
			Layer_Game.Add(_Player1);
			HandleDeviceResize(_Core.DeviceSize);

            _hud = new(_Core, Input);
            Layer_Overlay.Add(_hud);

            // NPC
            Game.IsRunning = true;
			var partyArea = _Collisions.First(c => c.Type == CollisionType.PartyArea).Shape;
			var oceanArea = _Collisions.First(c => c.Type == CollisionType.Ocean).Shape;
			_Npcs = new NPCManager(_Core, new(partyArea.Position, partyArea.Size), 10f, oceanArea, TextureLoader, _hud);
			_Npcs.Dying += HandleDying;
			_Npcs.AddEntities(50);
			Layer_Game.Add(_Npcs);

			

			// PauseMenu
			_Pause = new PauseMenu(_Core, Input) { Visible = false };
			//Game.IsRunning = false; // when pause menu is opened
			Layer_Overlay.Add(_Pause);

			// Collision Helper
			/*
			Layer_Game.Add(new ColHelp(_Core, _Player1.CollisionShape));
			foreach (var item in _Npcs.GetAll<NPC>())
			{
				Layer_Game.Add(new ColHelp(_Core, item.CollisionShape));
			}
			*/
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

		private void OnP1Act(bool activate)
		{
			Log.Debug(activate);
			if(activate)
			{
				var npc = _Npcs.GetAll<NPC>().FirstOrDefault(npc => npc.CollisionShape.CollidesWith(_Player1.CollisionShape));
				if(npc != null)
				{
					npc.Position = default;
					npc.State = NPCState.Rescue;
					_Player1.Add(npc);
				}
			}
			else
			{
				var npc = _Player1.GetAll<NPC>().FirstOrDefault();
				if (npc != null)
				{
					if (_Player1.CollisionShape.CollidesWith(_Collisions.First(c => c.Type == CollisionType.Ocean).Shape))
					{ 
						if(npc.State == NPCState.Swiming || npc.State == NPCState.Drowning)
						{
                            npc.Position = default;
                            npc.State = NPCState.Rescue;
                            _Player1.Add(npc);
                        }
						
					}
					else
					{
						_Player1.Remove(npc);
						_hud.Score += 100;
						_Npcs.AddEntity(_Npcs.PosInPartyArea);
					}
				}
			}
		}
	}
}
