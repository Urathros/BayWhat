using SFML.System;
using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Animation;

namespace BayWhat
{
	internal class NPCAnimations : Container
	{
		private BlittingAnimation _MaleWalk;
		private BlittingAnimation _MaleDrunkWalk;
		private BlittingAnimation _MaleSwim;
		private BlittingAnimation _MaleDrownSwim;
		private BlittingAnimation _MaleWalkOV;
		private BlittingAnimation _MaleDrunkWalkOV;
		private BlittingAnimation _MaleSwimOV;
		private BlittingAnimation _MaleDrownSwimOV;

		public NPCAnimations(Core core, TextureLoader texLoader) : base(core)
		{
			// Coinflips
			//bool bottle = _Core.Random.Next(2) == 0;
			bool female = false;// _Core.Random.Next(2) == 0;

			var baseTex = texLoader.Load($"NPC_{(female ? nameof(female) : nameof(female)[2..])}_base");
			var overlayTex = texLoader.Load($"NPC_{(female ? nameof(female) : nameof(female)[2..])}_overlay");

			var frame = new Vector2u(32, 32);
			var frameTime = 0.2f;
			// Base
			_MaleWalk = new BlittingAnimation(_Core, frameTime, baseTex, Game.CalcFrames(baseTex, frame, 8, 11).ToArray());
			_MaleDrunkWalk = new BlittingAnimation(_Core, frameTime, baseTex, Game.CalcFrames(baseTex, frame, 16, 17).ToArray());
			_MaleSwim = new BlittingAnimation(_Core, frameTime, baseTex, Game.CalcFrames(baseTex, frame, 20, 21).ToArray());
			_MaleDrownSwim = new BlittingAnimation(_Core, frameTime, baseTex, Game.CalcFrames(baseTex, frame, 22, 23).ToArray());
			//Overlay
			_MaleWalkOV = new BlittingAnimation(_Core, frameTime, overlayTex, Game.CalcFrames(baseTex, frame, 8, 11).ToArray());
			_MaleDrunkWalkOV = new BlittingAnimation(_Core, frameTime, overlayTex, Game.CalcFrames(baseTex, frame, 16, 17).ToArray());
			_MaleSwimOV = new BlittingAnimation(_Core, frameTime, overlayTex, Game.CalcFrames(baseTex, frame, 20, 21).ToArray());
			_MaleDrownSwimOV = new BlittingAnimation(_Core, frameTime, overlayTex, Game.CalcFrames(baseTex, frame, 22, 23).ToArray());
		}

		public void Show(NPCState state)
		{
			Clear();
			switch (state)
			{
				case NPCState.Dancing:
					Add(_MaleWalk);
					Add(_MaleWalkOV);
					break;
				case NPCState.Drunken:
					Add(_MaleDrunkWalk);
					Add(_MaleDrunkWalkOV);
					break;
				case NPCState.Swiming:
				case NPCState.Rescue:
					Add(_MaleSwim);
					Add(_MaleSwimOV);
					break;
				case NPCState.Idle:
					break;
				case NPCState.Partying:
					break;
				case NPCState.Drowning:
					Add(_MaleDrownSwim);
					Add(_MaleDrownSwimOV);
					break;
				default:
					break;
			}
		}
	}
}
