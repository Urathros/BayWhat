using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackCoat;
using BlackCoat.Collision;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;
using SFML.Graphics;
using SFML.System;

namespace BayWhat
{
    public class TestScene : Scene
    {
        NPCManager _npcs;
        Rectangle _ocean;
        PauseMenu _pause;

        public TestScene(Core core) : base(core, string.Empty, "Assets")
        {
            Name = nameof(TestScene);
        }

        protected override bool Load()
        {
            //var rect = new Rectangle(_Core, new Vector2f(100f, 100f), Color.Red);
            //rect.Position = _Core.DeviceSize / 2;
            //rect.Origin = rect.Size / 2;
            //Layer_Game.Add(rect);

            //_Core.AnimationManager.Run(0, 360, 5, v => rect.Rotation = v);
            //OpenInspector(Layer_Game);
            _ocean = new(_Core, _Core.DeviceSize, Color.Blue);
            _ocean.Position = new(0, _Core.DeviceSize.Y - 1);


            Game.IsRunning = true;

            _npcs = new NPCManager(_Core, new(new(0f, 0f), _Core.DeviceSize), 10f, _ocean);
            _npcs.AddEntities(50);

            //_pause = new(_Core, Input);
            //Game.IsRunning = false; // when pause menu is opened

            Layer_Game.Add(_npcs);
            Layer_Game.Add(_ocean);
            //Layer_Game.Add(_pause);

            //var font = FontLoader.Load("PixelifySans-Regular");
            var text = Game.GetPixelText(_Core, "Pugs", 32);
            text.Position = _Core.DeviceSize / 2;
            Layer_Game.Add(text);

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
