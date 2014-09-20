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

		// rect margins
		public static Rectangle MarginLeft(this Rectangle inp, int margin)
		{
			return new Rectangle(inp.X, inp.Y, margin, inp.Height);
		}
		public static Rectangle MarginRight(this Rectangle inp, int margin)
		{
			return new Rectangle((inp.X + inp.Width) - margin, inp.Y, margin, inp.Height);
		}
		public static Rectangle MarginTop(this Rectangle inp, int margin)
		{
			return new Rectangle(inp.X, inp.Y, inp.Width, margin);
		}
		public static Rectangle MarginBottom(this Rectangle inp, int margin)
		{
			return new Rectangle(inp.X, (inp.Y + inp.Height) - margin, inp.Width, margin);
		}
	}
}
