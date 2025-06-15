using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Entities.Animation;
using BlackCoat.Collision.Shapes;
using BlackCoat.Collision;
using BlackCoat.Animation;

namespace BayWhat
{
	class Shark : Container
	{
		private readonly RectangleCollisionShape _Ocean;

		private BlittingAnimation _Swim;
		private BlittingAnimation _Attack;

		private Vector2f _Dimensions;
		private Vector2f _DimensionsCenter;
		public float Velocity { get; set; } = 1;

		public Shark(Core core, TextureLoader texLoader, RectangleCollisionShape ocean) : base(core)
		{
			_Ocean = ocean;

			_Dimensions = new Vector2f(32, 32);
			_DimensionsCenter = new Vector2f(_Dimensions.X / 2, _Dimensions.Y);
			CollisionShape = new RectangleCollisionShape(_Core.CollisionSystem, Position, _DimensionsCenter);

			// PlayerGfx
			Add(new Rectangle(_Core, _Dimensions, Color.Magenta));
			var pos = new Vector2f(_DimensionsCenter.X, -_Dimensions.Y);
			var origin = new Vector2f(_DimensionsCenter.X, 0);
			var frametime = 0.2f;
			Texture char_tex = texLoader.Load("baywhat_mc");
			_Swim = new BlittingAnimation(_Core, frametime, char_tex, Game.CalcFrames(char_tex, _Dimensions.ToVector2u(), 8, 11).ToArray())
			{
				Position = pos,
				Origin = origin
			};
			Add(_Swim);
		}

		public void Start()
		{
			if (Disposed) return;
			Log.Debug("start");
			var target = _Core.Random.NextVector(_Ocean.Position, _Ocean.Position+_Ocean.Size);
			target.X = (float)Math.Round(target.X);
			target.Y = (float)Math.Round(target.Y);

			_Swim.Scale = new Vector2f(target.X < Position.X ? -1 : 1, 1);
			var dur = _Core.Random.NextFloat(2, 5);
			_Core.AnimationManager.Run(Position.X, target.X, dur, v => Position = new(v, Position.Y), Done, InterpolationType.InCubic);
			_Core.AnimationManager.Run(Position.Y, target.Y, dur, v => Position = new(Position.X, v), null, InterpolationType.OutCubic);
		}

		private void Done()
		{
			Log.Debug("done");
			var dur = _Core.Random.NextFloat(2, 5);
			_Core.AnimationManager.Wait(dur, Start);
		}
	}
}