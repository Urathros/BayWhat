using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayWhat
{
    internal class HUD : UIContainer
    {
        private Label _scoreLabel;
        private int _score;

        public int Score
        {
            get => _score; 
            set 
            { 
                _score = value;
                _scoreLabel.Text = $"Score: {_score}";
            }
        }


        public HUD(Core core, Input input, params UIComponent[] components) : base(core, components)
        {
            Name = nameof(HUD);
            Input = new UIInput(input, true);

            _scoreLabel = Game.GetPixelLabel(core, "Score: xxxxxxxx", 60);
            _scoreLabel.Position = new(core.DeviceSize.X - 280f , 20f);

            Score = 0;

            Init = new[]
            {
                _scoreLabel
            };
        }
    }
}
    