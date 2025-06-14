﻿using BlackCoat;
using BlackCoat.Animation;
using BlackCoat.Collision;
using BlackCoat.Collision.Shapes;
using BlackCoat.Entities;
using BlackCoat.Entities.Animation;
using BlackCoat.Entities.Shapes;
using SFML.Graphics;
using SFML.System;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace BayWhat
{
    enum NPCState : byte
    {
        Idle,
        Dancing,
        Partying,
        Drunken,
        Swiming,
        Drowning,
        Rescue
    }
    internal class NPC : Container
    {

        #region Constants
        const float START_VAL = 0f;
        const float END_VAL = 1f;
        const float DURATION = .5f;
        const float MIN_DANCE_SPEED = 48f;
        const float MAX_DANCE_SPEED = 52f;
        const float MIN_SWIM_SPEED = MIN_DANCE_SPEED - 10;
        const float MAX_SWIM_SPEED = MAX_DANCE_SPEED - 10;

        readonly Vector2f ZERO = new(0f, 0f);
        readonly Vector2f FORWARD = new(0f, 1f);
        readonly Vector2f BACKWARD = new(0f, -1f);
        readonly Vector2f RIGHT = new(1f, 0f);
        readonly Vector2f LEFT = new(-1f, 0f);

        #endregion

        #region Fields

        private NPCState _state;

        private NPCAnimations _sprite;

        /// <summary>
        /// Walking Direction
        /// </summary>
        private Vector2f _direction;

        private Vector2f _drowningPos = new(0f, 0f);
        private float _deltaT;
        private float _speed;

        public RectangleCollisionShape OceanCollision { get; set; }

        public FloatRect PartyArea { get; set; }

        #endregion
        public NPCState State 
        {
            get => _state;
            set
            {
                _state = value;
                _sprite.Show(_state);
            }

        }

        public HUD Hud { get; set; }


        public Vector2f Position
        {
            get => _sprite.Position; 
            set => _sprite.Position = value; 
        }
		public Vector2f CollisionOffset { get; }


        public Action Dying = () => { };

        Vector2f CalcDirection(Vector2f startDir, Vector2f endDir)
        {
            return (endDir - startDir).Normalize();
        }

        public void ShowAnim(NPCState state) => _sprite.Show(state);

        void DecideDirection()
        {
            var rand = _Core.Random.Next(1, 5);
            switch (rand)
            {
                case 1: _direction = FORWARD; break;
                case 2: _direction = LEFT; break;
                case 3: _direction = BACKWARD; break;
                case 4: _direction = RIGHT;  break;
                default:
                    break;
            }
        }

        void ChangePartyMode()
        {
            if (!(State == NPCState.Idle || State == NPCState.Dancing || State == NPCState.Partying)) return;

            var rand = _Core.Random.Next(1, 4);

            switch (rand)
            {
                case 1: State = NPCState.Idle; break;
                case 2: State = NPCState.Dancing; break;
                case 3: State = NPCState.Partying; break;
                default:
                    break;
            }

            _Core.AnimationManager.Wait(2, ChangePartyMode);

        }

        void HandleMoving(Vector2f dir)
        {
            if (!Game.IsRunning || Disposed) return;

            DecideDirection();

            var potientialX = dir.X + Position.X;
            var potentialY = dir.Y + Position.Y;
            if (    ( potientialX < PartyArea.Left || potientialX > PartyArea.Left + PartyArea.Width || potentialY < PartyArea.Top || potentialY > PartyArea.Top + PartyArea.Height)
                 && (State == NPCState.Idle || State == NPCState.Dancing || State == NPCState.Partying)) { }
            else _sprite.Translate(dir * _speed * _deltaT);


            //_sprite.Scale = new(dir.X > 0 ? 1 : -1 , 1); // TODO: Alex einbinden, wenn Origin stimmt
            if(dir.X != 0) _sprite.Flip(dir.X > 0);
        }

        void HandleStateBehaviour(Animation? anim = null)
        {
            if (Disposed) return;
            switch (State)
            {
                case NPCState.Idle://nobreak
                case NPCState.Partying: _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(ZERO), HandleStateBehaviour); break; 
                case NPCState.Dancing:
                {
                    if (_direction == FORWARD) _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(LEFT), HandleStateBehaviour);
                    else if (_direction == LEFT) _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(BACKWARD), HandleStateBehaviour);
                    else if (_direction == BACKWARD) _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(RIGHT), HandleStateBehaviour);
                    else if (_direction == RIGHT) _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(FORWARD), HandleStateBehaviour);
                }
                    break;
                case NPCState.Drunken:
                    _Core.AnimationManager.RunAdvanced(START_VAL, END_VAL, DURATION, v => HandleMoving(BACKWARD), HandleStateBehaviour);
                    break;
                case NPCState.Swiming:
                    _speed = _Core.Random.NextFloat(MIN_SWIM_SPEED, MAX_SWIM_SPEED);
                    if(_drowningPos == new Vector2f(0f, 0f)) _drowningPos = _Core.Random.NextVector(OceanCollision.Position.X,
                                                              OceanCollision.Position.X + OceanCollision.Size.X,
                                                              OceanCollision.Position.Y,
                                                              OceanCollision.Position.Y + OceanCollision.Size.Y);
                    _Core.AnimationManager.RunAdvanced( START_VAL, END_VAL, DURATION, 
                                                        v => HandleMoving( CalcDirection(_direction, _drowningPos)), HandleStateBehaviour);
                    if (Position.Y >= _drowningPos.Y) State = NPCState.Drowning;
                    break;
                case NPCState.Rescue:
                    break;
				case NPCState.Drowning:
					StartDrowning();
					break;
				default:
                    break;
            }
        }

		public void StartDrowning()
		{
            _Core.AnimationManager.Wait(Game.DrowningTime, () => 
            { 
                Parent?.Remove(this);
                Dying.Invoke(); 
            });
		}

		public NPC(Core core, TextureLoader texLoader) : base(core)
        {
            _sprite ??= new(core, texLoader);
            Add(_sprite);
            _direction = FORWARD;
            _speed = _Core.Random.NextFloat(MIN_DANCE_SPEED, MAX_DANCE_SPEED);
            State = NPCState.Dancing;

            ChangePartyMode();

            HandleStateBehaviour();

            var big = new Vector2f(32, 32); // frame size
            var smol = new Vector2f(13, 11); // size of area containing pixel in gfx
            CollisionOffset = (big - smol) / 2;
            CollisionShape = new RectangleCollisionShape(_Core.CollisionSystem, CollisionOffset, smol);

        }

        public override void Update(float deltaT)
        {
            base.Update(deltaT);
            _deltaT = deltaT;

			(CollisionShape as RectangleCollisionShape)!.Position = Position + CollisionOffset;

			if (OceanCollision.CollidesWith(_sprite.Position) && State == NPCState.Drunken)
            {
                State = NPCState.Swiming;

                if (!Hud.IsBlinking) Hud.IsBlinking = true;

            }

        }


        override public void Draw()
        {
            base.Draw();
        }
    }
}
