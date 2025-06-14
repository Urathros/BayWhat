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
        }

        private void HandleDrunkenStateRandom()
        {
            var npc = ((NPC)_Entities[_Core.Random.Next(0, _Entities.Count)]);
            
            if(npc.State == NPCState.Drunken)
            {
                HandleDrunkenStateRandom();
                return;
            }

            npc.State = NPCState.Drunken;
            _Core.AnimationManager.Wait(_Core.Random.NextFloat(DrunkSeconds - 2, DrunkSeconds + 2), HandleDrunkenStateRandom);
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

    }
}
