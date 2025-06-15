using Accessibility;
using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Animation;
using SFML.Graphics;
using SFML.System;
using System;

namespace BayWhat
{
	internal class NPCAnimations : Container
	{
		private BlittingAnimation _Idle;
		private BlittingAnimation _Walk;
		private BlittingAnimation _Party;
		private BlittingAnimation _DrunkWalk;
		private BlittingAnimation _Swim;
		private BlittingAnimation _Drown;
		private BlittingAnimation _IdleOV;
		private BlittingAnimation _WalkOV;
		private BlittingAnimation _PartyOV;
		private BlittingAnimation _DrunkWalkOV;
		private BlittingAnimation _SwimOV;
		private BlittingAnimation _DrownOV;

		public NPCAnimations(Core core, TextureLoader texLoader) : base(core)
		{
			// Coinflips
			bool bottle = _Core.Random.Next(2) == 0;
			bool female = _Core.Random.Next(2) == 0;

			var baseTex = texLoader.Load($"NPC_{(female ? nameof(female) : nameof(female)[2..])}_base");
			var overlayTex = texLoader.Load($"NPC_{(female ? nameof(female) : nameof(female)[2..])}_overlay");

			var frame = new Vector2u(32, 32);
			var frameTime = 0.2f;
			// Base
			var idleFrames = (bottle ? Game.CalcFrames(baseTex, frame, 2, 3) : Game.CalcFrames(baseTex, frame, 0, 1)).ToArray();
			_Idle = new BlittingAnimation(_Core, frameTime, baseTex, idleFrames);
			var walkFrames = (bottle ? Game.CalcFrames(baseTex, frame, 4, 7) : Game.CalcFrames(baseTex, frame, 8, 11)).ToArray();
			_Walk = new BlittingAnimation(_Core, frameTime, baseTex, walkFrames);
			var partyFrames = (bottle ? Game.CalcFrames(baseTex, frame, 12, 13) : Game.CalcFrames(baseTex, frame, 14, 15)).ToArray();
			_Party = new BlittingAnimation(_Core, frameTime, baseTex, partyFrames);
			_DrunkWalk = new BlittingAnimation(_Core, frameTime, baseTex, Game.CalcFrames(baseTex, frame, 16, 17).ToArray());
			_Swim = new BlittingAnimation(_Core, frameTime, baseTex, Game.CalcFrames(baseTex, frame, 20, 21).ToArray());
			_Drown = new BlittingAnimation(_Core, frameTime, baseTex, Game.CalcFrames(baseTex, frame, 22, 23).ToArray());
			//Overlay
			Color tint = GenerateVibrantColor();
			_IdleOV = new BlittingAnimation(_Core, frameTime, overlayTex, idleFrames) { Color = tint };
			_WalkOV = new BlittingAnimation(_Core, frameTime, overlayTex, walkFrames) { Color = tint };
			_PartyOV = new BlittingAnimation(_Core, frameTime, overlayTex, partyFrames) { Color = tint };
			_DrunkWalkOV = new BlittingAnimation(_Core, frameTime, overlayTex, Game.CalcFrames(baseTex, frame, 16, 17).ToArray()) { Color = tint };
			_SwimOV = new BlittingAnimation(_Core, frameTime, overlayTex, Game.CalcFrames(baseTex, frame, 20, 21).ToArray()) { Color = tint };
			_DrownOV = new BlittingAnimation(_Core, frameTime, overlayTex, Game.CalcFrames(baseTex, frame, 22, 23).ToArray()) { Color = tint };
		}

		public void Show(NPCState state)
		{
			Clear();
			switch (state)
			{
				case NPCState.Dancing:
					Add(_Walk);
					Add(_WalkOV);
					break;
				case NPCState.Drunken:
					Add(_DrunkWalk);
					Add(_DrunkWalkOV);
					break;
				case NPCState.Swiming:
				case NPCState.Rescue:
					Add(_Swim);
					Add(_SwimOV);
					break;
				case NPCState.Idle:
					Add(_Idle);
					Add(_IdleOV);
					break;
				case NPCState.Partying:
					Add(_Party);
					Add(_PartyOV);
					break;
				case NPCState.Drowning:
					Add(_Drown);
					Add(_DrownOV);
					break;
			}
		}

		private Color GenerateVibrantColor()
		{
			// Random hue between 0 and 360 degrees
			double h = _Core.Random.NextDouble() * 360;

			// High saturation and brightness (vibrancy)
			double s = _Core.Random.NextDouble() * 0.3 + 0.7; // 0.7 to 1.0
			double v = _Core.Random.NextDouble() * 0.2 + 0.8; // 0.8 to 1.0

			return HsvToRgb(h, s, v);
		}

		// HSV to RGB conversion
		private Color HsvToRgb(double h, double s, double v)
		{
			double c = v * s;
			double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
			double m = v - c;

			double rPrime = 0, gPrime = 0, bPrime = 0;

			if (h < 60)
			{
				rPrime = c; gPrime = x; bPrime = 0;
			}
			else if (h < 120)
			{
				rPrime = x; gPrime = c; bPrime = 0;
			}
			else if (h < 180)
			{
				rPrime = 0; gPrime = c; bPrime = x;
			}
			else if (h < 240)
			{
				rPrime = 0; gPrime = x; bPrime = c;
			}
			else if (h < 300)
			{
				rPrime = x; gPrime = 0; bPrime = c;
			}
			else
			{
				rPrime = c; gPrime = 0; bPrime = x;
			}

			byte r = (byte)Math.Round((rPrime + m) * 255);
			byte g = (byte)Math.Round((gPrime + m) * 255);
			byte b = (byte)Math.Round((bPrime + m) * 255);
			return new Color(r,g,b);
		}
		public void Flip(bool left)
		{
			foreach (var item in GetAll<BlittingAnimation>())
			{
				item.Position = item.Origin = new(16, 0);
				item.Scale = new(left ? -1 : 1, 1);
			}
		}
	}
}
