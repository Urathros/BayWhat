using BlackCoat;
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

        public NPCManager(Core core, FloatRect area) : base(core)
        {
            _area = area;
        }

        public NPCManager AddEntity(Vector2f position)
        {
            var npc = new NPC(_Core);
            npc.Position = position;
            Add(npc);
            return this;
        }

        public void AddEntities(int size)
        {
            for (int i = 0; i < size; i++) AddEntity(_Core.Random.NextVector(_area));
        }
    }
}
