using BlackCoat;
using BlackCoat.Animation;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;
using SFML.Graphics;
using SFML.System;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayWhat
{
    enum NPCState : byte
    {
        Dancing,
        Drunken,
        Swiming,
        Rescure
    }
    internal class NPC : Container
    {

        #region Constants
        const float START_VAL = 0f;
        const float END_VAL = 1f;
        const float DURATION = .5f;

        readonly Vector2f FORWARD = new(0f, 1f);
        readonly Vector2f BACKWARD = new(0f, -1f);
        readonly Vector2f RIGHT = new(1f, 0f);
        readonly Vector2f LEFT = new(-1f, 0f);

        #endregion

        #region Fields
        Rectangle _sprite;

        /// <summary>
        /// Walking Direction
        /// </summary>
        Vector2f _direction;
        float _deltaT;
        float _speed;
        float _duration;

        #endregion
        public NPCState State { get; set; }


        public Vector2f Position
        {
            get => _sprite.Position; 
            set => _sprite.Position = value; 
        }


        void HandleMoving(Vector2f dir)
        {
            switch (_Core.Random.Next(1, 5))
            {
                case 1: _direction = FORWARD; break;
                case 2: _direction = LEFT;break;
                case 3: _direction = BACKWARD;break;
                case 4: _direction = RIGHT; break;
                default:
                    break;
            }
            
            _sprite.Translate(dir * _speed * _deltaT);
        }

        void HandleDirectionChange(Animation? anim = null)
        {
            switch (State)
            {
                case NPCState.Dancing:
                    {
                        if (_direction == FORWARD) _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(LEFT), HandleDirectionChange);
                        else if (_direction == LEFT) _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(BACKWARD), HandleDirectionChange);
                        else if (_direction == BACKWARD) _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(RIGHT), HandleDirectionChange);
                        else if (_direction == RIGHT) _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(FORWARD), HandleDirectionChange);
                    }
                    break;
                case NPCState.Drunken:
                    _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(FORWARD), HandleDirectionChange);
                    break;
                case NPCState.Swiming:
                    break;
                case NPCState.Rescure:
                    break;
                default:
                    break;
            }
        }


        public NPC(Core core, string name = "") : base(core)
        {
            Name = $"{nameof(NPC)}_{name}";

            _sprite ??= new(core, new Vector2f(32f, 32f), Color.Blue);
            //_sprite.Position = _Core.DeviceSize / 2;
            _sprite.Origin = _sprite.Size / 2;
            Add(_sprite);
            _direction = FORWARD;
            _speed = (float)_Core.Random.Next(48, 52);
            _duration = _Core.Random.NextFloat(.4f, .6f);
            State = NPCState.Dancing;
            


            HandleDirectionChange();
        }

        public override void Update(float deltaT)
        {
            base.Update(deltaT);

            _deltaT = deltaT;
            
        }


        override public void Draw()
        {
            base.Draw();
        }
    }
}
