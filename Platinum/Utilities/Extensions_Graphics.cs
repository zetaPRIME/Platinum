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
		public static void Draw(this SpriteBatch sb, ExtTexture texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects)
		{
			sb.Draw(texture.texture, position, sourceRectangle, color, rotation, origin, scale * texture.baseScale, SpriteEffects.None, 0f);
		}
	}
}
