using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;
using BlackCoat.InputMapping;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Entities.Animation;
using BlackCoat.Collision.Shapes;

namespace BayWhat
{
    class Player : Container
    {
        private SimpleInputMap<GameAction> _InputMap;
		private readonly RectangleCollisionShape[] _Blocker;

		public event Action<bool> Act = a => { };

        private BlittingAnimation _Idle;
        private BlittingAnimation _Walk;
        private BlittingAnimation _Swim;

		private Vector2f _Dimensions;
        private Vector2f _DimensionsCenter;
        private Vector2f _Direction;
        private Vector2f _Velocity = new(200,200);
		private Vector2f _CollisionOffset = new(8, -32);

		public float Velocity { get; set; } = 1;

		public Player(Core core, SimpleInputMap<GameAction> inputMap, TextureLoader texLoader, RectangleCollisionShape[] blocker) : base(core)
        {
            _InputMap = inputMap;
			_Blocker = blocker;
			_InputMap.MappedOperationInvoked += HandleInput;

            _Dimensions = new Vector2f(32, 32);
            _DimensionsCenter = new Vector2f(_Dimensions.X / 2, _Dimensions.Y);
            CollisionShape = new RectangleCollisionShape(_Core.CollisionSystem, Position, _DimensionsCenter);

            // PlayerGfx
            //Add(new Rectangle(_Core, _Dimensions, Color.Yellow));
            var pos = new Vector2f(_DimensionsCenter.X, -_Dimensions.Y);
            var origin = new Vector2f(_DimensionsCenter.X, 0);
            var frametime = 0.2f;
			Texture char_tex = texLoader.Load("baywhat_mc");
			_Idle = new BlittingAnimation(_Core, frametime, char_tex, Game.CalcFrames(char_tex, _Dimensions.ToVector2u(), 0, 3).ToArray())
			{
				Position = pos,
				Origin = origin
			};
			Add(_Idle);
			_Walk = new BlittingAnimation(_Core, frametime, char_tex, Game.CalcFrames(char_tex, _Dimensions.ToVector2u(), 4, 7).ToArray())
			{
				Position = pos,
				Origin = origin,
				Visible = false
			};
			Add(_Walk);
			_Swim = new BlittingAnimation(_Core, frametime, char_tex, Game.CalcFrames(char_tex, _Dimensions.ToVector2u(), 8, 11).ToArray())
			{
				Position = pos,
				Origin = origin,
				Visible = false
			};
			Add(_Swim);
		}


        private void HandleInput(GameAction action, bool activate)
        {
            switch (action)
			{
				case GameAction.Left:
					_Direction = new Vector2f(activate ? -1 : (_Direction.X == -1 ? 0 : _Direction.X), _Direction.Y);
					if(activate) _Walk.Scale = _Idle.Scale = _Swim.Scale = new(-1, 1);
					break;
				case GameAction.Right:
					_Direction = new Vector2f(activate ? 1 : (_Direction.X == 1 ? 0 : _Direction.X), _Direction.Y);
					if (activate) _Walk.Scale = _Idle.Scale = _Swim.Scale = new(1, 1);
					break;
				case GameAction.Up:
					_Direction = new Vector2f(_Direction.X, activate ? -1 : (_Direction.Y == -1 ? 0 : _Direction.Y));
					break;
				case GameAction.Down:
					_Direction = new Vector2f(_Direction.X, activate ? 1 : (_Direction.Y == 1 ? 0 : _Direction.Y));
					break;
                case GameAction.Act:
                    Act(activate);
                    break;
            }
        }

        public override void Update(float deltaT)
        {
            base.Update(deltaT);

			// GFX
            if (Velocity == 1)
            {
                _Idle.Visible = _Direction == default;
                _Walk.Visible = !_Idle.Visible;
				_Swim.Visible = false;
			}
            else
            {
                _Idle.Visible = _Walk.Visible = false;
                _Swim.Visible = true;
			}

            // MOVE
            Position += _Direction.MultiplyBy(_Velocity*Velocity) * deltaT;
			(CollisionShape as RectangleCollisionShape)!.Position = Position + _CollisionOffset;
			while( _Blocker.Any(b => b.CollidesWith(CollisionShape)))
			{
				Position -= _Direction;
				(CollisionShape as RectangleCollisionShape)!.Position = Position + _CollisionOffset;
			}

			if (Position.X < -1000 || Position.X > 2500 || Position.Y < -100 || Position.Y > 5000) Position = _Core.DeviceSize / 2;
        }

        public void Destroy()
        {
            _InputMap.MappedOperationInvoked -= HandleInput;
        }
    }
}