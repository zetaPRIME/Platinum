﻿using System;
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
		public static Vector2 Scale(this Vector2 inp, float x, float y)
		{
			return new Vector2(inp.X * x, inp.Y * y);
		}

		public static Vector2 Clamp(this Vector2 inp, VecRect rect)
		{
			return new Vector2(Math.Max(rect.left, Math.Min(inp.X, rect.right)), Math.Max(rect.top, Math.Min(inp.Y, rect.bottom)));
		}

		public static float WrapRot(this float inp)
		{
			float res = inp % ((float)Math.PI * 2f);
			if (res < 0) res += ((float)Math.PI * 2f);
			return res;
		}

		public static Vector2 Transform(this Vector2 inp, Matrix matrix)
		{
			return Vector2.Transform(inp, matrix);
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

		// rect other stuff
		public static Point PointWithin(this Rectangle rect, Point pt)
		{
			return new Point(pt.X - rect.X, pt.Y - rect.Y);
		}
		public static Vector2 VecSize(this Rectangle rect)
		{
			return new Vector2(rect.Width, rect.Height);
		}

		// quick matrix invert
		public static Matrix Invert(this Matrix mtx) { return Matrix.Invert(mtx); }

		// quick upcast
		public static Vector3 Upcast(this Vector2 vec) { return new Vector3(vec, 0); }
	}
}
