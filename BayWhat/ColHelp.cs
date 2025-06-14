using BlackCoat;
using BlackCoat.Collision;
using BlackCoat.Collision.Shapes;
using BlackCoat.Entities.Shapes;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayWhat
{
	internal class ColHelp : Rectangle
	{
		private RectangleCollisionShape _Rect;

		public ColHelp(Core core, ICollisionShape src) : base(core, default, null, Color.Magenta)
		{
			_Rect = (src as RectangleCollisionShape)!;
			Size = _Rect.Size;
		}

		public override void Update(float deltaT)
		{
			base.Update(deltaT);
			Position = _Rect.Position;
		}
	}
}
