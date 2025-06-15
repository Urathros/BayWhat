using BlackCoat;
using BlackCoat.Animation;
using BlackCoat.Collision;
using BlackCoat.Collision.Shapes;
using BlackCoat.Entities;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BayWhat
{
    internal class NPCManager : Container
    {
        
        FloatRect _partyArea;

        const float DURATION = 1f;

        /// <summary>
        /// Time the next NPC needs to become drunken
        /// </summary>
        public float DrunkSeconds { get; set; }
        public Vector2f Offset => _partyArea.Position();

        public Vector2f PosInPartyArea => _Core.Random.NextVector(_partyArea);



        private RectangleCollisionShape _oceanCollision;
        private HUD _hud;
		private string[] _Texts;
		private readonly TextureLoader _TextureLoader;

        public Action Dying = () => { };

		public NPCManager(Core core, FloatRect partyArea, float drunkSeconds, RectangleCollisionShape oceanCollision, TextureLoader textureLoader, HUD hud) : base(core)
        {
            _partyArea = partyArea;
            DrunkSeconds = drunkSeconds;
            _oceanCollision = oceanCollision;
			_TextureLoader = textureLoader;
			_Core.AnimationManager.Wait(_Core.Random.NextFloat(DrunkSeconds -2, DrunkSeconds+2), HandleDrunkenStateRandom);
            _hud = hud;

            _Texts = File.ReadAllLines("Assets\\PartyTexts.txt");
        }

        private void HandleDrunkenStateRandom()
		{
			if (Disposed) return;
			NPC npc = GetRandomNPC();

			if (npc.State == NPCState.Drunken)
			{
				HandleDrunkenStateRandom();
				return;
			}

			npc.State = NPCState.Drunken;
			_Core.AnimationManager.Wait(_Core.Random.NextFloat(DrunkSeconds - 2, DrunkSeconds + 2), HandleDrunkenStateRandom);
		}

		private NPC GetRandomNPC()
		{
            var npcs = _Entities.OfType<NPC>().ToArray();
			return npcs[_Core.Random.Next(0, npcs.Length)];
		}

		public NPCManager AddEntity(Vector2f position)
        {
            var npc = new NPC(_Core, _TextureLoader);
            npc.Position = position;
            npc.OceanCollision = _oceanCollision;
            npc.PartyArea = _partyArea;
            npc.Hud = _hud;
            npc.Dying += () => Dying.Invoke();
            Add(npc);
            return this;
        }

        public void AddEntities(int size)
        {
            for (int i = 0; i < size; i++) AddEntity(PosInPartyArea);
        }

        public void StartTextSpawn()
        {
            if (Disposed) return;
            SpawnPartyText();
            _Core.AnimationManager.Wait(_Core.Random.Next(2,6), StartTextSpawn);
        }

        private void SpawnPartyText()
        {
            var text = _Texts[_Core.Random.Next(_Texts.Length)];
            var item = Game.GetPixelText(_Core, text, 10);
            item.Color = Color.Blue;
            Add(item);
            item.Origin = item.LocalBounds.Size() / 2;
            item.Position = GetRandomNPC().Position;
			_Core.AnimationManager.Run(item.Position.Y, item.Position.Y - 200, 4,
                                       v=>MoveText(v,item), ()=>KillText(item));
        }

		private void MoveText(float v, TextItem tex)
		{
            if (Disposed) return;
            tex.Position = new Vector2f(tex.Position.X, v);
		}

		private void KillText(TextItem tex)
		{
			if (Disposed) return;
            Remove(tex);
		}
	}
}
