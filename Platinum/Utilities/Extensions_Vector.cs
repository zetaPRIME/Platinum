using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public static partial class Extensions
	{
		public static Vector2 Pixelize(this Vector2 inp)
		{
			return new Vector2((float)((int)inp.X), (float)((int)inp.Y));
		}
	}
}
