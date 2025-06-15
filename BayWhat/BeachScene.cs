using BlackCoat;
using BlackCoat.Collision.Shapes;
using BlackCoat.Entities.Shapes;
using BlackCoat.InputMapping;
using SFML.Graphics;
using SFML.System;
using System.ComponentModel;
using System.Drawing;
using static System.Formats.Asn1.AsnWriter;

namespace BayWhat
{
	internal class BeachScene : Scene
	{
		private Player _Player = default!;
		private CollisionObject[] _Collisions = [];
		private View _View;
		private Vector2i _MapSize;
		private IntRect _ViewBounds;
		private RectangleCollisionShape _OceanArea;
		private NPCManager _Npcs;
		private PauseMenu _Pause;
		private GameOverMenu _gameOver;
		private HUD _hud;
		private uint _deathCounter;
		private Vector2f _DevSize;
		private Shark _Shark;

		public BeachScene(Core core) : base(core, nameof(BeachScene), "Assets")
		{
			_deathCounter = 0;
		}

		private void HandleDying()
		{
			_deathCounter++;
			if(_deathCounter >= Game.DeathLimit)
			{
                _gameOver.Score = _hud.Score;
				_gameOver.Visible = true;

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
			var inputMap = Game.CreateInput(0);
			inputMap.MappedOperationInvoked += (a,b) =>
			{
				if (b && a == GameAction.Pause) _Pause.Visible = true;
			};
			_Player = new Player(_Core, inputMap, TextureLoader, _Collisions.Where(c => c.Type == CollisionType.Normal).Select(c=>c.Shape).ToArray())
			{
				Position = _Collisions.Where(c => c.Type == CollisionType.P1Start).First().Shape.Position,
			};
			_Player.Act += OnP1Act;

			Layer_Game.Add(_Player);
			_DevSize = _Core.DeviceSize;
			HandleDeviceResize(_Core.DeviceSize);

			_hud = new(_Core, Input);
			Layer_Overlay.Add(_hud);

			// NPC
			Game.IsRunning = true;
			var partyArea = _Collisions.First(c => c.Type == CollisionType.PartyArea).Shape;
			_OceanArea = _Collisions.First(c => c.Type == CollisionType.Ocean).Shape;
			_Player.OceanArea = _OceanArea;
			_Npcs = new NPCManager(_Core, new(partyArea.Position, partyArea.Size), 10f, _OceanArea, TextureLoader, _hud);
            _Npcs.Dying += HandleDying;
            _Npcs.AddEntities(50);
			Layer_Game.Add(_Npcs);



			// PauseMenu
			_Pause = new PauseMenu(_Core, Input, TextureLoader) { Visible = false };
			//Game.IsRunning = false; // when pause menu is opened
			Layer_Overlay.Add(_Pause);

			_gameOver = new GameOverMenu(_Core, Input, TextureLoader) { Visible = false };
            _gameOver.Score = _hud.Score;
			Layer_Overlay.Add(_gameOver);

			_Shark = new Shark(_Core, TextureLoader, _OceanArea);
			_Shark.Position = _OceanArea.Position + _OceanArea.Size / 2;
			_Shark.Start();
			Layer_Game.Add(_Shark);

#if DEBUG
			// Collision Helper
			Layer_Game.Add(new ColHelp(_Core, _Player.CollisionShape));
			foreach (var item in _Collisions)
			{
				Layer_Game.Add(new ColHelp(_Core, item.Shape));
			}
#endif
			return true;
		}

		private void HandleDeviceResize(Vector2f deviceSize)
		{
			var scaleX = (float)deviceSize.X / _DevSize.X;
			var scaleY = (float)deviceSize.Y / _DevSize.Y;
			var m = (scaleX + scaleY) / 2; // or sqrt(scaleX * scaleY) for geometric mean
			_View.Size = deviceSize / m / 2;

			Vector2i half = _View.Size.ToVector2i() / 2;
			_ViewBounds = new IntRect(half, _MapSize - half);
		}

		protected override void Update(float deltaT)
		{
			_View.Center = new()
			{
				X = _Player.Position.X < _ViewBounds.Left ? _ViewBounds.Left :
					_Player.Position.X > _ViewBounds.Width ? _ViewBounds.Width : _Player.Position.X,
				Y = _Player.Position.Y < _ViewBounds.Top ? _ViewBounds.Top :
					_Player.Position.Y > _ViewBounds.Height ? _ViewBounds.Height : _Player.Position.Y
			};
			//_View.Center = (_Player.Position - _Shark.Position) / -2 + _Player.Position;
			_Player.Velocity = _Player.CollisionShape.CollidesWith(_OceanArea) ? 0.5f : 1;
		}

		protected override void Destroy()
		{
		}

		private void OnP1Act(bool activate)
		{
			if (activate) // down
			{
				var npc = _Npcs.GetAll<NPC>().FirstOrDefault(npc => npc.CollisionShape.CollidesWith(_Player.CollisionShape));
				if (npc != null)
                {
                    _Player.GrabbedNPC = npc;
                    if (npc.State == NPCState.Swiming || npc.State == NPCState.Drowning)
					{
						npc.Position = default;
						npc.State = NPCState.Rescue;
						_Player.Add(npc);
					}
				}
			}
			else // release
			{
                _Player.GrabbedNPC = _Player.GetAll<NPC>().FirstOrDefault();
                if (_Player.GrabbedNPC != null)
                {
                    if (_Player.CollisionShape.CollidesWith(_OceanArea))
					{
						_Npcs.Add(_Player.GrabbedNPC);
						_Player.GrabbedNPC.Position = _Player.Position;
						_Player.GrabbedNPC.State = NPCState.Drowning;
                        _Player.GrabbedNPC.StartDrowning();
					}
					else
					{
						_Player.Remove(_Player.GrabbedNPC);
						_hud.Score += 100;
						_Npcs.AddEntity(_Npcs.PosInPartyArea);
						_hud.IsBlinking = _Npcs.GetAll<NPC>().Any(n => n.State == NPCState.Drowning || n.State == NPCState.Swiming);
					}
				}
			}
		}
	}
}
