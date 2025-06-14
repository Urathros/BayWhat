using BlackCoat;
using BlackCoat.Animation;
using BlackCoat.Collision;
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
        
        FloatRect _area;

        const float DURATION = 1f;

        /// <summary>
        /// Time the next NPC needs to become drunken
        /// </summary>
        public float DrunkSeconds { get; set; }


        private ICollisionShape _oceanCollision;

        public NPCManager(Core core, FloatRect area, float drunkSeconds, ICollisionShape oceanCollision) : base(core)
        {
            _area = area;
            DrunkSeconds = drunkSeconds;
            _oceanCollision = oceanCollision;
            _Core.AnimationManager.Wait(_Core.Random.NextFloat(DrunkSeconds -2, DrunkSeconds+2), HandleDrunkenStateRandom);
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
            var npc = new NPC(_Core);
            npc.Position = position;
            npc.OceanCollision = _oceanCollision;
            Add(npc);
            return this;
        }

        public void AddEntities(int size)
        {
            for (int i = 0; i < size; i++) AddEntity(_Core.Random.NextVector(_area));
        }

    }
}
