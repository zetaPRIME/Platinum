using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class ExtTexture
	{
		public Texture2D texture;
		public float baseScale = 1;
		public int pixelScale = 1;

		public Vector2 center
		{
			get { return new Vector2(texture.Width / 2f, texture.Height / 2f); }
		}
	}
}
