using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Platinum.Wrappers
{
	public struct Vector2Wrapper
	{
		public Vector2 value;

		public static implicit operator Vector2Wrapper(Vector2 vec)
		{
			Vector2Wrapper w = new Vector2Wrapper();
			w.value = vec;
			return w;
		}

		// facilitate matrix mult
		public static Vector2 operator *(Vector2Wrapper vec, Matrix mtx) { return Vector2.Transform(vec.value, mtx); }
		public static Vector2 operator /(Vector2Wrapper vec, Matrix mtx) { return Vector2.Transform(vec.value, Matrix.Invert(mtx)); }
	}
}
