using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.UI;
using SFML.Graphics;
using SFML.System;
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
        private Label _warningLabel;
        private int _score;
        private Vector2f _sccoreLabelPosition;
        private Vector2f _warningLabelPosition;

        public int Score
        {
            get => _score; 
            set 
            { 
                _score = value;
                _scoreLabel.Text = $"Score: {_score}";
            }
        }


        public Vector2f ScoreLabelPosition
        {
            get => _sccoreLabelPosition; 
            set => _sccoreLabelPosition = new(value.X - 280f, 20f); 
        }


        public Vector2f WarningLabelPosition
        {
            get => _warningLabelPosition; 
            set => _warningLabelPosition = new(value.X - 160, value.Y - 50);
        }



        public HUD(Core core, Input input, params UIComponent[] components) : base(core, components)
        {
            Name = nameof(HUD);
            Input = new UIInput(input, true);

            ScoreLabelPosition = core.DeviceSize;
            WarningLabelPosition = core.DeviceSize;

            _scoreLabel = Game.GetPixelLabel(core, "Score: xxxxxxxx", 60);
            _scoreLabel.Position = ScoreLabelPosition;

            _warningLabel = Game.GetPixelLabel(core, "WARNING", 150);
            _warningLabel.TextColor = Color.Red;
            _warningLabel.Style = Text.Styles.Bold;
            _warningLabel.Position = WarningLabelPosition;

            Score = 0;

            Init = new UIComponent[]
            {
                _scoreLabel,
                _warningLabel
            };


            _Core.DeviceResized += HandleResize;
        }


        private void HandleResize(Vector2f size)
        {
            if(Disposed)
            {
                _Core.DeviceResized -= HandleResize;
                return;
            }

            ScoreLabelPosition = size;
            WarningLabelPosition = size;

            _scoreLabel.Position = ScoreLabelPosition;
            _warningLabel.Position = WarningLabelPosition;

        }
    }
}
    