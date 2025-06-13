using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;
using SFML.Graphics;
using SFML.System;

namespace BayWhat
{
    public class TestScene : Scene
    {
        NPCManager _npcs;

        public TestScene(Core core) : base(core, string.Empty)
        {
            var pos = _Core.DeviceSize / 2;
            pos.X = pos.X + 50f;
            pos.Y = pos.Y + 50f;
            _npcs = new NPCManager(core, new(new(0f, 0f), _Core.DeviceSize)); 
            _npcs.AddEntities(5);
        }

        protected override bool Load()
        {
            //var rect = new Rectangle(_Core, new Vector2f(100f, 100f), Color.Red);
            //rect.Position = _Core.DeviceSize / 2;
            //rect.Origin = rect.Size / 2;
            //Layer_Game.Add(rect);

            //_Core.AnimationManager.Run(0, 360, 5, v => rect.Rotation = v);
            //OpenInspector(Layer_Game);


            Layer_Game.Add(_npcs);
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
