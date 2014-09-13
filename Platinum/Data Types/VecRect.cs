using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public struct VecRect
	{
		public Vector2 topLeft;
		public Vector2 bottomRight;

		//public VecRect() : this(Vector2.Zero, Vector2.Zero) { }
		public VecRect(Vector2 topLeft, Vector2 bottomRight)
		{
			this.topLeft = topLeft; this.bottomRight = bottomRight;
		}

		#region operators and static defs
		public static VecRect operator +(VecRect orig, Vector2 offset)
		{
			return new VecRect(orig.topLeft + offset, orig.bottomRight + offset);
		}
		public static VecRect operator -(VecRect orig, Vector2 offset)
		{
			return orig + (offset * -1f);
		}

		public static VecRect Zero { get { return new VecRect(Vector2.Zero, Vector2.Zero); } }
		#endregion

		public Vector2 Size
		{
			get { return bottomRight - topLeft; }
		}
	}
}
